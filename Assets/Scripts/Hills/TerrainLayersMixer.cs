using System.Collections.Generic;
using OpenSkiJumping.PerlinNoise;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    public class TerrainLayersMixer : MonoBehaviour
    {
        [SerializeField] private List<Terrain> terrains;
        [SerializeField] private int seed;
        [SerializeField] private float scale;
        [SerializeField] private int octaves;
        [SerializeField] private float persistence;
        [SerializeField] private float lacunarity;


        public void MixTextures()
        {
            foreach (var terr in terrains)
            {
                var pos = terr.transform.position;
                MixLayers(terr, new Vector2(pos.x, pos.z));
            }
        }

        public void MixLayers(Terrain terr, Vector2 offset)
        {
            var terrainData = terr.terrainData;
            var height = terrainData.alphamapHeight;
            var width = terrainData.alphamapWidth;
            var offsetNormalized = new Vector2(offset.x / width, offset.y / height);
            var noiseData =
                Noise.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offsetNormalized);

            var map = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 2];
            for (var y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (var x = 0; x < terrainData.alphamapWidth; x++)
                {
                    var frac = noiseData[x, y];
                    map[x, y, 0] = frac;
                    map[x, y, 1] = 1 - frac;
                }
            }

            terrainData.SetAlphamaps(0, 0, map);
        }
    }
}