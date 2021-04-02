using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    [Serializable]
    public class Curve
    {
        public List<Vector3> points;

        private static Vector3 Perpendicular(Vector3 v)
        {
            return Vector3.Cross(v, Vector3.up);
        }

        public Curve ShiftedCurve(float width, float height)
        {
            var normals = new List<Vector3>();
            for (var i = 0; i < points.Count - 1; i++)
            {
                normals.Add(Perpendicular(points[i + 1] - points[i]).normalized);
            }

            var offsets = new List<Vector3> {normals[0]};
            for (var i = 0; i < points.Count - 2; i++)
            {
                var v = (normals[i] + normals[i + 1]) / 2;
                v *= Vector3.Dot(normals[i], normals[i]) / Vector3.Dot(v, normals[i]);
                offsets.Add(v);
            }

            offsets.Add(normals[normals.Count - 1]);
            for (var i = 0; i < offsets.Count; i++)
            {
                offsets[i] = points[i] + offsets[i] * width + Vector3.up * height;
            }

            return new Curve {points = offsets};
        }
    }

    [Serializable]
    public class Seats
    {
        public Curve curve;
        public float width;
        public float height;
        public float stepHeight;
    }

    public class SeatsGenerator : MonoBehaviour
    {
        [SerializeField] private Seats seats;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private bool useLineRendererPoints;


        public void Generate()
        {
            seats.stepHeight = seats.height / Mathf.Round(seats.height / seats.stepHeight);
            var stepsCount = Mathf.RoundToInt(seats.height / seats.stepHeight);
            if (useLineRendererPoints)
            {
                var xd = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(xd);
                seats.curve.points = xd.ToList();
            }

            var inside = seats.curve;
            var outside = seats.curve.ShiftedCurve(seats.width, seats.height);

            var n = seats.curve.points.Count;
            var vertices = new Vector3[4 * (n - 1) * 2 * stepsCount];
            var tris = new int[6 * (n - 1) * 2 * stepsCount];
            var uvs = new Vector2[4 * (n - 1) * 2 * stepsCount];

            for (var i = 0; i < n - 1; i++)
            {
                for (var j = 0; j < stepsCount; j++)
                {
                    var heightOffset = Vector3.up * seats.height / stepsCount;
                    var x0 = Vector3.Lerp(inside.points[i], outside.points[i], (float) j / stepsCount) + heightOffset;
                    var x1 = Vector3.Lerp(inside.points[i + 1], outside.points[i + 1], (float) j / stepsCount) +
                             heightOffset;
                    var y0 = Vector3.Lerp(inside.points[i], outside.points[i], (float) (j + 1) / stepsCount);
                    var y1 = Vector3.Lerp(inside.points[i + 1], outside.points[i + 1], (float) (j + 1) / stepsCount);
                    var z0 = Vector3.Lerp(inside.points[i], outside.points[i], (float) j / stepsCount);
                    var z1 = Vector3.Lerp(inside.points[i + 1], outside.points[i + 1], (float) j / stepsCount);

                    vertices[8 * (stepsCount * i + j)] = x0;
                    uvs[8 * (stepsCount * i + j)] = new Vector2(x0.x, x0.z);
                    vertices[8 * (stepsCount * i + j) + 1] = y0;
                    uvs[8 * (stepsCount * i + j) + 1] = new Vector2(y0.x, y0.z);

                    vertices[8 * (stepsCount * i + j) + 2] = x1;
                    uvs[8 * (stepsCount * i + j) + 2] = new Vector2(x1.x, x1.z);
                    vertices[8 * (stepsCount * i + j) + 3] = y1;
                    uvs[8 * (stepsCount * i + j) + 3] = new Vector2(y1.x, y1.z);

                    vertices[8 * (stepsCount * i + j) + 4] = z0;
                    uvs[8 * (stepsCount * i + j) + 4] = new Vector2(z0.x, z0.z);
                    vertices[8 * (stepsCount * i + j) + 5] = x0;
                    uvs[8 * (stepsCount * i + j) + 5] = new Vector2(x0.x, x0.z);

                    vertices[8 * (stepsCount * i + j) + 6] = z1;
                    uvs[8 * (stepsCount * i + j) + 6] = new Vector2(z1.x, z1.z);
                    vertices[8 * (stepsCount * i + j) + 7] = x1;
                    uvs[8 * (stepsCount * i + j) + 7] = new Vector2(x1.x, x1.z);
                }
            }

            for (var i = 0; i < n - 1; i++)
            {
                for (var j = 0; j < stepsCount; j++)
                {
                    tris[12 * (stepsCount * i + j)] = 8 * (stepsCount * i + j);
                    tris[12 * (stepsCount * i + j) + 1] = 8 * (stepsCount * i + j) + 1;
                    tris[12 * (stepsCount * i + j) + 2] = 8 * (stepsCount * i + j) + 3;
                    tris[12 * (stepsCount * i + j) + 3] = 8 * (stepsCount * i + j);
                    tris[12 * (stepsCount * i + j) + 4] = 8 * (stepsCount * i + j) + 3;
                    tris[12 * (stepsCount * i + j) + 5] = 8 * (stepsCount * i + j) + 2;
                    
                    tris[12 * (stepsCount * i + j) + 6] = 8 * (stepsCount * i + j) + 4;
                    tris[12 * (stepsCount * i + j) + 7] = 8 * (stepsCount * i + j) + 5;
                    tris[12 * (stepsCount * i + j) + 8] = 8 * (stepsCount * i + j) + 7;
                    tris[12 * (stepsCount * i + j) + 9] = 8 * (stepsCount * i + j) + 4;
                    tris[12 * (stepsCount * i + j) + 10] = 8 * (stepsCount * i + j) + 7;
                    tris[12 * (stepsCount * i + j) + 11] = 8 * (stepsCount * i + j) + 6;
                }
            }

            var mesh = new Mesh {vertices = vertices, triangles = tris, uv = uvs};
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshFilter.sharedMesh = mesh;
        }
    }
}