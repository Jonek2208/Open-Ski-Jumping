using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Hills.CurvePaths;
using OpenSkiJumping.ScriptableObjects.Variables;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    [Serializable]
    public class GeneratedMeshData
    {
        public MaterialType material;
        public MeshFilter meshFilter;
    }

    [Serializable]
    public class RenderedPath
    {
        public string id = "";
        public List<Vector3> data;
        public float length;
    }
    
    [Serializable]
    public class RenderedPoint
    {
        public string id = "";
        public SerializableTransform value;
    }

    public class HillsMapGenerator : MonoBehaviour
    {
        private const float Eps = 0.0001f;
        [SerializeField] private HillsMapVariable hillsMapVariable;
        [SerializeField] private List<MeshScript> hillsList;
        [SerializeField] private GameObject wallsGameObject;
        [SerializeField] private List<GeneratedMeshData> generatedMeshDatas;

        [SerializeField] private List<RenderedPath> renderedPaths;
        [SerializeField] private List<Path3D> propagatedPaths;
        [SerializeField] private List<Path3D> builtInPaths;
        [SerializeField] private List<ReferencePoint> builtInRefPoints;
        [SerializeField] private List<RenderedPoint> renderedPoints;
        
        [SerializeField] private int bezierSteps = 10;


        private Dictionary<string, SerializableTransform> _propagatedPoints;


        public void RenderPaths()
        {
            propagatedPaths.Clear();
            renderedPaths.Clear();

            foreach (var path in builtInPaths)
            {
                propagatedPaths.Add(PropagatePath3D(path));
            }

            foreach (var path in hillsMapVariable.Value.paths)
            {
                propagatedPaths.Add(PropagatePath3D(path));
            }

            foreach (var path in propagatedPaths)
            {
                var curvePath = RenderCurvePath(path);
                var length = curvePath.Length;
                var segments = Mathf.RoundToInt(length);
                var renderedPath = new RenderedPath {id = path.id, length = length, data = new List<Vector3>()};

                for (var i = 0; i <= segments; i++)
                {
                    renderedPath.data.Add(curvePath.Eval((float) i / segments));
                }

                var globalTransform = PropagateRefPoint(path.refPoint);

                for (var i = 0; i < renderedPath.data.Count; i++)
                {
                    renderedPath.data[i] = HillsGeneratorUtils.Transformation(globalTransform, renderedPath.data[i]);
                }

                renderedPaths.Add(renderedPath);
            }
        }

        public static IEnumerable<(string, string)> GetRefDepsFromPoint(ReferencePoint item)
        {
            var lastId = item.id;
            if (!string.IsNullOrEmpty(item.referenceId))
            {
                yield return (item.referenceId, item.id);
                lastId = item.referenceId;
            }

            if (item.auxiliaryRefs == null) yield break;
            foreach (var it in item.auxiliaryRefs)
            {
                if (string.IsNullOrEmpty(it.referenceId)) continue;
                yield return (it.referenceId, lastId);
                lastId = it.referenceId;
            }
        }

        public static IEnumerable<(string, string)> GetRefDependencies(IEnumerable<ReferencePoint> lst)
        {
            return lst.SelectMany(GetRefDepsFromPoint);
        }

        public void PropagateReferencePoints()
        {
            _propagatedPoints = new Dictionary<string, SerializableTransform>();
            var hillMap = hillsMapVariable.Value;
            var tmp = builtInRefPoints.Concat(hillMap.referencePoints).ToDictionary(it => it.id, it => it);
            var sorted = Digraph.TopologicalSort(tmp.Keys, GetRefDependencies(tmp.Values));
            
            var set = new HashSet<string>(sorted);
            sorted.AddRange(tmp.Keys.Where(id => !string.IsNullOrEmpty(id) && !set.Contains(id)));

            foreach (var id in sorted)
            {
                var point = tmp[id];
                _propagatedPoints[point.id] = PropagateRefPoint(point);
            }
        }

        public void AddNode(CurvePath curvePath, PathNode node, SerializableTransform inheritedTransform)
        {
            var p1 = PropagateRefPoint(node.target, inheritedTransform);
            var c0 = PropagateRefPoint(node.c0, inheritedTransform);
            var c1 = PropagateRefPoint(node.c1, inheritedTransform);

            switch (node.nodeType)
            {
                case NodeType.Line:
                    curvePath.AddLine(p1.position);
                    break;
                case NodeType.Arc:
                    curvePath.AddArc(p1.position, c0.position);
                    break;
                case NodeType.Bezier3:
                    curvePath.AddBezier3(c0.position, c1.position, p1.position);
                    break;
                case NodeType.Bezier2:
                    curvePath.AddBezier2(c0.position, p1.position);
                    break;
                case NodeType.Path:
                    var pathVal = propagatedPaths.Find(it => it.id == node.pathId);
                    foreach (var nd in pathVal.data) AddNode(curvePath, nd, p1);
                    break;
            }
        }

        public void AddNode(CurvePath curvePath, PathNode node) =>
            AddNode(curvePath, node, SerializableTransform.Identity);

        public Path3D PropagatePath3D(Path3D path)
        {
            var res = new Path3D
                {id = string.Copy(path.id), refPoint = path.refPoint.Clone(), data = new List<PathNode>()};

            for (var i = 0; i < path.data.Count; i++)
            {
                var node = path.data[i].Clone();
                node.target.value = PropagateRefPoint(node.target);
                node.target.referenceId = "";
                node.c0.value = PropagateRefPoint(node.c0);
                node.c0.referenceId = "";
                node.c1.value = PropagateRefPoint(node.c1);
                node.c1.referenceId = "";
                res.data.Add(node);
            }

            return res;
        }

        public CurvePath RenderCurvePath(Path3D path)
        {
            var curvePath = new CurvePath();
            foreach (var node in path.data)
                AddNode(curvePath, node);

            return curvePath;
        }

        private SerializableTransform GetPropagatedRefPoint(string id)
        {
            return string.IsNullOrEmpty(id) ? SerializableTransform.Identity : _propagatedPoints[id];
        }

        public SerializableTransform PropagateRefPoint(ReferencePoint point)
        {
            var referencePoint = GetPropagatedRefPoint(point.referenceId);
            var result = HillsGeneratorUtils.CalculatePoint(point.value, referencePoint);
            foreach (var pt in point.auxiliaryRefs)
            {
                referencePoint = PropagateRefPoint(pt);
                result = HillsGeneratorUtils.CalculatePoint(result, referencePoint);
            }

            return result;
        }

        public SerializableTransform PropagateRefPoint(ReferencePoint point, SerializableTransform globalTransform)
        {
            var calculatedTransform = PropagateRefPoint(point);
            return HillsGeneratorUtils.CalculatePoint(calculatedTransform, globalTransform);
        }

        public IEnumerable<Mesh> GenerateConstruction(Construction wall)
        {
            var centerPath = renderedPaths.FindLast(it => it.id == wall.centerPath.id);
            if (centerPath == null) yield break;

            var xLen = Mathf.Min(wall.t1 - wall.t0, wall.length);
            var tmp = Mathf.Round((wall.t1 - wall.t0) * 100000 / xLen) / 100000;
            var floorToInt = Mathf.FloorToInt(tmp);
            var realCount = Mathf.Min(floorToInt, wall.count);
            var step = realCount <= 1 ? 0 : (wall.t1 - wall.t0 - xLen) / (realCount - 1);

            var topLeftTransform = PropagateRefPoint(wall.topLeftPath.refPoint, SerializableTransform.Identity)
                .position;
            var topRightTransform = PropagateRefPoint(wall.topRightPath.refPoint, SerializableTransform.Identity)
                .position;
            var bottomLeftTransform =
                PropagateRefPoint(wall.bottomLeftPath.refPoint, SerializableTransform.Identity).position;
            var bottomRightTransform =
                PropagateRefPoint(wall.bottomRightPath.refPoint, SerializableTransform.Identity).position;

            var topLeftPath = renderedPaths.FindLast(it => it.id == wall.topLeftPath.id);
            var topRightPath = renderedPaths.FindLast(it => it.id == wall.topRightPath.id);
            var bottomLeftPath = renderedPaths.FindLast(it => it.id == wall.bottomLeftPath.id);
            var bottomRightPath = renderedPaths.FindLast(it => it.id == wall.bottomRightPath.id);

            var topLeftOffset = OffsetFunction.FromRenderedPath(topLeftPath);
            var topRightOffset = OffsetFunction.FromRenderedPath(topRightPath);
            var bottomLeftOffset = OffsetFunction.FromRenderedPath(bottomLeftPath);
            var bottomRightOffset = OffsetFunction.FromRenderedPath(bottomRightPath);

            for (var c = 0; c < realCount; c++)
            {
                var t0 = c * step + wall.t0;
                var t1 = c * step + xLen + wall.t0;

                var points = HillsGeneratorUtils.PathStartEndWithArgs(centerPath, t0, t1, out var args);
                var n = points.Count;

                var segments = HillsGeneratorUtils.GetSegments(points).ToArray();
                var normals1 = new Vector3[n - 1];
                var normals2 = new Vector3[n - 1];

                for (var i = 0; i < n - 1; i++)
                {
                    normals2[i] = Vector3.Cross(segments[i], Vector3.up).normalized;
                    normals1[i] = Vector3.Cross(normals2[i], segments[i]).normalized;
                }

                var shifts1 = new Vector3[n];
                var shifts2 = new Vector3[n];
                shifts1[0] = normals1[0];
                shifts2[0] = normals2[0];
                shifts1[n - 1] = normals1[n - 2];
                shifts2[n - 1] = normals2[n - 2];
                for (var i = 1; i < n - 1; i++)
                {
                    var v = normals1[i - 1] + normals1[i];
                    shifts1[i] = v * Vector3.Dot(normals1[i], normals1[i]) / Vector3.Dot(v, normals1[i]);

                    v = normals2[i - 1] + normals2[i];
                    shifts2[i] = v * Vector3.Dot(normals2[i], normals2[i]) / Vector3.Dot(v, normals2[i]);
                }


                var topLeftPos = new List<Vector3>();
                var topRightPos = new List<Vector3>();
                var bottomLeftPos = new List<Vector3>();
                var bottomRightPos = new List<Vector3>();

                for (var i = 0; i < n; i++)
                {
                    var tl = topLeftOffset.EvalNorm(args[i]) + topLeftTransform;
                    var tr = topRightOffset.EvalNorm(args[i]) + topRightTransform;
                    var bl = bottomLeftOffset.EvalNorm(args[i]) + bottomLeftTransform;
                    var br = bottomRightOffset.EvalNorm(args[i]) + bottomRightTransform;
                    topLeftPos.Add(points[i] + shifts1[i] * tl.y + shifts2[i] * tl.z);
                    bottomLeftPos.Add(points[i] + shifts1[i] * bl.y + shifts2[i] * bl.z);
                    topRightPos.Add(points[i] + shifts1[i] * tr.y + shifts2[i] * tr.z);
                    bottomRightPos.Add(points[i] + shifts1[i] * br.y + shifts2[i] * br.z);
                }

                var globalTransform = PropagateRefPoint(wall.centerPath.refPoint, SerializableTransform.Identity);

                for (var i = 0; i < n; i++)
                {
                    topLeftPos[i] = HillsGeneratorUtils.Transformation(globalTransform, topLeftPos[i]);
                    topRightPos[i] = HillsGeneratorUtils.Transformation(globalTransform, topRightPos[i]);
                    bottomLeftPos[i] = HillsGeneratorUtils.Transformation(globalTransform, bottomLeftPos[i]);
                    bottomRightPos[i] = HillsGeneratorUtils.Transformation(globalTransform, bottomRightPos[i]);
                }

                yield return MeshFunctions.GeneratePathMesh(topLeftPos, topRightPos, bottomLeftPos, bottomRightPos);
            }
        }

        public IEnumerable<Mesh> GenerateStairs(Stairs wall)
        {
            var centerPath = renderedPaths.FindLast(it => it.id == wall.centerPath.id);
            var totalPathLength = centerPath.length * (wall.t1 - wall.t0);
            var step = wall.stepLength / centerPath.length;
            var tmp = Mathf.Round(100000f * totalPathLength / wall.stepLength) / 100000;
            var realCount = Mathf.FloorToInt(tmp);
            var leftTransform = PropagateRefPoint(wall.leftPath.refPoint, SerializableTransform.Identity).position;
            var rightTransform = PropagateRefPoint(wall.rightPath.refPoint, SerializableTransform.Identity).position;

            var leftPath = renderedPaths.FindLast(it => it.id == wall.leftPath.id);
            var rightPath = renderedPaths.FindLast(it => it.id == wall.rightPath.id);

            var leftOffset = OffsetFunction.FromRenderedPath(leftPath);
            var rightOffset = OffsetFunction.FromRenderedPath(rightPath);


            var points = HillsGeneratorUtils.PathStartEndWithArgs(centerPath, wall.t0, wall.t1, out var args);

            var n = points.Count;

            var segments = HillsGeneratorUtils.GetSegments(points).ToArray();
            var normals = new Vector3[n - 1];

            for (var i = 0; i < n - 1; i++)
            {
                normals[i] = Vector3.Cross(segments[i], Vector3.up).normalized;
            }

            var shifts2 = new Vector3[n];
            shifts2[0] = normals[0];
            shifts2[n - 1] = normals[n - 2];
            for (var i = 1; i < n - 1; i++)
            {
                var v = normals[i - 1] + normals[i];
                shifts2[i] = v * Vector3.Dot(normals[i], normals[i]) / Vector3.Dot(v, normals[i]);
            }

            var leftPos = new List<Vector3>();
            var rightPos = new List<Vector3>();

            for (var i = 0; i < n; i++)
            {
                var l = leftOffset.EvalNorm(args[i]) + leftTransform;
                var r = rightOffset.EvalNorm(args[i]) + rightTransform;
                leftPos.Add(points[i] + shifts2[i] * l.z);
                rightPos.Add(points[i] + shifts2[i] * r.z);
            }

            var centerFinal = new List<Vector3>();
            var leftFinal = new List<Vector3>();
            var rightFinal = new List<Vector3>();

            for (var c = 0; c < realCount + 1; c++)
            {
                var t0 = c * step + wall.t0;
                var tApprox = Mathf.Round(100000 * (n - 1) * t0) / 100000;
                var startPoint = Mathf.CeilToInt(tApprox);
                var startVal = tApprox + 1 - startPoint;

                if (!Mathf.Approximately(startVal, 1))
                {
                    // if (startPoint == 0 || startPoint > n - 1) Debug.Log("VAR");
                    centerFinal.Add(Vector3.Lerp(points[startPoint - 1], points[startPoint], startVal));
                    leftFinal.Add(Vector3.Lerp(leftPos[startPoint - 1], leftPos[startPoint], startVal));
                    rightFinal.Add(Vector3.Lerp(rightPos[startPoint - 1], rightPos[startPoint], startVal));
                }
                else
                {
                    centerFinal.Add(points[startPoint]);
                    leftFinal.Add(leftPos[startPoint]);
                    rightFinal.Add(rightPos[startPoint]);
                }
            }

            var globalTransform = PropagateRefPoint(wall.centerPath.refPoint, SerializableTransform.Identity);

            for (var i = 0; i < n; i++)
            {
                centerFinal[i] = HillsGeneratorUtils.Transformation(globalTransform, centerFinal[i]);
                leftFinal[i] = HillsGeneratorUtils.Transformation(globalTransform, leftFinal[i]);
                rightFinal[i] = HillsGeneratorUtils.Transformation(globalTransform, rightFinal[i]);
            }

            yield return MeshFunctions.GenerateStairs(centerFinal, leftFinal, rightFinal, shifts2);
        }

        public IEnumerable<Mesh> GenerateWall(Wall wall)
        {
            var xLen = Mathf.Min((wall.t1 - wall.t0), wall.length);
            var realCount = Mathf.Min(Mathf.FloorToInt((wall.t1 - wall.t0) / xLen), wall.count);
            var step = realCount <= 1 ? 0 : (wall.t1 - wall.t0 - xLen) / (realCount - 1);
            var centerPath = renderedPaths.FindLast(it => it.id == wall.centerPathId);

            for (var c = 0; c < realCount; c++)
            {
                var t0 = c * step + wall.t0;
                var t1 = c * step + xLen + wall.t0;
                var points = HillsGeneratorUtils.PathStartEnd(centerPath, t0, t1);
                var n = points.Count;

                var segments = new Vector3[n - 1];
                for (var i = 0; i < n - 1; i++)
                    segments[i] = points[i + 1] - points[i];
                var normals1 = new Vector3[n - 1];
                var normals2 = new Vector3[n - 1];

                for (var i = 0; i < n - 1; i++)
                {
                    normals2[i] = Vector3.Cross(segments[i], Vector3.up).normalized;
                    normals1[i] = Vector3.Cross(normals2[i], segments[i]).normalized;
                }

                var shifts1 = new Vector3[n];
                var shifts2 = new Vector3[n];
                shifts1[0] = normals1[0];
                shifts2[0] = normals2[0];
                shifts1[n - 1] = normals1[n - 2];
                shifts2[n - 1] = normals2[n - 2];
                for (var i = 1; i < n - 1; i++)
                {
                    var v = normals1[i - 1] + normals1[i];
                    shifts1[i] = v * Vector3.Dot(normals1[i], normals1[i]) / Vector3.Dot(v, normals1[i]);

                    v = normals2[i - 1] + normals2[i];
                    shifts2[i] = v * Vector3.Dot(normals2[i], normals2[i]) / Vector3.Dot(v, normals2[i]);
                }


                var topLeftPos = new List<Vector3>();
                var topRightPos = new List<Vector3>();
                var bottomLeftPos = new List<Vector3>();
                var bottomRightPos = new List<Vector3>();

                for (var i = 0; i < n; i++)
                {
                    topLeftPos.Add(points[i] + shifts1[i] * wall.h1 + shifts2[i] * wall.w1);
                    bottomLeftPos.Add(points[i] - shifts1[i] * wall.h2 + shifts2[i] * wall.w1);
                    topRightPos.Add(points[i] + shifts1[i] * wall.h1 - shifts2[i] * wall.w2);
                    bottomRightPos.Add(points[i] - shifts1[i] * wall.h2 - shifts2[i] * wall.w2);
                }

                var globalTransform = PropagateRefPoint(wall.refPoint, SerializableTransform.Identity);

                for (var i = 0; i < n; i++)
                {
                    topLeftPos[i] = HillsGeneratorUtils.Transformation(globalTransform, topLeftPos[i]);
                    topRightPos[i] = HillsGeneratorUtils.Transformation(globalTransform, topRightPos[i]);
                    bottomLeftPos[i] = HillsGeneratorUtils.Transformation(globalTransform, bottomLeftPos[i]);
                    bottomRightPos[i] = HillsGeneratorUtils.Transformation(globalTransform, bottomRightPos[i]);
                }

                yield return MeshFunctions.GeneratePathMesh(topLeftPos, topRightPos, bottomLeftPos, bottomRightPos);
            }
        }

        public void GenerateWalls()
        {
            var hillMap = hillsMapVariable.Value;
            var meshDict = generatedMeshDatas.ToDictionary(it => it.material, it => new List<Mesh>());

            // foreach (var item in hillMap.walls)
            // {
            //     meshDict[item.material].AddRange(GenerateWall(item));
            // }

            foreach (var item in hillMap.constructions)
            {
                // Debug.Log(item.material);
                meshDict[item.material].AddRange(GenerateConstruction(item));
            }

            foreach (var item in hillMap.stairs)
            {
                meshDict[item.material].AddRange(GenerateStairs(item));
            }

            foreach (var item in generatedMeshDatas)
            {
                item.meshFilter.sharedMesh = MeshFunctions.MergeMeshes(meshDict[item.material]);
            }
        }

        public void GenerateHills()
        {
            builtInRefPoints.Clear();
            builtInPaths.Clear();
            propagatedPaths.Clear();
            renderedPaths.Clear();


            CalculateHillsTransforms();
            PropagateReferencePoints();
            renderedPoints = _propagatedPoints.Select(it => new RenderedPoint { id = it.Key, value = it.Value}).ToList();

            for (var i = 0; i < Mathf.Min(hillsList.Count, hillsMapVariable.Value.profiles.Count); i++)
            {
                var hill = hillsList[i];
                hill.gameObject.SetActive(true);
                var profile = hillsMapVariable.Value.profiles[i];
                hill.profileData.Value = profile.profileData;
                hill.GenerateMesh();
                var hillTransform = hill.transform;
                var hillName = hillsList[i].profileData.Value.name;

                var hillOrigin = GetPropagatedRefPoint($"{hillName}/origin");

                hillTransform.position = hillOrigin.position;
                hillTransform.rotation = hillOrigin.rotation;
            }

            for (var i = hillsMapVariable.Value.profiles.Count; i < hillsList.Count; i++)
            {
                hillsList[i].gameObject.SetActive(false);
            }

            RenderPaths();
            GenerateWalls();
        }

        private void CalculateHillsTransforms()
        {
            for (var i = 0; i < Mathf.Min(hillsList.Count, hillsMapVariable.Value.profiles.Count); i++)
            {
                var hill = hillsList[i];
                hill.gameObject.SetActive(true);
                var profile = hillsMapVariable.Value.profiles[i];
                hill.profileData.Value = profile.profileData;
                hill.hill.SetValues(profile.profileData);
                var hillName = hillsList[i].profileData.Value.name;


                builtInRefPoints.AddRange(HillsGeneratorUtils.GenerateHillReferencePoints(hillsList[i].hill, hillName));
                builtInPaths.AddRange(HillsGeneratorUtils.GenerateHillPaths(hillsList[i].hill, hillName));
                builtInRefPoints.Add(new ReferencePoint
                {
                    id = $"{hillName}/origin",
                    value = hillsMapVariable.Value.profiles[i].anchor.value,
                    referenceId = hillsMapVariable.Value.profiles[i].anchor.referenceId,
                    auxiliaryRefs = new List<ReferencePoint>
                    {
                        new ReferencePoint
                        {
                            value = SerializableTransform.Minus,
                        },
                        new ReferencePoint
                        {
                            value = hillsMapVariable.Value.profiles[i].refTransform.value,
                            referenceId = hillsMapVariable.Value.profiles[i].refTransform.referenceId
                        }
                    }
                });
            }
        }

        public OffsetFunction GetOffsetFromPath(string pathId)
        {
            var path = renderedPaths.FindLast(it => it.id == pathId);
            return OffsetFunction.FromRenderedPath(path);
        }
    }
}