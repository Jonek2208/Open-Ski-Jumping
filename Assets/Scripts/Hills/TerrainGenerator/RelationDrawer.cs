using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.Hills.TerrainGenerator
{
    public class RelationDrawer : MonoBehaviour
    {
        [SerializeField] private List<string> ids;
        [SerializeField] private OsmReader osmReader;
        [SerializeField] private MapLocalTangentPlane mapLtp;
        [SerializeField] private ElevationData terrainData;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private float width;
        [SerializeField] private Vector3 offset;
        [SerializeField] private ElevationData elevationData;
        [SerializeField] private Vector2 centerCoords;


        public void Generate()
        {
            elevationData.GetTileContainingCoord(centerCoords, out var tile);
            // var tile = new DataTile(HgtReader.ReadFile(terrainData.FilePath), terrainData.Coords,
            //     terrainData.Resolution);
            osmReader.Read();
            var tmp = new List<Vector3>();
            foreach (var id in ids)
            {
                tmp.AddRange(osmReader.GetWayById(id)
                    .Select(it => new Vector3(it.x, it.y, tile.GetInterpolatedHeight(it) - 400)));
            }

            var points = tmp.Select(it =>
                mapLtp.FromGeog(it)).ToArray();
            var n = points.Length;
            var segments = new Vector3[n - 1];
            for (var i = 0; i < n - 1; i++)
                segments[i] = points[i + 1] - points[i];
            var normals = segments.Select(it => Vector3.Cross(it, Vector3.up).normalized).ToArray();
            var shifts = new Vector3[n];
            shifts[0] = normals[0];
            shifts[n - 1] = normals[n - 2];
            for (var i = 1; i < n - 1; i++)
            {
                var v = normals[i - 1] + normals[i];
                shifts[i] = v * Vector3.Dot(normals[i], normals[i]) / Vector3.Dot(v, normals[i]);
            }

            var vertices = new Vector3[2 * n];
            var triangles = new int[6 * n - 6];
            for (var i = 0; i < n; i++)
            {
                vertices[2 * i] = points[i] + shifts[i] * width + offset;
                vertices[2 * i + 1] = points[i] - shifts[i] * width + offset;
            }

            for (var i = 0; i < n - 1; i++)
            {
                triangles[6 * i] = 2 * i;
                triangles[6 * i + 1] = 2 * i + 2;
                triangles[6 * i + 2] = 2 * i + 3;
                triangles[6 * i + 3] = 2 * i;
                triangles[6 * i + 4] = 2 * i + 3;
                triangles[6 * i + 5] = 2 * i + 1;
            }

            var mesh = new Mesh
                {vertices = vertices, triangles = triangles, uv = new Vector2[2 * n]};
            mesh.RecalculateNormals();
            meshFilter.sharedMesh = mesh;
        }

        public void GenerateTrees()
        {
        }

        public void FillTriangle(List<Vector2> vertices, int[,] image)
        {
            vertices = vertices.Select(it => new Vector2(Mathf.Round(it.x), Mathf.Round(it.y))).OrderBy(it => it.y)
                .ToList();
            if (vertices[0].y == vertices[1].y)
            {
            }

            var xs = vertices.Select(it => it.x).ToArray();
            var ys = vertices.Select(it => it.y).ToArray();

            var minX = Mathf.Min(xs);
            var maxX = Mathf.Max(xs);
            var minY = Mathf.Min(ys);
            var maxY = Mathf.Max(ys);

            var xCount = image.GetLength(0);
            var yCount = image.GetLength(1);

            for (var i = 0; i < yCount; i++)
            {
                for (var j = 0; j < xCount; j++)
                {
                }
            }
        }
    }
}