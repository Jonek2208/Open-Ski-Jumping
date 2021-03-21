using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    [Serializable]
    public class Forest
    {
        public List<Vector2> points;
        public float spacing;
        public float noiseScale;
        public float minHeight;
        public float maxHeight;
        public float minWidth;
        public float maxWidth;
    }

    public class ForestGenerator : MonoBehaviour
    {
        [SerializeField] private List<Forest> forests;

        [SerializeField] private List<Terrain> terrains;

        public void GenerateForests()
        {
            foreach (var forest in forests)
            {
                GenerateForest(forest);
            }
        }

        public void GenerateForest(Forest forest)
        {
            var n = forest.points.Count;
            var minX = forest.points[0].x;
            var minZ = forest.points[0].y;
            var maxX = forest.points[0].x;
            var maxZ = forest.points[0].y;

            for (var i = 0; i < n; i++)
            {
                minX = Mathf.Min(minX, forest.points[i].x);
                minZ = Mathf.Min(minZ, forest.points[i].y);
                maxX = Mathf.Max(maxX, forest.points[i].x);
                maxZ = Mathf.Max(maxZ, forest.points[i].y);
            }

            var trees = new List<Vector2>();

            for (var i = 0; minX + i * forest.spacing <= maxX; i++)
            {
                for (var j = 0; minZ + j * forest.spacing <= maxZ; j++)
                {
                    var point = new Vector2(minX + i * forest.spacing, minZ + j * forest.spacing);
                    // Debug.Log(point);
                    if (IsInside(forest.points, point))
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
                        .Select(position => (position,
                            noise: Noise2D(new Vector2(position.x / data.size.x, position.y / data.size.z),
                                forest.noiseScale, 0.324324f,
                                0.1345457f, 0.79238f, 0.857283f))).Select(it => new TreeInstance
                        {
                            position = new Vector3(
                                (it.position.x - min.x +
                                 it.noise.x * forest.spacing / 2) /
                                data.size.x, 0,
                                (it.position.y - min.z +
                                 it.noise.y * forest.spacing / 2) /
                                data.size.z),
                            heightScale = Mathf.Lerp(forest.minHeight, forest.maxHeight,
                                Mathf.PerlinNoise(it.position.x / data.size.x * forest.noiseScale + 0.1235f,
                                    it.position.y / data.size.z * forest.noiseScale + 0.786351f)),
                            widthScale = Mathf.Lerp(forest.minWidth, forest.maxWidth,
                                Mathf.PerlinNoise(it.position.x / data.size.x * forest.noiseScale + 0.43543f,
                                    it.position.y / data.size.z * forest.noiseScale + 0.8561498f)),
                            rotation =
                                360 * Mathf.PerlinNoise(it.position.x / data.size.x * forest.noiseScale + 0.43543f,
                                    it.position.y / data.size.z * forest.noiseScale + 0.8561498f),
                            color = NoiseColor(new Vector2(it.position.x / data.size.x, it.position.y / data.size.z),
                                forest.noiseScale, 0.324324f,
                                0.1345457f, 0.79238f, 0.857283f, 0.64388345f, 0.298435f),
                            lightmapColor = new Color32(255, 255, 255, 255)
                        })
                        .ToArray();
                data.SetTreeInstances(treeInstances, true);
                terr.Flush();
            }
        }

        private static float Cross(Vector2 lhs, Vector2 rhs) => lhs.x * rhs.y - lhs.y * rhs.x;

        private static bool IsInside(List<Vector2> polygon, Vector2 point)
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

        public static Vector2 Noise2D(Vector2 position, float scale, float x0, float x1, float y0, float y1)
        {
            return new Vector2(2 * Mathf.PerlinNoise(position.x * scale + x0, position.y * scale + y0) - 1,
                2 * Mathf.PerlinNoise(position.x * scale + x1, position.y * scale + y1) - 1);
        }

        public static Color32 NoiseColor(Vector2 position, float scale, float x0, float x1, float x2, float y0,
            float y1, float y2)
        {
            return new Color32((byte) (255 * Mathf.PerlinNoise(position.x * scale + x0, position.y * scale + y0)),
                (byte) (255 * Mathf.PerlinNoise(position.x * scale + x1, position.y * scale + y1)),
                (byte) (255 * Mathf.PerlinNoise(position.x * scale + x2, position.y * scale + y2)), 255);
        }
    }
}