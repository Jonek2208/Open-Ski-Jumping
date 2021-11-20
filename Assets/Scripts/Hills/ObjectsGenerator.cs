using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    public static class ObjectsGenerator
    {
        public static SerializableTransform GetPropagatedRefPoint(string id, GeneratedMapData gMD)
        {
            return string.IsNullOrEmpty(id) ? SerializableTransform.Identity : gMD.propagatedPoints[id];
        }

        public static SerializableTransform PropagateRefPoint(ReferencePoint point, GeneratedMapData gMD)
        {
            var referencePoint = GetPropagatedRefPoint(point.referenceId, gMD);
            var result = HillsGeneratorUtils.CalculatePoint(point.value, referencePoint);
            foreach (var pt in point.auxiliaryRefs)
            {
                referencePoint = PropagateRefPoint(pt, gMD);
                result = HillsGeneratorUtils.CalculatePoint(result, referencePoint);
            }

            return result;
        }

        public static SerializableTransform PropagateRefPoint(ReferencePoint point, SerializableTransform globalTransform, GeneratedMapData gMD)
        {
            var calculatedTransform = PropagateRefPoint(point, gMD);
            return HillsGeneratorUtils.CalculatePoint(calculatedTransform, globalTransform);
        }

        public static IEnumerable<Mesh> GenerateConstruction(Construction wall, GeneratedMapData gMD)
        {
            var centerPath = gMD.renderedPaths.FindLast(it => it.id == wall.centerPath.idY);
            if (centerPath == null) yield break;

            var t0Normalized = HillsGeneratorUtils.GetNormalizedValue(wall.t0, centerPath.length);
            var t1Normalized = HillsGeneratorUtils.GetNormalizedValue(wall.t1, centerPath.length);
            var lengthNormalized = HillsGeneratorUtils.GetNormalizedValue(wall.length, centerPath.length);

            var xLen = Mathf.Min(t1Normalized - t0Normalized, lengthNormalized);
            var tmp = Mathf.Round((t1Normalized - t0Normalized) * 100000 / xLen) / 100000;
            var floorToInt = Mathf.FloorToInt(tmp);
            var realCount = Mathf.Min(floorToInt, wall.count);
            var step = realCount <= 1 ? 0 : (t1Normalized - t0Normalized - xLen) / (realCount - 1);

            var topLeftTransform = PropagateRefPoint(wall.topLeftPath.refPoint, gMD).position;
            var topRightTransform = PropagateRefPoint(wall.topRightPath.refPoint, gMD).position;
            var bottomLeftTransform = PropagateRefPoint(wall.bottomLeftPath.refPoint, gMD).position;
            var bottomRightTransform = PropagateRefPoint(wall.bottomRightPath.refPoint, gMD).position;

            var topLeftOffsetY =
                OffsetFunction.FromRenderedPath(gMD.renderedPaths.FindLast(it => it.id == wall.topLeftPath.idY));
            var topRightOffsetY =
                OffsetFunction.FromRenderedPath(gMD.renderedPaths.FindLast(it => it.id == wall.topRightPath.idY));
            var bottomLeftOffsetY =
                OffsetFunction.FromRenderedPath(gMD.renderedPaths.FindLast(it => it.id == wall.bottomLeftPath.idY));
            var bottomRightOffsetY =
                OffsetFunction.FromRenderedPath(gMD.renderedPaths.FindLast(it => it.id == wall.bottomRightPath.idY));

            var topLeftOffsetZ =
                OffsetFunction.FromRenderedPath(gMD.renderedPaths.FindLast(it => it.id == wall.topLeftPath.idZ));
            var topRightOffsetZ =
                OffsetFunction.FromRenderedPath(gMD.renderedPaths.FindLast(it => it.id == wall.topRightPath.idZ));
            var bottomLeftOffsetZ =
                OffsetFunction.FromRenderedPath(gMD.renderedPaths.FindLast(it => it.id == wall.bottomLeftPath.idZ));
            var bottomRightOffsetZ =
                OffsetFunction.FromRenderedPath(gMD.renderedPaths.FindLast(it => it.id == wall.bottomRightPath.idZ));

            for (var c = 0; c < realCount; c++)
            {
                var t0 = c * step + t0Normalized;
                var t1 = c * step + xLen + t0Normalized;

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
                    var tlY = topLeftOffsetY.EvalNorm(args[i]).y + topLeftTransform.y;
                    var trY = topRightOffsetY.EvalNorm(args[i]).y + topRightTransform.y;
                    var blY = bottomLeftOffsetY.EvalNorm(args[i]).y + bottomLeftTransform.y;
                    var brY = bottomRightOffsetY.EvalNorm(args[i]).y + bottomRightTransform.y;

                    var tlZ = topLeftOffsetZ.EvalNorm(args[i]).z + topLeftTransform.z;
                    var trZ = topRightOffsetZ.EvalNorm(args[i]).z + topRightTransform.z;
                    var blZ = bottomLeftOffsetZ.EvalNorm(args[i]).z + bottomLeftTransform.z;
                    var brZ = bottomRightOffsetZ.EvalNorm(args[i]).z + bottomRightTransform.z;

                    topLeftPos.Add(points[i] + shifts1[i] * tlY + shifts2[i] * tlZ);
                    bottomLeftPos.Add(points[i] + shifts1[i] * blY + shifts2[i] * blZ);
                    topRightPos.Add(points[i] + shifts1[i] * trY + shifts2[i] * trZ);
                    bottomRightPos.Add(points[i] + shifts1[i] * brY + shifts2[i] * brZ);
                }

                var globalTransform = PropagateRefPoint(wall.centerPath.refPoint, gMD);

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

        public static IEnumerable<Mesh> GenerateStairs(Stairs wall, GeneratedMapData gMD)
        {
            var centerPath = gMD.renderedPaths.FindLast(it => it.id == wall.centerPath.idY);
            var totalPathLength = centerPath.length * (wall.t1 - wall.t0);
            var tmp = Mathf.Round(100000f * totalPathLength / wall.stepLength) / 100000;
            var realCount = Mathf.FloorToInt(tmp);
            var step = wall.stepLength / totalPathLength;
            
            // var t0Normalized = HillsGeneratorUtils.GetNormalizedValue(wall.t0, centerPath.length);
            // var t1Normalized = HillsGeneratorUtils.GetNormalizedValue(wall.t1, centerPath.length);
            // var stepLengthNormalized = HillsGeneratorUtils.GetNormalizedValue(wall.stepLength, centerPath.length);

            var leftTransform = PropagateRefPoint(wall.leftPath.refPoint, gMD).position;
            var rightTransform = PropagateRefPoint(wall.rightPath.refPoint, gMD).position;
            var topTransform = PropagateRefPoint(wall.topPath.refPoint, gMD).position;

            var leftPath = gMD.renderedPaths.FindLast(it => it.id == wall.leftPath.idZ);
            var rightPath = gMD.renderedPaths.FindLast(it => it.id == wall.rightPath.idZ);
            var topPath = gMD.renderedPaths.FindLast(it => it.id == wall.topPath.idY);

            var leftOffset = OffsetFunction.FromRenderedPath(leftPath);
            var rightOffset = OffsetFunction.FromRenderedPath(rightPath);
            var topOffset = OffsetFunction.FromRenderedPath(topPath);

            //ToDo
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
                var t0 = c * step;
                var tApprox = Mathf.Round(100000 * (n - 1) * t0) / 100000;
                var startPoint = Mathf.CeilToInt(tApprox);
                var startVal = tApprox + 1 - startPoint;

                if (!Mathf.Approximately(startVal, 1))
                {
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

            var globalTransform = PropagateRefPoint(wall.centerPath.refPoint, gMD);

            for (var i = 0; i < realCount + 1; i++)
            {
                leftFinal[i] = HillsGeneratorUtils.Transformation(globalTransform, leftFinal[i]);
                rightFinal[i] = HillsGeneratorUtils.Transformation(globalTransform, rightFinal[i]);
                centerFinal[i] = HillsGeneratorUtils.Transformation(globalTransform, centerFinal[i]);
            }

            yield return MeshFunctions.GenerateStairs(centerFinal, leftFinal, rightFinal, shifts2);
        }
    }
}