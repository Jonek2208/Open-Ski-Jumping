using System;
using UnityEngine;

namespace OpenSkiJumping.PerlinNoise
{
    public class MapGenerator : MonoBehaviour
    {
        public enum DrawMode
        {
            NoiseMap,
            ColorMap
        }

        [SerializeField] private DrawMode drawMode;

        [SerializeField] private int mapWidth;
        [SerializeField] private int mapHeight;
        [SerializeField] private float noiseScale;
        [SerializeField] private MapDisplay mapDisplay;
        [SerializeField] private int octaves;
        [Range(0, 1)] [SerializeField] private float persistence;
        [SerializeField] private float lacunarity;
        [SerializeField] private int seed;
        [SerializeField] private Vector2 offset;

        [SerializeField] private TerrainType[] regions;


        public bool autoUpdate;

        public void GenerateMap()
        {
            var noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence,
                lacunarity, offset);

            var colorMap = new Color[mapWidth * mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    var currentHeight = noiseMap[x, y];
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }

            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                    break;
                case DrawMode.ColorMap:
                    mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
                    break;
            }
        }

        private void OnValidate()
        {
            mapWidth = Mathf.Max(mapWidth, 1);
            mapHeight = Mathf.Max(mapHeight, 1);
            lacunarity = Mathf.Max(lacunarity, 1);
            octaves = Mathf.Max(octaves, 1);
        }
    }

    [Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}