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

    [Serializable]
    public class GeneratedMapData
    {
        public List<RenderedPath> renderedPaths = new();
        public List<Path3D> propagatedPaths = new();
        public List<Path3D> builtInPaths = new();
        public List<ReferencePoint> builtInRefPoints=new();
        public Dictionary<string, SerializableTransform> propagatedPoints = new();
        public List<RenderedPoint> renderedPoints = new();
    }

    public class HillsMapGenerator : MonoBehaviour
    {
        [SerializeField] private int currentHillIndex;
        [SerializeField] private HillsMapVariable hillsMapVariable;
        [SerializeField] private List<MeshScript> hillsList;
        [SerializeField] private GameObject wallsGameObject;
        [SerializeField] private List<GeneratedMeshData> generatedMeshDatas;

        [SerializeField] private GeneratedMapData generatedMapData;

        public int CurrentHillIndex
        {
            get => currentHillIndex;
            set => currentHillIndex = value;
        }

        public void RenderPaths()
        {
            generatedMapData.propagatedPaths.Clear();
            generatedMapData.renderedPaths.Clear();

            foreach (var path in generatedMapData.builtInPaths)
            {
                generatedMapData.propagatedPaths.Add(PropagatePath3D(path));
            }

            foreach (var path in hillsMapVariable.Value.paths)
            {
                generatedMapData.propagatedPaths.Add(PropagatePath3D(path));
            }

            foreach (var path in generatedMapData.propagatedPaths)
            {
                var curvePath = RenderCurvePath(path);
                var length = curvePath.Length;
                var segments = Mathf.RoundToInt(length);
                var renderedPath = new RenderedPath {id = path.id, length = length, data = new List<Vector3>()};

                for (var i = 0; i <= segments; i++)
                {
                    renderedPath.data.Add(curvePath.Eval((float) i / segments));
                }

                var globalTransform = ObjectsGenerator.PropagateRefPoint(path.refPoint, generatedMapData);

                for (var i = 0; i < renderedPath.data.Count; i++)
                {
                    renderedPath.data[i] = HillsGeneratorUtils.Transformation(globalTransform, renderedPath.data[i]);
                }

                generatedMapData.renderedPaths.Add(renderedPath);
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
            generatedMapData.propagatedPoints = new Dictionary<string, SerializableTransform>();
            var hillMap = hillsMapVariable.Value;
            var tmp = generatedMapData.builtInRefPoints.Concat(hillMap.referencePoints).ToDictionary(it => it.id, it => it);
            var sorted = Digraph.TopologicalSort(tmp.Keys, GetRefDependencies(tmp.Values));

            var set = new HashSet<string>(sorted);
            sorted.AddRange(tmp.Keys.Where(id => !string.IsNullOrEmpty(id) && !set.Contains(id)));

            foreach (var point in sorted.Select(id => tmp[id]))
            {
                generatedMapData.propagatedPoints[point.id] = ObjectsGenerator.PropagateRefPoint(point, generatedMapData);
            }
        }

        public void AddNode(CurvePath curvePath, PathNode node, SerializableTransform inheritedTransform)
        {
            var p1 = ObjectsGenerator.PropagateRefPoint(node.target, inheritedTransform, generatedMapData);
            var c0 = ObjectsGenerator.PropagateRefPoint(node.c0, inheritedTransform, generatedMapData);
            var c1 = ObjectsGenerator.PropagateRefPoint(node.c1, inheritedTransform, generatedMapData);

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
                    var pathVal = generatedMapData.propagatedPaths.Find(it => it.id == node.pathId);
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
                node.target.value = ObjectsGenerator.PropagateRefPoint(node.target, generatedMapData);
                node.target.referenceId = "";
                node.c0.value = ObjectsGenerator.PropagateRefPoint(node.c0, generatedMapData);
                node.c0.referenceId = "";
                node.c1.value = ObjectsGenerator.PropagateRefPoint(node.c1, generatedMapData);
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
        public void GenerateWalls()
        {
            var hillMap = hillsMapVariable.Value;
            var meshDict = generatedMeshDatas.ToDictionary(it => it.material, it => new List<Mesh>());

            foreach (var item in hillMap.constructions)
            {
                meshDict[item.material].AddRange(ObjectsGenerator.GenerateConstruction(item, generatedMapData));
            }

            foreach (var item in hillMap.stairs)
            {
                meshDict[item.material].AddRange(ObjectsGenerator.GenerateStairs(item, generatedMapData));
            }

            foreach (var item in generatedMeshDatas)
            {
                item.meshFilter.sharedMesh = MeshFunctions.MergeMeshes(meshDict[item.material]);
            }
        }


        public void GenerateHills()
        {
            try
            {
                transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                generatedMapData.builtInRefPoints.Clear();
                generatedMapData.builtInPaths.Clear();
                generatedMapData.propagatedPaths.Clear();
                generatedMapData.renderedPaths.Clear();


                CalculateHillsTransforms();
                PropagateReferencePoints();
                generatedMapData.renderedPoints = generatedMapData.propagatedPoints.Select(it => new RenderedPoint {id = it.Key, value = it.Value})
                    .ToList();

                for (var i = 0; i < Mathf.Min(hillsList.Count, hillsMapVariable.Value.profiles.Count); i++)
                {
                    var hill = hillsList[i];
                    hill.gameObject.SetActive(true);
                    var profile = hillsMapVariable.Value.profiles[i];
                    hill.profileData = profile.profileData;
                    hill.GenerateMesh();
                    var hillTransform = hill.transform;
                    var hillName = hillsList[i].profileData.name;

                    var hillOrigin = ObjectsGenerator.GetPropagatedRefPoint($"{hillName}/origin", generatedMapData);

                    hillTransform.position = hillOrigin.position;
                    hillTransform.rotation = hillOrigin.rotation;
                }

                for (var i = hillsMapVariable.Value.profiles.Count; i < hillsList.Count; i++)
                {
                    hillsList[i].gameObject.SetActive(false);
                }

                RenderPaths();
                GenerateWalls();
                var currentHillTransform = hillsList[currentHillIndex].transform;
                var xd = currentHillTransform.InverseTransformPoint(Vector3.zero);
                transform.position = xd;
                transform.rotation = Quaternion.Inverse(currentHillTransform.localRotation);
            }
            catch (Exception e)
            {
                Debug.Log($"Error: {e}");
            }
        }

        private void CalculateHillsTransforms()
        {
            for (var i = 0; i < Mathf.Min(hillsList.Count, hillsMapVariable.Value.profiles.Count); i++)
            {
                var hill = hillsList[i];
                hill.gameObject.SetActive(true);
                var profile = hillsMapVariable.Value.profiles[i];
                hill.profileData = profile.profileData;
                hill.hill.SetValues(profile.profileData);
                var hillName = hillsList[i].profileData.name;


                generatedMapData.builtInRefPoints.AddRange(HillsGeneratorUtils.GenerateHillReferencePoints(hillsList[i].hill, hillName));
                generatedMapData.builtInPaths.AddRange(HillsGeneratorUtils.GenerateHillPaths(hillsList[i].hill, hillName));
                generatedMapData.builtInRefPoints.Add(new ReferencePoint
                {
                    id = $"{hillName}/origin",
                    value = SerializableTransform.Minus,
                    auxiliaryRefs = new List<ReferencePoint>
                    {
                        new()
                        {
                            value = hillsMapVariable.Value.profiles[i].anchor.value,
                            referenceId = hillsMapVariable.Value.profiles[i].anchor.referenceId,
                        },
                        new()
                        {
                            value = SerializableTransform.Minus,
                        },
                        new()
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
            var path = generatedMapData.renderedPaths.FindLast(it => it.id == pathId);
            return OffsetFunction.FromRenderedPath(path);
        }
    }
}