using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.Hills.TerrainGenerator
{
    public class ForestDrawer : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private OsmReader osmReader;
        [SerializeField] private MapLocalTangentPlane mapLtp;
        [SerializeField] private ElevationData terrainData;
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private float width;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float xSpacing;
        [SerializeField] private float zSpacing;
        [SerializeField] private GameObject tree;
        [SerializeField] private List<Terrain> terrains;
        [SerializeField] private ElevationData elevationData;
        [SerializeField] private Vector2 centerCoords;


        public void Generate()
        {
            elevationData.GetTileContainingCoord(centerCoords, out var tile);
            // var tile = new DataTile(HgtReader.ReadFile(terrainData.FilePath), terrainData.Coords,
            //     terrainData.Resolution);
            osmReader.Read();
            var tmp = osmReader.GetWayById(id)
                .Select(it => new Vector3(it.x, it.y, tile.GetInterpolatedHeight(it) - 400)).ToList();
            var points = tmp.Select(it =>
                mapLtp.FromGeog(it)).ToArray();
            var n = points.Length;
            var minX = points[0].x;
            var minZ = points[0].z;
            var maxX = points[0].x;
            var maxZ = points[0].z;

            for (var i = 0; i < n; i++)
            {
                minX = Mathf.Min(minX, points[i].x);
                minZ = Mathf.Min(minZ, points[i].z);

                maxX = Mathf.Max(maxX, points[i].x);
                maxZ = Mathf.Max(maxZ, points[i].z);
            }

            var poly = points.Select(it => new Vector2(it.x, it.z)).ToList();
            var trees = new List<Vector2>();

            for (var i = 0; minX + i * xSpacing <= maxX; i++)
            {
                for (var j = 0; minZ + j * zSpacing <= maxZ; j++)
                {
                    var point = new Vector2(minX + i * xSpacing, minZ + j * zSpacing);
                    // Debug.Log(point);
                    if (IsInside(poly, point))
                    {
                        trees.Add(point);
                    }
                }
            }

            foreach (var terr in terrains)
            {
                var data = terr.terrainData;
                var min = terr.transform.position;
                var max = min + data.size;
                var treeInstances =
                    trees.Where(position =>
                            min.x <= position.x && position.x <= max.x && min.z <= position.y && position.y <= max.z)
                        .Select(position => (position, noise: Noise2D(position))).Select(it => new TreeInstance
                        {
                            position = new Vector3(
                                (it.position.x - min.x +
                                 it.noise.x * xSpacing / 2) /
                                data.size.x, 0,
                                (it.position.y - min.z +
                                 it.noise.y * zSpacing / 2) /
                                data.size.z),
                            heightScale = 1.0f,
                            widthScale = 1.0f,
                            rotation = 2f * it.noise.y * it.noise.y * Mathf.PI,
                            color = new Color32(200, 200, 200, 255),
                            lightmapColor = new Color32(255, 255, 255, 255)
                        })
                        .ToArray();
                data.SetTreeInstances(treeInstances, true);
                terr.Flush();
            }
        }

        private static float Cross(Vector2 lhs, Vector2 rhs) => lhs.x * rhs.y - lhs.y * rhs.x;

        public static bool IsInside(List<Vector2> polygon, Vector2 point)
        {
            var area = 0f;
            var cnt = 0;
            for (var i = 1; i < polygon.Count - 1; i++)
            {
                var a = polygon[i] - polygon[0];
                var b = polygon[i + 1] - polygon[i];
                var c = polygon[0] - polygon[i + 1];

                var tmp = 0;

                tmp += Cross(a, point - polygon[0]) > 0 ? 1 : -1;
                tmp += Cross(b, point - polygon[i]) > 0 ? 1 : -1;
                tmp += Cross(c, point - polygon[i + 1]) > 0 ? 1 : -1;

                var triangleArea = Cross(a, b);

                if (tmp == 3 || tmp == -3)
                    cnt += triangleArea > 0 ? 1 : -1;

                area += triangleArea;
            }

            cnt = area > 0 ? cnt : -cnt;
            return cnt > 0;
        }

        public static Vector2 Noise2D(Vector2 position)
        {
            var rng = new System.Random(19997);
            const float scale = 5f;
            var x0 = rng.Next(-100000, 100000);
            var y0 = rng.Next(-100000, 100000);
            var x1 = rng.Next(-100000, 100000);
            var y1 = rng.Next(-100000, 100000);
            return new Vector2(2 * Mathf.PerlinNoise(position.x * scale + x0, position.y * scale + y0) - 1,
                2 * Mathf.PerlinNoise(position.x * scale + x1, position.y * scale + y1) - 1);
        }
    }
}