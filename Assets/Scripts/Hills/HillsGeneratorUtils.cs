using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    public static class HillsGeneratorUtils
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
        
        public static IEnumerable<ReferencePoint> GenerateHillReferencePoints(Hill hill, string hillId)
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

            var pointsList = new List<(string, Vector3)>
            {
                ($"{hillId}/A", hill.A),
                ($"{hillId}/B", hill.B),
                ($"{hillId}/E1", hill.E1),
                ($"{hillId}/E2", hill.E2),
                ($"{hillId}/T", hill.T),
                
                ($"{hillId}/F", hill.F),
                ($"{hillId}/P", hill.P),
                ($"{hillId}/K", hill.K),
                ($"{hillId}/L", hill.L),
                ($"{hillId}/U", hill.U),
                
                ($"{hillId}/C1", hill.C1),
                ($"{hillId}/E1C", hillE1C),
                ($"{hillId}/E2C", hillE2C),
                
                ($"{hillId}/FC", hillFC),
                ($"{hillId}/PC", hillPC),
                ($"{hillId}/CL", hill.CL),
                ($"{hillId}/C2", hill.C2),
                ($"{hillId}/UC", hillUC),
                
                ($"{hillId}/JT", hillJT),
            };

            foreach (var (id, val) in pointsList)
            {
                yield return ReferencePoint.FromPos(id, val, "");
                yield return ReferencePoint.FromPos($"{id}/global", val, $"{hillId}/origin");
            }
        }

        public static IEnumerable<Path3D> GenerateHillPaths(Hill hill, string hillId)
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
    }
}