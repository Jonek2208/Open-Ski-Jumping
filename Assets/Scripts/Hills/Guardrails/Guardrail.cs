using System.Collections.Generic;
using UnityEngine;

namespace OpenSkiJumping.Hills.Guardrails
{
    [CreateAssetMenu(menuName = "HillElements/Guardrail")]
    public class Guardrail : ScriptableObject
    {
        [SerializeField]
        private float height;
        [SerializeField]
        private float width = 0.1f;
        [SerializeField]
        private float toTerrain = 2f;

        [SerializeField]
        private Material material;

        public float Width { get => width; }

        public Mesh Generate(Vector3[] points, int side)
        {
            int sgn = (side == 0 ? -1 : 1);

            Mesh mesh = new Mesh();
            List<Vector3> verticesList = new List<Vector3>();
            List<Vector2> uvsList = new List<Vector2>();
            List<(int, int, int, int)> facesList = new List<(int, int, int, int)>();

            List<float> len = new List<float>();
            len.Add(0);
            for (int i = 1; i < points.Length; i++) { len.Add((points[i] - points[i - 1]).magnitude + len[i - 1]); }

            for (int i = 0; i < points.Length; i++)
            {
                verticesList.Add(new Vector3(points[i].x, points[i].y - toTerrain, points[i].z + sgn * Width - Width));
                uvsList.Add(new Vector2(len[i], 0));

                verticesList.Add(new Vector3(points[i].x, points[i].y + height, points[i].z + sgn * Width - Width));
                uvsList.Add(new Vector2(len[i], height + toTerrain));

                verticesList.Add(new Vector3(points[i].x, points[i].y + height, points[i].z + sgn * Width - Width));
                uvsList.Add(new Vector2(len[i], height + toTerrain));

                verticesList.Add(new Vector3(points[i].x, points[i].y + height, points[i].z + sgn * Width + Width));
                uvsList.Add(new Vector2(len[i], height + Width + toTerrain));

                verticesList.Add(new Vector3(points[i].x, points[i].y + height, points[i].z + sgn * Width + Width));
                uvsList.Add(new Vector2(len[i], height + Width + toTerrain));

                verticesList.Add(new Vector3(points[i].x, points[i].y - toTerrain, points[i].z + sgn * Width + Width));
                uvsList.Add(new Vector2(len[i], height + Width + height + toTerrain + toTerrain));

                if (i > 0)
                {
                    int x = verticesList.Count;
                    facesList.Add((x - 12, x - 11, x - 6, x - 5));
                    facesList.Add((x - 10, x - 9, x - 4, x - 3));
                    facesList.Add((x - 8, x - 7, x - 2, x - 1));
                }
            }

            // front
            verticesList.Add(new Vector3(points[0].x, points[0].y - toTerrain, points[0].z + sgn * Width - Width));
            uvsList.Add(new Vector2(points[0].y - toTerrain, points[0].z + sgn * Width - Width));

            verticesList.Add(new Vector3(points[0].x, points[0].y + height, points[0].z + sgn * Width - Width));
            uvsList.Add(new Vector2(points[0].y + height, points[0].z + sgn * Width - Width));

            verticesList.Add(new Vector3(points[0].x, points[0].y + height, points[0].z + sgn * Width + Width));
            uvsList.Add(new Vector2(points[0].y + height, points[0].z + sgn * Width + Width));

            verticesList.Add(new Vector3(points[0].x, points[0].y - toTerrain, points[0].z + sgn * Width + Width));
            uvsList.Add(new Vector2(points[0].y - toTerrain, points[0].z + sgn * Width + Width));

            int xx = verticesList.Count;
            facesList.Add((xx - 4, xx - 1, xx - 3, xx - 2));
            // back 
            int last = points.Length - 1;
            verticesList.Add(new Vector3(points[last].x, points[last].y - toTerrain, points[last].z + sgn * Width - Width));
            uvsList.Add(new Vector2(points[last].y - toTerrain, points[last].z + sgn * Width - Width));

            verticesList.Add(new Vector3(points[last].x, points[last].y + height, points[last].z + sgn * Width - Width));
            uvsList.Add(new Vector2(points[last].y + height, points[last].z + sgn * Width - Width));

            verticesList.Add(new Vector3(points[last].x, points[last].y + height, points[last].z + sgn * Width + Width));
            uvsList.Add(new Vector2(points[last].y + height, points[last].z + sgn * Width + Width));

            verticesList.Add(new Vector3(points[last].x, points[last].y - toTerrain, points[last].z + sgn * Width + Width));
            uvsList.Add(new Vector2(points[last].y - toTerrain, points[last].z + sgn * Width + Width));

            xx = verticesList.Count;
            facesList.Add((xx - 1, xx - 4, xx - 2, xx - 3));

            mesh.vertices = verticesList.ToArray();
            mesh.triangles = MeshFunctions.FacesToTriangles(facesList);
            mesh.uv = uvsList.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }


        public Material GetMaterial()
        {
            return material;
        }

    }
}