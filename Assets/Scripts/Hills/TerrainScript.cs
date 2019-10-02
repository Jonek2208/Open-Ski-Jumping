using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public List<MeshScript> hills;
    public Terrain[] terrains;
    public float offset;
    public float inrunFlatLength;
    public GameObject terrainObject;
    public TerrainBase terrainBase;

    public void GetTerrain()
    {
        terrains = terrainObject.GetComponentsInChildren<Terrain>();
    }

    public void GenerateTerrain()
    {
        GetTerrain();

        List<HillProfile.Hill> hillsList = new List<HillProfile.Hill>();

        foreach (var it in hills)
        {
            Transform hillTransform = it.GetComponent<Transform>();
            HillProfile.Hill hill = new HillProfile.Hill(it.profileData);
            hill.Calculate();
            float inrunTerrain = it.inrunTerrain;
            foreach (var terr in terrains)
            {
                Vector3 center = terr.GetComponent<Transform>().position;
                float[,] tab = new float[terr.terrainData.heightmapResolution, terr.terrainData.heightmapResolution];
                for (int i = 0; i < terr.terrainData.heightmapResolution; i++)
                {
                    for (int j = 0; j < terr.terrainData.heightmapResolution; j++)
                    {
                        float x = (float)(j) / (terr.terrainData.heightmapResolution - 1) * (terr.terrainData.size.x) + center.x;
                        float z = (float)(i) / (terr.terrainData.heightmapResolution - 1) * (terr.terrainData.size.z) + center.z;

                        Vector3 pointOnHill = hillTransform.InverseTransformPoint(new Vector3(x, 0, z));

                        float hillY = 0;
                        float b = 15;
                        float c = 0;

                        if (pointOnHill.x < hill.A.x)
                        {
                            c = hill.A.x + inrunFlatLength - pointOnHill.x;
                            hillY = hill.A.y * inrunTerrain - hill.s;
                        }
                        else if (pointOnHill.x < hill.T.x)
                        {
                            hillY = hill.Inrun(pointOnHill.x) * inrunTerrain - hill.s;
                            if (hill.A.x <= pointOnHill.x) b = 15;
                        }
                        else if (hill.T.x <= pointOnHill.x && pointOnHill.x <= hill.U.x)
                        {
                            hillY = hill.LandingArea(pointOnHill.x);
                            b = 15;
                        }
                        else if (pointOnHill.x <= hill.U.x + hill.a)
                        {
                            hillY = hill.U.y;
                            b = 15;
                        }
                        else
                        {
                            c = pointOnHill.x - hill.U.x - hill.a;
                            hillY = hill.U.y;
                        }

                        hillY += hillTransform.position.y;

                        // float terrainY = 200 * Mathf.PerlinNoise(x / 200.0f + 2000, z / 200.0f + 2000);
                        float terrainY = hill.U.y;
                        if (terrainBase == TerrainBase.PerlinNoise)
                        {
                            terrainY = 200 * Mathf.PerlinNoise(x / 200.0f + 2000, z / 200.0f + 2000);
                        }
                        else if (terrainBase == TerrainBase.currentTerrain)
                        {
                            terrainY = terr.terrainData.GetHeight(j, i) + center.y;
                        }

                        float blendFactor = Mathf.SmoothStep(0, 1, Mathf.Clamp01(new Vector2(Mathf.Clamp01((Mathf.Abs(pointOnHill.z) - b) / offset), Mathf.Clamp01(c / offset)).magnitude));
                        float y = hillY * (1 - blendFactor) + terrainY * blendFactor;

                        y = (y - center.y - 1) / terr.terrainData.size.y;
                        tab[i, j] = Mathf.Clamp(y, 0, 1);
                    }
                }

                terr.terrainData.SetHeights(0, 0, tab);
            }
        }
    }
}
