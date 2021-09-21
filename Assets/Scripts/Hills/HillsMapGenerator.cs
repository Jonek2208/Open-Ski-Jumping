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

    public class OffsetFunction
    {
        private readonly List<Vector3> _points;
        private readonly int _n;
        private readonly float _minX, _maxX;

        public OffsetFunction(IEnumerable<Vector3> points)
        {
            _points = ProjectPathToFunction(points).ToList();
            _n = _points.Count;
            _minX = _points[0].x;
            _maxX = _points[_n - 1].x;
        }

        public static OffsetFunction FromRenderedPath(RenderedPath renderedPath)
        {
            return renderedPath == null ? Zero : new OffsetFunction(renderedPath.data);
        }

        public static OffsetFunction Zero => new OffsetFunction(new[] {Vector3.zero, Vector3.right});

        public static IEnumerable<Vector3> ProjectPathToFunction(IEnumerable<Vector3> points)
        {
            float? minVal = null;
            foreach (var p in points)
            {
                if (minVal.HasValue && p.x <= minVal) continue;
                minVal = p.x;
                yield return p;
            }
        }

        public Vector3 EvalAbs(float t)
        {
            if (t < _minX) return _points[0];
            if (t > _maxX) return _points[_n - 1];

            var l = 1;
            var r = _n - 1;
            var i = 1;
            while (l <= r)
            {
                var mid = (l + r) / 2;
                if (_points[mid].x < t)
                {
                    l = mid + 1;
                }
                else if (_points[mid].x > t)
                {
                    r = mid - 1;
                    i = mid;
                }
                else
                {
                    return _points[mid];
                }
            }

            var lo = _points[i - 1].x;
            var hi = _points[i].x;
            var u = (t - lo) / (hi - lo);
            return Vector3.Lerp(_points[i - 1], _points[i], u);
        }

        public Vector3 EvalNorm(float t)
        {
            return EvalAbs(Mathf.Lerp(_minX, _maxX, t));
        }
    }

    public static class PathUtils
    {
        public static SerializableTransform CalculatePoint(SerializableTransform a, SerializableTransform rf)
        {
            return new SerializableTransform
            {
                position = rf.position + rf.rotation * Vector3.Scale(rf.scale, a.position),
                rotation = rf.rotation * a.rotation,
                scale = Vector3.Scale(rf.scale, a.scale)
            };
        }

        public static List<Vector3> PathStartEnd(RenderedPath path, float t0, float t1)
        {
            var n = path.data.Count - 1;
            var startPoint = Mathf.CeilToInt(n * t0);
            var startVal = n * t0 + 1 - startPoint;
            var endPoint = Mathf.CeilToInt(n * t1);
            var endVal = n * t1 + 1 - endPoint;
            var res = new List<Vector3>();

            if (!Mathf.Approximately(startVal, 1))
            {
                res.Add(Vector3.Lerp(path.data[startPoint - 1], path.data[startPoint], startVal));
            }

            for (var i = startPoint; i < endPoint; i++)
            {
                res.Add(path.data[i]);
            }

            if (startPoint <= endPoint)
                res.Add(Vector3.Lerp(path.data[endPoint - 1], path.data[endPoint], endVal));


            return res;
        }

        public static List<Vector3> PathStartEndWithArgs(RenderedPath path, float t0, float t1,
            out List<float> args)
        {
            var n = path.data.Count - 1;
            var startPoint = Mathf.CeilToInt(n * t0);
            var startVal = n * t0 + 1 - startPoint;
            var endPoint = Mathf.CeilToInt(n * t1);
            var endVal = n * t1 + 1 - endPoint;
            var res = new List<Vector3>();
            args = new List<float>();

            if (!Mathf.Approximately(startVal, 1))
            {
                res.Add(Vector3.Lerp(path.data[startPoint - 1], path.data[startPoint], startVal));
                args.Add(t0);
            }

            for (var i = startPoint; i < endPoint; i++)
            {
                res.Add(path.data[i]);
                args.Add((float) i / n);
            }

            if (startPoint <= endPoint)
            {
                res.Add(Vector3.Lerp(path.data[endPoint - 1], path.data[endPoint], endVal));
                args.Add(t1);
            }

            return res;
        }

        public static IEnumerable<Vector3> GetSegments(IReadOnlyList<Vector3> points)
        {
            var n = points.Count;
            for (var i = 0; i < n - 1; i++)
                yield return points[i + 1] - points[i];
        }

        public static Vector3 Transformation(SerializableTransform transform, Vector3 point)
        {
            return transform.rotation * Vector3.Scale(transform.scale, point) + transform.position;
        }
    }

    [Serializable]
    public class RenderedPath
    {
        public string id = "";
        public List<Vector3> data;
        public float length;
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
        [SerializeField] private int bezierSteps = 10;


        private readonly Dictionary<string, SerializableTransform> _propagatedPoints =
            new Dictionary<string, SerializableTransform>();

        public IEnumerable<ReferencePoint> GenerateHillReferencePoints(Hill hill, string hillId)
        {
            // var hillsMap = hillsMapVariable.Value;
            // var refPoints = hillsMap.referencePoints;
            // refPoints.Clear();
            // var hill = hillsList[0].hill;

            //inrun curve
            var tangentE1 = new Vector2(Mathf.Cos(hill.gammaR), -Mathf.Sin(hill.gammaR));
            var tangentE2 = new Vector2(Mathf.Cos(hill.alphaR), -Mathf.Sin(hill.alphaR));
            var pointingVec = hill.E2 - hill.E1;
            var dist = Vector2.Dot(tangentE1, pointingVec);
            var hillE1C = hill.E1 + tangentE1 * dist / 3f;
            var hillE2C = hill.E2 - tangentE2 * dist / Vector2.Dot(tangentE1, tangentE2) / 3f;

            //knoll
            var tangentF = new Vector2(1, -Mathf.Tan(hill.beta0R));
            var tangentP = new Vector2(1, -Mathf.Tan(hill.betaPR));
            dist = hill.P.x - hill.F.x;
            var hillFC = hill.F + tangentF * dist / 3f;
            var hillPC = hill.P - tangentP * dist / 3f;

            //landing hill curve

            var tangentL = new Vector2(Mathf.Cos(hill.betaLR), -Mathf.Sin(hill.betaLR));
            var normalU = new Vector2(Mathf.Sin(0), Mathf.Cos(0));
            var t = Vector2.Dot(hill.U - hill.L, normalU) / Vector2.Dot(tangentL, normalU);
            var hillUC = hill.L + t * tangentL;

            //judges tower

            var hillJT = new Vector3(hill.d, hill.g + hill.LandingArea(hill.d), hill.q);


            yield return ReferencePoint.FromPos($"{hillId}/A", hill.A, "");
            yield return ReferencePoint.FromPos($"{hillId}/B", hill.B, "");
            yield return ReferencePoint.FromPos($"{hillId}/E1", hill.E1, "");
            yield return ReferencePoint.FromPos($"{hillId}/E2", hill.E2, "");
            yield return ReferencePoint.FromPos($"{hillId}/T", hill.T, "");

            yield return ReferencePoint.FromPos($"{hillId}/F", hill.F, "");
            yield return ReferencePoint.FromPos($"{hillId}/P", hill.P, "");
            yield return ReferencePoint.FromPos($"{hillId}/K", hill.K, "");
            yield return ReferencePoint.FromPos($"{hillId}/L", hill.L, "");
            yield return ReferencePoint.FromPos($"{hillId}/U", hill.U, "");


            yield return ReferencePoint.FromPos($"{hillId}/C1", hill.C1, "");
            yield return ReferencePoint.FromPos($"{hillId}/E1C", hillE1C, "");
            yield return ReferencePoint.FromPos($"{hillId}/E2C", hillE2C, "");

            yield return ReferencePoint.FromPos($"{hillId}/FC", hillFC, "");
            yield return ReferencePoint.FromPos($"{hillId}/PC", hillPC, "");
            yield return ReferencePoint.FromPos($"{hillId}/CL", hill.CL, "");
            yield return ReferencePoint.FromPos($"{hillId}/C2", hill.C2, "");
            yield return ReferencePoint.FromPos($"{hillId}/UC", hillUC, "");

            yield return ReferencePoint.FromPos($"{hillId}/JT", hillJT, "");
        }

        public IEnumerable<Path3D> GenerateHillPaths(Hill hill, string hillId)
        {
            yield return new Path3D
            {
                id = $"{hillId}/inrun",
                refPoint = ReferencePoint.FromRefId($"{hillId}/origin"),
                data = new List<PathNode>
                {
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/A")),
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/B")),
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/E1")),
                    PathNode.Bezier3(ReferencePoint.FromRefId($"{hillId}/E2"),
                        ReferencePoint.FromRefId($"{hillId}/E1C"), ReferencePoint.FromRefId($"{hillId}/E2C")),
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/T"))
                }
            };

            yield return new Path3D
            {
                id = $"{hillId}/landing-hill",
                refPoint = ReferencePoint.FromRefId($"{hillId}/origin"),
                data = new List<PathNode>
                {
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/F")),
                    PathNode.Bezier3(ReferencePoint.FromRefId($"{hillId}/P"), ReferencePoint.FromRefId($"{hillId}/FC"),
                        ReferencePoint.FromRefId($"{hillId}/PC")),
                    PathNode.Arc(ReferencePoint.FromRefId($"{hillId}/L"), ReferencePoint.FromRefId($"{hillId}/CL")),
                    PathNode.Bezier2(ReferencePoint.FromRefId($"{hillId}/U"), ReferencePoint.FromRefId($"{hillId}/UC"))
                }
            };

            yield return new Path3D
            {
                id = $"{hillId}/inrun-x",
                refPoint = ReferencePoint.FromPosRotScale("", Vector3.zero, Quaternion.identity, Vector3.right,
                    $"{hillId}/origin"),
                data = new List<PathNode>
                {
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/A")),
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/T"))
                }
            };

            yield return new Path3D
            {
                id = $"{hillId}/landing-hill-x",
                refPoint = ReferencePoint.FromPosRotScale("", Vector3.zero, Quaternion.identity, Vector3.right,
                    $"{hillId}/origin"),
                data = new List<PathNode>
                {
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/F")),
                    PathNode.Line(ReferencePoint.FromRefId($"{hillId}/U"))
                }
            };

            yield return new Path3D
            {
                id = $"{hillId}/inrun-terrain",
                refPoint = ReferencePoint.Neutral,
                data = new List<PathNode>
                {
                    new PathNode
                    {
                        nodeType = NodeType.Path, pathId = $"{hillId}/inrun",
                        target = ReferencePoint.FromPosRotScale("", Vector3.down * hill.s, Quaternion.identity,
                            new Vector3(1, hill.terrainSteepness, 1))
                    }
                }
            };

            yield return new Path3D
            {
                id = $"{hillId}/landing-hill-terrain",
                refPoint = ReferencePoint.Neutral,
                data = new List<PathNode>
                {
                    new PathNode
                    {
                        nodeType = NodeType.Path, pathId = $"{hillId}/landing-hill",
                        target = ReferencePoint.FromPosRotScale("", Vector3.zero, Quaternion.identity,
                            Vector3.one)
                    }
                }
            };
        }

        public void RenderPaths()
        {
            propagatedPaths.Clear();
            foreach (var path in builtInPaths)
            {
                propagatedPaths.Add(PropagatePath3D(path));
            }

            foreach (var path in hillsMapVariable.Value.paths)
            {
                propagatedPaths.Add(PropagatePath3D(path));
            }

            renderedPaths.Clear();
            foreach (var path in propagatedPaths)
            {
                var curvePath = RenderCurvePath(path);
                var length = curvePath.Length;
                var segments = Mathf.RoundToInt(length);
                var renderedPath = new RenderedPath {id = path.id, length = length, data = new List<Vector3>()};
                // Debug.Log(path.id + ": " + segments);

                for (var i = 0; i <= segments; i++)
                {
                    renderedPath.data.Add(curvePath.Eval((float) i / segments));
                }

                var globalTransform = PropagateRefPoint(path.refPoint);

                for (var i = 0; i < renderedPath.data.Count; i++)
                {
                    renderedPath.data[i] = PathUtils.Transformation(globalTransform, renderedPath.data[i]);
                }

                renderedPaths.Add(renderedPath);
            }
        }

        public void PropagateReferencePoints()
        {
            //built-in ref points
            foreach (var point in builtInRefPoints)
            {
                Debug.Log($"{point.id} / {point.referenceId}");
                var referencePoint = string.IsNullOrEmpty(point.referenceId)
                    ? SerializableTransform.Identity
                    : _propagatedPoints[point.referenceId];

                var xd = PathUtils.CalculatePoint(point.value, referencePoint);
                _propagatedPoints[point.id] = xd;
            }

            //user defined ref points
            var hillMap = hillsMapVariable.Value;
            foreach (var point in hillMap.referencePoints)
            {
                var referencePoint = string.IsNullOrEmpty(point.referenceId)
                    ? SerializableTransform.Identity
                    : _propagatedPoints[point.referenceId];

                var xd = PathUtils.CalculatePoint(point.value, referencePoint);
                _propagatedPoints[point.id] = xd;
            }
        }


        // public IEnumerable<Vector3> RenderPathSegment(PathNode node, PathNode lastNode = null)
        // {
        //     if (lastNode == null || node.nodeType == NodeType.Line)
        //     {
        //         var computedNode = PropagateRefPoint(node.target);
        //         return new List<Vector3> {computedNode.GetPosition()};
        //     }
        //
        //     var p0 = PropagateRefPoint(lastNode.target).GetPosition();
        //     var p1 = PropagateRefPoint(node.target).GetPosition();
        //     var k0 = PropagateRefPoint(node.c0).GetPosition();
        //     var k1 = PropagateRefPoint(node.c1).GetPosition();
        //     var buf = new List<Vector3>();
        //
        //     for (var i = 1; i <= bezierSteps; i++)
        //     {
        //         buf.Add(BezierCurve.Bezier3(p0, p1, k0, k1, (float) i / bezierSteps));
        //     }
        //
        //     return buf;
        // }

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

        public SerializableTransform PropagateRefPoint(ReferencePoint point)
        {
            var referencePoint = string.IsNullOrEmpty(point.referenceId)
                ? SerializableTransform.Identity
                : _propagatedPoints[point.referenceId];

            return PathUtils.CalculatePoint(PathUtils.CalculatePoint(point.value, referencePoint),
                SerializableTransform.Identity);
        }

        public SerializableTransform PropagateRefPoint(ReferencePoint point,
            SerializableTransform globalTransform)
        {
            var referencePoint = string.IsNullOrEmpty(point.referenceId)
                ? SerializableTransform.Identity
                : _propagatedPoints[point.referenceId];


            return PathUtils.CalculatePoint(PathUtils.CalculatePoint(point.value, referencePoint), globalTransform);
        }

        public SerializableTransform PropagateRefPoint(ReferencePoint point, ReferencePoint globalRef)
        {
            var referencePoint = string.IsNullOrEmpty(point.referenceId)
                ? SerializableTransform.Identity
                : _propagatedPoints[point.referenceId];

            var globalTransform = PropagateRefPoint(globalRef, SerializableTransform.Identity);
            return PathUtils.CalculatePoint(PathUtils.CalculatePoint(point.value, referencePoint), globalTransform);
        }


        public IEnumerable<Mesh> GenerateConstruction(Construction wall)
        {
            var xLen = Mathf.Min(wall.t1 - wall.t0, wall.length);
            var tmp = Mathf.Round((wall.t1 - wall.t0) * 100000 / xLen) / 100000;
            var floorToInt = Mathf.FloorToInt(tmp);
            var realCount = Mathf.Min(floorToInt, wall.count);
            var step = realCount <= 1 ? 0 : (wall.t1 - wall.t0 - xLen) / (realCount - 1);

            var centerPath = renderedPaths.FindLast(it => it.id == wall.centerPathId);

            var topLeftTransform = PropagateRefPoint(wall.topLeftRefPoint, SerializableTransform.Identity).position;
            var topRightTransform = PropagateRefPoint(wall.topRightRefPoint, SerializableTransform.Identity).position;
            var bottomLeftTransform =
                PropagateRefPoint(wall.bottomLeftRefPoint, SerializableTransform.Identity).position;
            var bottomRightTransform =
                PropagateRefPoint(wall.bottomRightRefPoint, SerializableTransform.Identity).position;

            var topLeftPath = renderedPaths.FindLast(it => it.id == wall.topLeftPathId);
            var topRightPath = renderedPaths.FindLast(it => it.id == wall.topRightPathId);
            var bottomLeftPath = renderedPaths.FindLast(it => it.id == wall.bottomLeftPathId);
            var bottomRightPath = renderedPaths.FindLast(it => it.id == wall.bottomRightPathId);

            var topLeftOffset = OffsetFunction.FromRenderedPath(topLeftPath);
            var topRightOffset = OffsetFunction.FromRenderedPath(topRightPath);
            var bottomLeftOffset = OffsetFunction.FromRenderedPath(bottomLeftPath);
            var bottomRightOffset = OffsetFunction.FromRenderedPath(bottomRightPath);

            for (var c = 0; c < realCount; c++)
            {
                var t0 = c * step + wall.t0;
                var t1 = c * step + xLen + wall.t0;

                var points = PathUtils.PathStartEndWithArgs(centerPath, t0, t1, out var args);
                var n = points.Count;

                var segments = PathUtils.GetSegments(points).ToArray();
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

                var globalTransform = PropagateRefPoint(wall.centerRefPoint, SerializableTransform.Identity);

                for (var i = 0; i < n; i++)
                {
                    topLeftPos[i] = PathUtils.Transformation(globalTransform, topLeftPos[i]);
                    topRightPos[i] = PathUtils.Transformation(globalTransform, topRightPos[i]);
                    bottomLeftPos[i] = PathUtils.Transformation(globalTransform, bottomLeftPos[i]);
                    bottomRightPos[i] = PathUtils.Transformation(globalTransform, bottomRightPos[i]);
                }

                yield return MeshFunctions.GeneratePathMesh(topLeftPos, topRightPos, bottomLeftPos, bottomRightPos);
            }
        }

        public IEnumerable<Mesh> GenerateStairs(Stairs wall)
        {
            var centerPath = renderedPaths.FindLast(it => it.id == wall.centerPathId);
            var totalPathLength = centerPath.length * (wall.t1 - wall.t0);
            var step = wall.stepLength / centerPath.length;
            var tmp = Mathf.Round(100000f *totalPathLength/wall.stepLength) / 100000;
            var realCount = Mathf.FloorToInt(tmp);
            var leftTransform = PropagateRefPoint(wall.leftRefPoint, SerializableTransform.Identity).position;
            var rightTransform = PropagateRefPoint(wall.rightRefPoint, SerializableTransform.Identity).position;

            var leftPath = renderedPaths.FindLast(it => it.id == wall.leftPathId);
            var rightPath = renderedPaths.FindLast(it => it.id == wall.rightPathId);

            var leftOffset = OffsetFunction.FromRenderedPath(leftPath);
            var rightOffset = OffsetFunction.FromRenderedPath(rightPath);


            var points = PathUtils.PathStartEndWithArgs(centerPath, wall.t0, wall.t1, out var args);

            var n = points.Count;

            var segments = PathUtils.GetSegments(points).ToArray();
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
                    if (startPoint == 0 || startPoint > n - 1) Debug.Log("VAR");
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

            var globalTransform = PropagateRefPoint(wall.centerRefPoint, SerializableTransform.Identity);

            for (var i = 0; i < n; i++)
            {
                centerFinal[i] = PathUtils.Transformation(globalTransform, centerFinal[i]);
                leftFinal[i] = PathUtils.Transformation(globalTransform, leftFinal[i]);
                rightFinal[i] = PathUtils.Transformation(globalTransform, rightFinal[i]);
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
                var points = PathUtils.PathStartEnd(centerPath, t0, t1);
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
                    topLeftPos[i] = PathUtils.Transformation(globalTransform, topLeftPos[i]);
                    topRightPos[i] = PathUtils.Transformation(globalTransform, topRightPos[i]);
                    bottomLeftPos[i] = PathUtils.Transformation(globalTransform, bottomLeftPos[i]);
                    bottomRightPos[i] = PathUtils.Transformation(globalTransform, bottomRightPos[i]);
                }

                yield return MeshFunctions.GeneratePathMesh(topLeftPos, topRightPos, bottomLeftPos, bottomRightPos);
            }
        }

        public void GenerateWalls()
        {
            var hillMap = hillsMapVariable.Value;
            var meshDict = generatedMeshDatas.ToDictionary(it => it.material, it => new List<Mesh>());

            foreach (var item in hillMap.walls)
            {
                meshDict[item.material].AddRange(GenerateWall(item));
            }

            foreach (var item in hillMap.constructions)
            {
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
            for (var i = 0; i < Mathf.Min(hillsList.Count, hillsMapVariable.Value.profiles.Count); i++)
            {
                var hill = hillsList[i];
                hill.gameObject.SetActive(true);
                var profile = hillsMapVariable.Value.profiles[i];
                hill.profileData.Value = profile.profileData;
                hill.GenerateMesh();
                var hillTransform = hill.transform;
                var hillName = hillsList[i].profileData.Value.name;

                if (i > 0)
                {
                    hillTransform.rotation = Quaternion.Euler(0, profile.azimuth, 0);
                    hillTransform.position = hillsList[0].transform.position + profile.pos +
                                             (Vector3) (hillsList[0].hill.U - hill.hill.U);
                }
                else
                {
                    hillTransform.rotation = Quaternion.identity;
                    hillTransform.position = Vector3.zero;
                }


                builtInRefPoints.Add(ReferencePoint.FromTransform($"{hillName}/origin", hillTransform));
                builtInRefPoints.AddRange(GenerateHillReferencePoints(hillsList[i].hill, hillName));
                builtInPaths.AddRange(GenerateHillPaths(hillsList[i].hill, hillName));
            }

            for (var i = hillsMapVariable.Value.profiles.Count; i < hillsList.Count; i++)
            {
                hillsList[i].gameObject.SetActive(false);
            }

            PropagateReferencePoints();
            RenderPaths();
            GenerateWalls();
        }

        public OffsetFunction GetOffsetFromPath(string pathId)
        {
            var path = renderedPaths.FindLast(it => it.id == pathId);
            return OffsetFunction.FromRenderedPath(path);
        }
    }
}