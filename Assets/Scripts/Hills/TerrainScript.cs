using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public bool alignHills;
    public float padding;
    public List<GameObject> hillObjects;
    private List<MeshScript> hills;
    private List<Hill> hillsList;
    public Terrain[] terrains;
    public float offset;
    public float inrunFlatLength;
    public GameObject terrainObject;
    public TerrainBase terrainBase;

    public void GetTerrain()
    {
        terrains = terrainObject.GetComponentsInChildren<Terrain>();
    }

    public void GetHills()
    {
        hills = new List<MeshScript>();
        foreach (var it in hillObjects)
        {
            hills.Add(it.GetComponentInChildren<MeshScript>());
        }
        hillsList = new List<Hill>();
        foreach (var it in hills)
        {
            Hill hill = new Hill(it.profileData.Value);
            hill.Calculate();
            hillsList.Add(hill);
        }
    }

    public void AutoAlignHills()
    {
        for (int i = 0; i < hillObjects.Count; i++)
        {
            if (i > 0)
            {
                hillObjects[i].GetComponent<Transform>().position = hillObjects[i - 1].GetComponent<Transform>().position +
                new Vector3(hillsList[i - 1].U.x - hillsList[i].U.x, hillsList[i - 1].U.y - hillsList[i].U.y, hillsList[i - 1].bU / 2 + hillsList[i].bU / 2 + padding);
            }
        }
    }

    public void GenerateTerrain()
    {
        GetHills();
        GetTerrain();

        if (alignHills) { AutoAlignHills(); }

        foreach (var terr in terrains)
        {
            Vector3 center = terr.GetComponent<Transform>().position;
            float[,] tab = new float[terr.terrainData.heightmapResolution, terr.terrainData.heightmapResolution];
            float[,] coeffs = new float[terr.terrainData.heightmapResolution, terr.terrainData.heightmapResolution];
            // bool[,] insideHill = new bool[terr.terrainData.heightmapResolution, terr.terrainData.heightmapResolution];
            for (int i = 0; i < terr.terrainData.heightmapResolution; i++)
            {
                for (int j = 0; j < terr.terrainData.heightmapResolution; j++)
                {
                    float x = (float)(j) / (terr.terrainData.heightmapResolution - 1) * (terr.terrainData.size.x) + center.x;
                    float z = (float)(i) / (terr.terrainData.heightmapResolution - 1) * (terr.terrainData.size.z) + center.z;
                    coeffs[i, j] = 2;
                    // int hillsCount = 0;
                    // float terrainHeight = 0;
                    for (int ii = 0; ii < hills.Count; ii++)
                    {
                        Transform hillTransform = hills[ii].GetComponent<Transform>();
                        Hill hill = hillsList[ii];
                        float inrunTerrain = hills[ii].inrunTerrain;
                        Vector3 pointOnHill = hillTransform.InverseTransformPoint(new Vector3(x, 0, z));

                        float hillY = 0;
                        float b = 15;
                        float c = 0;

                        if (pointOnHill.x < hill.A.x)
                        {
                            c = hill.A.x + inrunFlatLength - pointOnHill.x;
                            hillY = hill.A.y * inrunTerrain - hill.s;
                            b = hill.b1 / 2;
                        }
                        else if (pointOnHill.x < hill.T.x)
                        {
                            hillY = hill.Inrun(pointOnHill.x) * inrunTerrain - hill.s;
                            if (hill.A.x <= pointOnHill.x) b = hill.bK;
                            b = Mathf.Lerp(hill.b2, hill.b1, pointOnHill.x / hill.A.x) / 2;
                        }
                        else if (hill.T.x <= pointOnHill.x && pointOnHill.x <= hill.U.x)
                        {
                            hillY = hill.LandingArea(pointOnHill.x);
                            b = (pointOnHill.x <= hill.K.x ? (hill.b2 / 2) + pointOnHill.x / hill.K.x * ((hill.bK - hill.b2) / 2) :
                            pointOnHill.x >= hill.U.x ? (hill.bU / 2) : (hill.bK / 2) + (pointOnHill.x - hill.K.x) / (hill.U.x - hill.K.x) * ((hill.bU - hill.bK) / 2));
                        }
                        else if (pointOnHill.x <= hill.U.x + hill.a)
                        {
                            hillY = hill.U.y;
                            b = hill.bU / 2;
                        }
                        else
                        {
                            c = pointOnHill.x - hill.U.x - hill.a;
                            hillY = hill.U.y;
                        }

                        hillY += hillTransform.position.y;

                        // float terrainY = 200 * Mathf.PerlinNoise(x / 200.0f + 2000, z / 200.0f + 2000);
                        float terrainY = hill.U.y + hillTransform.GetComponent<Transform>().position.y;
                        if (terrainBase == TerrainBase.PerlinNoise)
                        {
                            terrainY = 200 * Mathf.PerlinNoise(x / 200.0f + 2000, z / 200.0f + 2000);
                        }
                        else if (terrainBase == TerrainBase.currentTerrain)
                        {
                            terrainY = terr.terrainData.GetHeight(j, i) + center.y;
                            // terrainY = coeffs[i, j] + center.y;
                        }

                        float blendFactor = Mathf.SmoothStep(0, 1, Mathf.Clamp01(new Vector2(Mathf.Clamp01((Mathf.Abs(pointOnHill.z) - b) / offset), Mathf.Clamp01(c / offset)).magnitude));
                        // float blendFactor = Mathf.Clamp01(new Vector2(Mathf.Clamp01((Mathf.Abs(pointOnHill.z) - b) / offset), Mathf.Clamp01(c / offset)).magnitude);
                        float y = hillY * (1 - blendFactor) + terrainY * blendFactor;
                        y = (y - center.y - 1) / terr.terrainData.size.y;
                        if (blendFactor < coeffs[i, j])
                        {
                            coeffs[i, j] = blendFactor;
                            tab[i, j] = y;
                        }
                        // if (insideHill[i, j] == false) { tab[i, j] = y; }

                        // if (blendFactor == 0) { insideHill[i, j] = true; }
                    }
                }
            }
            terr.terrainData.SetHeights(0, 0, tab);
        }
    }
}

