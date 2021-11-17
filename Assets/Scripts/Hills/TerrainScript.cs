using System.Collections.Generic;
using OpenSkiJumping.Hills.TerrainGenerator;
using OpenSkiJumping.ScriptableObjects.Variables;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    public enum TerrainBase
    {
        NodePoints,
        Flat,
        ElevationData
    }

    public class TerrainScript : MonoBehaviour
    {
        public bool alignHills;
        public float padding;
        public List<GameObject> hillObjects;
        private List<Transform> _hillTransforms;
        private List<MeshScript> _hills;
        private List<Hill> _hillsList;
        private NodePoint[] _dataPoints;

        public Terrain[] terrains;
        public float offset;
        public float hillYOffset;
        public float fallback;
        public float inrunFlatLength;
        [SerializeField] private HillsMapGenerator hillsMapGenerator;
        [SerializeField] private ElevationData elevationData;

        [SerializeField] private MapLocalTangentPlane mapLtp;
        [SerializeField] private HillsMapVariable hillsMap;

        [SerializeField] private float minHeight = 400;
        [SerializeField] private float power = 2;
        [SerializeField] private AnimationCurve smoothCurve;
        private List<(OffsetFunction, OffsetFunction)> _hillTerrain = new List<(OffsetFunction, OffsetFunction)>();


        private const float Eps = 0.1f;


        public GameObject terrainObject;
        public bool useElevationData;
        public bool useNodePoints;
        public TerrainBase terrainBase;
        [SerializeField] private float terrainHeight;


        public void GetTerrain()
        {
            terrains = terrainObject.GetComponentsInChildren<Terrain>();
        }

        public void GetHills()
        {
            _hills = new List<MeshScript>();
            _hillsList = new List<Hill>();
            _hillTransforms = new List<Transform>();

            foreach (var it in hillObjects)
            {
                if (!it.activeSelf) continue;
                var ms = it.GetComponentInChildren<MeshScript>();
                _hills.Add(ms);
                _hillsList.Add(ms.hill);
                _hillTransforms.Add(it.transform);
            }
        }

        public void AutoAlignHills()
        {
            for (var i = 0; i < hillObjects.Count; i++)
            {
                if (i > 0)
                {
                    _hillTransforms[i].position =
                        _hillTransforms[i - 1].position +
                        new Vector3(_hillsList[i - 1].U.x - _hillsList[i].U.x,
                            _hillsList[i - 1].U.y - _hillsList[i].U.y,
                            _hillsList[i - 1].bU / 2 + _hillsList[i].bU / 2 + padding);
                }
            }
        }

        public void GenerateTerrain()
        {
            mapLtp.centerCoords = hillsMap.Value.pos;
            mapLtp.SetValues();
            GetHills();
            GetTerrain();
            
            _hillTerrain.Clear();
            for (var ii = 0; ii < _hills.Count; ii++)
            {
                var hillId = hillsMap.Value.profiles[ii].profileData.name;
                var inrunTerrainFunction = hillsMapGenerator.GetOffsetFromPath($"{hillId}/inrun/terrain");
                var landingHillTerrainFunction = hillsMapGenerator.GetOffsetFromPath($"{hillId}/landing-hill/terrain");
                _hillTerrain.Add((inrunTerrainFunction, landingHillTerrainFunction));
            }

            if (alignHills)
            {
                AutoAlignHills();
            }

            var terrainBaseY = _hillTransforms[0].transform.position.y + _hillsList[0].U.y;

            foreach (var terr in terrains)
            {
                var center = terr.transform.position;
                var terrainData = terr.terrainData;
                var tab = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
                for (var i = 0; i < terrainData.heightmapResolution; i++)
                {
                    for (var j = 0; j < terrainData.heightmapResolution; j++)
                    {
                        var x = (float) j / (terrainData.heightmapResolution - 1) * terrainData.size.x + center.x;
                        var z = (float) i / (terrainData.heightmapResolution - 1) * terrainData.size.z + center.z;
                        var terrainY = terrainBaseY;

                        if (useElevationData)
                        {
                            var resolution = terrainData.heightmapResolution - 1;
                            var tileSize = terrainData.size;
                            var coords =
                                mapLtp.ToGeog(center + new Vector3(j * tileSize.x, 0, i * tileSize.z) / resolution);
                            elevationData.GetElevation(coords, out var elevation);
                            terrainY = elevation - mapLtp.centerCoords.z;
                        }

                        if (useNodePoints)
                        {
                            terrainY = GetInterpolatedHeight(x, z, terrainY);
                        }

                        var weights = new List<(float weight, float height)>();
                        for (var ii = 0; ii < _hills.Count; ii++)
                        {
                            var hillTransform = _hills[ii].transform;
                            var hill = _hillsList[ii];
                            var pointOnHill = hillTransform.InverseTransformPoint(new Vector3(x, 0, z));

                            var profileData = _hills[ii].profileData.Value;
                            // var hillRay = GetHillRay(pointOnHill, hill, terrainY, profileData.terrainSteepness,
                            //     1, hillTransform);                      
                            var hillRay = GetHillRay2(pointOnHill, hill, profileData.terrainSteepness,
                                hillTransform, ii);

                            var distVec = new Vector2(Mathf.Max(0, Mathf.Abs(pointOnHill.z) - hillRay.z),
                                Mathf.Max(0, hillRay.x));
                            var clampedDist = Mathf.Clamp01(distVec.magnitude);
                            var blendFactor = smoothCurve.Evaluate(clampedDist);

                            weights.Add((distVec.magnitude, hillRay.y));
                        }

                        var finalHeight = GetFinalHeight(weights, terrainY);
                        // var finalHeight = terrainY + center.y;
                        tab[i, j] = (finalHeight - center.y) / terr.terrainData.size.y;
                    }
                }

                terr.terrainData.SetHeights(0, 0, tab);
            }
        }

        private float GetFinalHeight(List<(float weight, float height)> weights, float terrainY)
        {
            var weightSum = 0f;
            var totalSum = 0f;
            foreach (var (weight, height) in weights)
            {
                var h = Mathf.Lerp(height, terrainY, Mathf.Clamp01(weight / offset));
                var dist = Mathf.Pow(weight, power);
                if (dist < Eps) return h;
                var w = 1f / dist;
                totalSum += h * w;
                weightSum += w;
            }

            return totalSum / weightSum;
        }


        private Vector3 GetHillRay(Vector3 pointOnHill, Hill hill, float terrainY, float inrun, float landingArea,
            Transform hillTransform)
        {
            var hillRay = Vector3.zero;
            terrainY -= hillTransform.position.y;

            if (pointOnHill.x < hill.A.x)
            {
                hillRay.x = hill.A.x - inrunFlatLength - pointOnHill.x;
                hillRay.y = hill.A.y * inrun - hill.s;
                hillRay.z = hill.b1 / 2;
            }
            else if (pointOnHill.x < hill.T.x)
            {
                hillRay.y = hill.Inrun(pointOnHill.x) * inrun - hill.s;
                if (hill.A.x <= pointOnHill.x) hillRay.z = hill.bK;
                hillRay.z = Mathf.Lerp(hill.b2, hill.b1, pointOnHill.x / hill.A.x) / 2;
            }
            else if (hill.T.x <= pointOnHill.x && pointOnHill.x <= hill.U.x)
            {
                hillRay.y = hill.LandingArea(pointOnHill.x);
                hillRay.z = pointOnHill.x <= hill.K.x
                    ? hill.b2 / 2 + pointOnHill.x / hill.K.x * ((hill.bK - hill.b2) / 2)
                    : pointOnHill.x >= hill.U.x
                        ? hill.bU / 2
                        : hill.bK / 2 + (pointOnHill.x - hill.K.x) / (hill.U.x - hill.K.x) *
                        ((hill.bU - hill.bK) / 2);
            }
            else if (pointOnHill.x <= hill.U.x + hill.a)
            {
                hillRay.y = hill.U.y;
                hillRay.z = hill.bU / 2;
            }
            else
            {
                hillRay.x = pointOnHill.x - hill.U.x - hill.a;
                hillRay.y = hill.U.y;
                hillRay.z = hill.bU / 2;
            }

            var position = hillTransform.position;
            hillRay.y += position.y - hillYOffset;
            return hillRay;
        }

        private Vector3 GetHillRay2(Vector3 pointOnHill, Hill hill, float inrun,
            Transform hillTransform, int hillId)
        {
            var (inrunTerrainFunction, landingHillTerrainFunction) = _hillTerrain[hillId];
            var hillRay = Vector3.zero;

            if (pointOnHill.x < hill.A.x)
            {
                hillRay.x = hill.A.x - inrunFlatLength - pointOnHill.x;
                hillRay.y = inrunTerrainFunction.EvalAbs(hill.A.x).y;
                hillRay.z = hill.b1 / 2;
            }
            else if (pointOnHill.x < hill.T.x)
            {
                hillRay.y = inrunTerrainFunction.EvalAbs(pointOnHill.x).y;
                hillRay.z = Mathf.Lerp(hill.b2, hill.b1, pointOnHill.x / hill.A.x) / 2;
            }
            else if (hill.T.x <= pointOnHill.x && pointOnHill.x <= hill.U.x)
            {
                hillRay.y = landingHillTerrainFunction.EvalAbs(pointOnHill.x).y;
                hillRay.z = pointOnHill.x <= hill.K.x
                    ? hill.b2 / 2 + pointOnHill.x / hill.K.x * ((hill.bK - hill.b2) / 2)
                    : pointOnHill.x >= hill.U.x
                        ? hill.bU / 2
                        : hill.bK / 2 + (pointOnHill.x - hill.K.x) / (hill.U.x - hill.K.x) *
                        ((hill.bU - hill.bK) / 2);
            }
            else if (pointOnHill.x <= hill.U.x + hill.a)
            {
                hillRay.y = landingHillTerrainFunction.EvalAbs(hill.U.x).y;
                hillRay.z = hill.bU / 2;
            }
            else
            {
                hillRay.x = pointOnHill.x - hill.U.x - hill.a;
                hillRay.y = landingHillTerrainFunction.EvalAbs(hill.U.x).y;
                hillRay.z = hill.bU / 2;
            }

            var position = hillTransform.position;
            hillRay.y += position.y - hillYOffset;
            return hillRay;
        }

        // public void GenerateTerrainFromDataPoints()
        // {
        //     _dataPoints = GetComponentsInChildren<NodePoint>();
        //
        //     foreach (var terrain in terrains)
        //     {
        //         var terrainData = terrain.terrainData;
        //         var terrainPosition = terrain.transform.position;
        //         var scl = terrainData.size / (terrainData.heightmapResolution - 1);
        //         var heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        //         for (var i = 0; i < terrainData.heightmapResolution; i++)
        //         {
        //             for (var j = 0; j < terrainData.heightmapResolution; j++)
        //             {
        //                 var coords = terrainPosition + new Vector3(j * scl.x, 0, i * scl.z);
        //
        //                 heights[i, j] = (GetInterpolatedHeight(coords) - terrainPosition.y, ) / terrainData.size.y;
        //             }
        //         }
        //
        //         terrainData.SetHeights(0, 0, heights);
        //     }
        // }

        public float GetInterpolatedHeight(Vector3 point, float baseHeight)
        {
            var heightsSum = baseHeight;
            var weightsSum = 1f;

            foreach (var node in _dataPoints)
            {
                var p = new Vector3(node.posX, node.posY, node.posZ);
                var ray = point - p;
                ray.y = 0;
                var dist = Mathf.Pow(ray.magnitude, power);
                if (dist > Eps)
                {
                    var weight = node.weight * 1f / dist;
                    heightsSum += p.y * weight;
                    weightsSum += weight;
                }
                else
                {
                    return p.y;
                }
            }

            return heightsSum / weightsSum;
        }

        public float GetInterpolatedHeight(float x, float z, float baseHeight) =>
            GetInterpolatedHeight(new Vector3(x, 0, z), baseHeight);
    }
}