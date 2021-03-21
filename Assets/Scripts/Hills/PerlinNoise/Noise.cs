using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.PerlinNoise
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves,
            float persistence, float lacunarity, Vector2 offset)
        {
            var prng = new System.Random(seed);
            var octaveOffsets = Enumerable.Range(0, octaves)
                .Select(it => new Vector2(prng.Next(-100000, 100000), prng.Next(-100000, 100000)) + offset).ToArray();

      

            var noiseMap = new float[mapWidth, mapHeight];
            scale = Mathf.Max(scale, 0.0001f);

            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;

            var halfWidth = mapWidth * 0.5f;
            var halfHeight = mapHeight * 0.5f;

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    var amplitude = 1f;
                    var frequency = 1f;
                    var noiseHeight = 0f;

                    for (var i = 0; i < octaves; i++)
                    {
                        var sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        var sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
                    minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}
