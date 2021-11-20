using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    public static class MeshFunctions
    {
        public static int[] FacesToTriangles(IEnumerable<(int, int, int, int)> facesList)
        {
            var triangles = new List<int>();
            foreach (var face in facesList)
            {
                triangles.Add(face.Item1);
                triangles.Add(face.Item2);
                triangles.Add(face.Item3);
                triangles.Add(face.Item2);
                triangles.Add(face.Item4);
                triangles.Add(face.Item3);
            }

            return triangles.ToArray();
        }

        public static IEnumerable<int> MakeTriangle(int v1, int v2, int v3)
        {
            yield return v1;
            yield return v2;
            yield return v3;
        }

        public static IEnumerable<int> MakeFace(int v1, int v2, int v3, int v4)
        {
            yield return v1;
            yield return v2;
            yield return v3;
            yield return v2;
            yield return v4;
            yield return v3;
        }

        public static Mesh MergeMeshes(IEnumerable<Mesh> meshes)
        {
            var mergedVerts = new List<Vector3>();
            var mergedUvs = new List<Vector2>();
            var mergedTris = new List<int>();

            foreach (var mesh in meshes)
            {
                var vCount = mergedVerts.Count;
                mergedVerts.AddRange(mesh.vertices);
                mergedUvs.AddRange(mesh.uv);
                mergedTris.AddRange(mesh.triangles.Select(it => it + vCount));
            }

            var mergedMesh = new Mesh
                {vertices = mergedVerts.ToArray(), uv = mergedUvs.ToArray(), triangles = mergedTris.ToArray()};
            mergedMesh.RecalculateNormals();
            return mergedMesh;
        }

        public static Mesh GeneratePathStrip(IList<Vector3> pos1, IList<Vector3> pos2)
        {
            var n = Mathf.Min(pos1.Count, pos2.Count);

            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();
            var faces = new List<(int, int, int, int)>();

            for (var i = 0; i < n; i++)
            {
                vertices.Add(pos1[i]);
                uvs.Add(pos1[i]);

                vertices.Add(pos2[i]);
                uvs.Add(pos2[i]);

                if (i == 0) continue;
                faces.Add((2 * i + 0, 2 * i + 1, 2 * i - 2, 2 * i - 1));
            }

            var triangles = FacesToTriangles(faces);
            return new Mesh {vertices = vertices.ToArray(), triangles = triangles, uv = uvs.ToArray()};
        }

        public static Mesh GenerateQuad(Vector3 topLeftPos, Vector3 topRightPos, Vector3 bottomLeftPos,
            Vector3 bottomRightPos)
        {
            var vertices = new List<Vector3> {topLeftPos, topRightPos, bottomLeftPos, bottomRightPos};
            var uvs = new List<Vector2> {topLeftPos, topRightPos, bottomLeftPos, bottomRightPos};
            var faces = new List<(int, int, int, int)> {(0, 1, 2, 3)};
            var triangles = FacesToTriangles(faces);
            return new Mesh {vertices = vertices.ToArray(), triangles = triangles, uv = uvs.ToArray()};
        }


        public static Mesh GeneratePathMesh(IList<Vector3> topLeftPos, IList<Vector3> topRightPos,
            IList<Vector3> bottomLeftPos, IList<Vector3> bottomRightPos,
            bool front = true, bool back = true, bool right = true, bool left = true,
            bool bottom = true, bool top = true)
        {
            var n = Mathf.Min(topLeftPos.Count, topRightPos.Count, bottomLeftPos.Count, bottomRightPos.Count);
            var meshes = new List<Mesh>();

            if (front)
            {
                meshes.Add(GenerateQuad(topLeftPos[0], topRightPos[0], bottomLeftPos[0], bottomRightPos[0]));
            }

            if (back)
            {
                meshes.Add(GenerateQuad(topRightPos[n - 1], topLeftPos[n - 1],
                    bottomRightPos[n - 1], bottomLeftPos[n - 1]));
            }

            if (right)
            {
                meshes.Add(GeneratePathStrip(topRightPos, bottomRightPos));
            }

            if (left)
            {
                meshes.Add(GeneratePathStrip(bottomLeftPos, topLeftPos));
            }

            if (bottom)
            {
                meshes.Add(GeneratePathStrip(bottomRightPos, bottomLeftPos));
            }

            if (top)
            {
                meshes.Add(GeneratePathStrip(topLeftPos, topRightPos));
            }

            return MergeMeshes(meshes);
        }

        public static Mesh GenerateCustomPathMesh(List<Vector3>[] positions, bool[] flags, bool front, bool back)
        {
            var n = Mathf.Min(flags.Length, positions.Length);
            var meshes = new List<Mesh>();

            // if (front)
            // {
            //     GeneratePolygon();
            // }
            //
            // if (back)
            // {
            //     GeneratePolygon();
            // }

            for (var i = 0; i < n; i++)
            {
                if (flags[i])
                {
                    meshes.Add(GeneratePathStrip(positions[i], positions[(i + 1) % n]));
                }
            }

            return MergeMeshes(meshes);
        }


        public static Mesh GenerateStairs(IList<Vector3> center, IList<Vector3> leftPos, IList<Vector3> rightPos,
            IList<Vector3> normals)
        {
            var n = Mathf.Min(leftPos.Count, rightPos.Count);
            var mesh = new Mesh();
            var verticesList = new List<Vector3>();
            var uvsList = new List<Vector2>();
            var trianglesList = new List<int>();

            for (var i = 0; i < n - 1; i++)
            {
                var width = Vector3.Distance(leftPos[i], rightPos[i]);

                //riser
                var x = verticesList.Count;
                verticesList.Add(new Vector3(leftPos[i + 1].x, leftPos[i].y, leftPos[i + 1].z));
                uvsList.Add(new Vector2(0, leftPos[i].y - leftPos[i + 1].y));
                verticesList.Add(new Vector3(rightPos[i + 1].x, rightPos[i].y, rightPos[i + 1].z));
                uvsList.Add(new Vector2(width, leftPos[i].y - leftPos[i + 1].y));

                verticesList.Add(leftPos[i]);
                uvsList.Add(new Vector2(0, 0));
                verticesList.Add(rightPos[i]);
                uvsList.Add(new Vector2(width, 0));

                trianglesList.AddRange(MakeFace(x + 0, x + 1, x + 2, x + 3));

                //tread
                x = verticesList.Count;
                verticesList.Add(leftPos[i + 1]);
                uvsList.Add(new Vector2(0, 2 * (leftPos[i].y - leftPos[i + 1].y)));
                verticesList.Add(rightPos[i + 1]);
                uvsList.Add(new Vector2(0, 2 * (rightPos[i].y - rightPos[i + 1].y)));

                verticesList.Add(new Vector3(leftPos[i + 1].x, leftPos[i].y, leftPos[i + 1].z));
                uvsList.Add(new Vector2(0, leftPos[i].y - leftPos[i + 1].y));
                verticesList.Add(new Vector3(rightPos[i + 1].x, rightPos[i].y, rightPos[i + 1].z));
                uvsList.Add(new Vector2(width, leftPos[i].y - leftPos[i + 1].y));

                trianglesList.AddRange(MakeFace(x + 0, x + 1, x + 2, x + 3));

                // //stringer left
                // x = verticesList.Count;
                // verticesList.Add(leftPos[i + 1]);
                // verticesList.Add(new Vector3(leftPos[i + 1].x, leftPos[i].y, leftPos[i + 1].z));
                // verticesList.Add(leftPos[i]);
                //
                // uvsList.Add(Vector2.right * (leftPos[i].y - leftPos[i + 1].y));
                // uvsList.Add(Vector2.zero);
                // uvsList.Add(Vector2.left * (leftPos[i].y - leftPos[i + 1].y));
                //
                // trianglesList.AddRange(MakeTriangle(x + 0, x + 1, x + 2));
                //
                // //stringer right
                // x = verticesList.Count;
                // verticesList.Add(rightPos[i + 1]);
                // verticesList.Add(new Vector3(rightPos[i + 1].x, rightPos[i].y, rightPos[i + 1].z));
                // verticesList.Add(rightPos[i]);
                //
                // uvsList.Add(Vector2.right * (rightPos[i].y - rightPos[i + 1].y));
                // uvsList.Add(Vector2.zero);
                // uvsList.Add(Vector2.left * (rightPos[i].y - rightPos[i + 1].y));
                //
                // trianglesList.AddRange(MakeTriangle(x + 0, x + 2, x + 1));
            }

            mesh.vertices = verticesList.ToArray();
            mesh.uv = uvsList.ToArray();
            mesh.triangles = trianglesList.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}