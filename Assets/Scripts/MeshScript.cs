using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HillProfile;

[System.Serializable]
public struct HillModel
{
    public GameObject inrun;
    public GameObject landingArea;

    //Hill construction
    public GameObject gateStairs;
    public GameObject startGate;
    public GameObject inrunConstruction;
    public GameObject outrun;
    Mesh inrunMesh;
    Mesh landingAreaMesh;

    Mesh gateStairsMesh;
    Mesh startGateMesh;

    Mesh inrunConstructionMesh;

    //Meshes
    public GameObject terrain;
    public Terrain[] terrains;

    public LineRenderer[] lineRenderers;

    public Vector3 jumperPosition;
    public Quaternion jumperRotation;

    public Vector2[] landingAreaPoints;
    public Vector2[] inrunPoints;
}


[System.Serializable]
public class ModelData
{
    public GameObject gObj;
    [HideInInspector]
    public Mesh mesh;
    public Material[] materials;
}

public enum TerrainBase
{
    currentTerrain,
    PerlinNoise,
    flat
};

public class MeshScript : MonoBehaviour
{
    [Header("Hill data")]
    public HillDataSerialization.ProfileData profileData;

    public Hill hill;

    /* Settings */
    [Space]
    [Header("Settings")]
    public bool generateTerrain;
    public bool generateGateStairsL;
    public bool generateGateStairsR;
    public bool generateInrunStairsL;
    public bool generateInrunStairsR;
    public bool generateLandingAreaGuardrailL;
    public bool generateLandingAreaGuardrailR;

    [Range(0.01f, 1)]
    public float inrunStairsStepHeigth;

    /* Hill profile */
    [Space]
    [Header("Hill profile")]
    public HillModel[] hills;
    public ModelData inrun;
    public ModelData landingArea;

    // /* Materials */
    // [Space]
    // [Header("Materials")]
    // public MaterialData[] materials;

    /* Inrun Track */

    public ModelData inrunTrack;

    /* Stairs */
    [Space]
    [Header("Stairs")]
    public ModelData gateStairsL;
    public ModelData gateStairsR;
    public ModelData inrunStairsL;
    public ModelData inrunStairsR;

    /* Guardrails */
    [Space]
    [Header("Guardrails")]
    public ModelData inrunGuardrailL;
    public ModelData inrunGuardrailR;
    public ModelData landingAreaGuardrailL;
    public ModelData landingAreaGuardrailR;
    public ModelData outrunGuardrail;

    /* Hill construction */
    [Space]
    [Header("Hill construction")]
    public GameObject startGate;
    public ModelData inrunConstruction;
    public GameObject lampPrefab;
    public GameObject inrunLampPrefab;

    /* Terrain */
    [Space]
    [Header("Terrain")]
    public GameObject terrainObject;
    private Terrain[] terrains;
    public LineRenderer[] lineRenderers;

    public Vector3 jumperPosition;
    public Quaternion jumperRotation;

    public Vector2[] landingAreaPoints;
    public Vector2[] inrunPoints;

    [Range(0, 1)]
    public float inrunTerrain;

    public TerrainBase terrainBase;

    //Lighting

    public Light sunLight;
    public Material daySkybox;
    public Material nightSkybox;
    public int time = 0;

    public List<int> igelitLines;


    private void Start()
    {
        // float[,] tab = new float[513, 513];
        // for (int i = 0; i < 513; i++)
        //     for (int j = 0; j < 513; j++)
        //     {
        //         tab[i, j] = 0;
        //     }

        // terrain1.terrainData.SetHeights(0, 0, tab);
        // // Debug.Log(terrain1.terrainData.size);
        // jumperPosition = new Vector3(0, 0, 0);
        // jumperRotation = new Quaternion();
    }

    public void SetGate(Hill hill, int nr)
    {
        jumperPosition = new Vector3(hill.GatePoint(nr).x, hill.GatePoint(nr).y, 0);
        jumperRotation.eulerAngles = new Vector3(0, 0, -hill.gamma);
    }

    public void GenerateMesh(Hill _hill)
    {
        hill = _hill;
        CalculateHill();

        GenerateInrun();
        GenerateInrunTrack();
        GenerateLandingAreaCollider();
        GenerateLandingArea();

        GenerateGateStairs(ref gateStairsL, 0, generateGateStairsL);
        GenerateGateStairs(ref gateStairsR, 1, generateGateStairsR);
        int stepsCount = Mathf.RoundToInt((hill.A.y - hill.T.y) / inrunStairsStepHeigth);
        inrunStairsStepHeigth = (hill.A.y - hill.T.y) / stepsCount;
        GenerateInrunStairs(ref inrunStairsL, 0, generateInrunStairsL, stepsCount);
        GenerateInrunStairs(ref inrunStairsR, 1, generateInrunStairsR, stepsCount);
        GenerateGuardrail(ref landingAreaGuardrailL, 0, generateLandingAreaGuardrailL, 1);
        GenerateGuardrail(ref landingAreaGuardrailR, 1, generateLandingAreaGuardrailR, 1);
        GenerateInrunConstruction();

        // inrunConstruction.GetComponent<MeshCollider>().sharedMesh = inrunConstructionMesh;

        //Gate


        /*
                //Barriers
                Vector3[] vertices3 = new Vector3[2 * 4 * (landingAreaPoints.Length + inrunPoints.Length)];
                // Vector2[] uv3 = new Vector2[2 * landingAreaPoints.Length];
                int[] triangles3 = new int[(landingAreaPoints.Length + inrunPoints.Length + 2) * 4 * 3 * 2 * 3];

                for (int i = 0; i < inrunPoints.Length; i++)
                {
                    // if (reverseGateStairs)
                    // {
                    //     if (inrunPoints[i].x >= hill.B.x)
                    //     {
                    //         vertices3[8 * i + 4] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, profileData.b1 / 2);
                    //         vertices3[8 * i + 5] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, profileData.b1 / 2);
                    //         vertices3[8 * i + 6] = new Vector3(inrunPoints[i].x, inrunPoints[i].y + 0.5f, profileData.b1 / 2);
                    //         vertices3[8 * i + 7] = new Vector3(inrunPoints[i].x, inrunPoints[i].y + 0.5f, profileData.b1 / 2);
                    //     }

                    //     vertices3[8 * i + 0] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, -profileData.b1 / 2);
                    //     vertices3[8 * i + 1] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, -(profileData.b1 / 2 - 0.2f));
                    //     vertices3[8 * i + 2] = new Vector3(inrunPoints[i].x, inrunPoints[i].y + 0.5f, -(profileData.b1 / 2));
                    //     vertices3[8 * i + 3] = new Vector3(inrunPoints[i].x, inrunPoints[i].y + 0.5f, -profileData.b1 / 2);

                    // }
                    // else
                    // {
                    if (inrunPoints[i].x >= hill.B.x)
                    {
                        vertices3[8 * i + 0] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, -profileData.b1 / 2);
                        vertices3[8 * i + 1] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, -(profileData.b1 / 2 - 0.2f));
                        vertices3[8 * i + 2] = new Vector3(inrunPoints[i].x, inrunPoints[i].y + 0.5f, -(profileData.b1 / 2));
                        vertices3[8 * i + 3] = new Vector3(inrunPoints[i].x, inrunPoints[i].y + 0.5f, -profileData.b1 / 2);
                    }

                    vertices3[8 * i + 4] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, profileData.b1 / 2);
                    vertices3[8 * i + 5] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, profileData.b1 / 2);
                    vertices3[8 * i + 6] = new Vector3(inrunPoints[i].x, inrunPoints[i].y + 0.5f, profileData.b1 / 2);
                    vertices3[8 * i + 7] = new Vector3(inrunPoints[i].x, inrunPoints[i].y + 0.5f, profileData.b1 / 2);
                    // }

                }

                for (int i = 0; i < landingAreaPoints.Length; i++)
                {
                    float b = landingAreaPoints[i].x <= hill.K.x ? (profileData.b2 / 2) + landingAreaPoints[i].x / hill.K.x * ((profileData.bK - profileData.b2) / 2) : landingAreaPoints[i].x >= hill.U.x ? (profileData.bU / 2) : (profileData.bK / 2) + (landingAreaPoints[i].x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.bU - profileData.bK) / 2);
                    vertices3[8 * (i + inrunPoints.Length) + 0] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -b);
                    vertices3[8 * (i + inrunPoints.Length) + 1] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -(b - 0.2f));
                    vertices3[8 * (i + inrunPoints.Length) + 2] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + 1, -(b - 0.2f));
                    vertices3[8 * (i + inrunPoints.Length) + 3] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + 1, -b);
                    vertices3[8 * (i + inrunPoints.Length) + 4] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, b - 0.2f);
                    vertices3[8 * (i + inrunPoints.Length) + 5] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, b);
                    vertices3[8 * (i + inrunPoints.Length) + 6] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + 1, b);
                    vertices3[8 * (i + inrunPoints.Length) + 7] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + 1, b - 0.2f);


                }


                int[] inrBarSeg = { 0, 11, 3, 0, 8, 11, 3, 10, 2, 3, 11, 10, 2, 10, 9, 2, 9, 1 };
                int[] lanBarSeg = { 0, 3, 11, 0, 11, 8, 3, 2, 10, 3, 10, 11, 2, 9, 10, 2, 1, 9 };

                for (int i = 0; i < inrunPoints.Length - 1; i++)
                {
                    if (inrunPoints[i + 1].x >= hill.B.x)
                    {
                        for (int j = 0; j < inrBarSeg.Length; j++)
                        {
                            triangles3[2 * inrBarSeg.Length * i + j] = 8 * i + inrBarSeg[j];
                        }
                    }

                    for (int j = 0; j < inrBarSeg.Length; j++)
                    {
                        triangles3[2 * inrBarSeg.Length * i + inrBarSeg.Length + j] = 8 * i + inrBarSeg[j] + 4;
                    }
                }

                for (int i = 0; i < landingAreaPoints.Length - 1; i++)
                {
                    for (int j = 0; j < lanBarSeg.Length; j++)
                    {
                        triangles3[2 * lanBarSeg.Length * (i + inrunPoints.Length - 1) + j] = 8 * (i + inrunPoints.Length) + lanBarSeg[j];
                    }
                    for (int j = 0; j < lanBarSeg.Length; j++)
                    {
                        triangles3[2 * lanBarSeg.Length * (i + inrunPoints.Length - 1) + lanBarSeg.Length + j] = 8 * (i + inrunPoints.Length) + lanBarSeg[j] + 4;
                    }
                }

                // int[] inrBarcaps = { 0, 1, 3, 0, 3, 2, 0, 2, 3, 0, 3, 1 };

                // for (int i = 0; i < 6; i++)
                // {
                //     inrunConstructionTriangles[segment.Length * (inrunPoints.Length - 1) + i] = caps[i];
                // }
                // for (int i = 6; i < 12; i++)
                // {
                //     inrunConstructionTriangles[segment.Length * (inrunPoints.Length - 1) + i] = 4 * (inrunPoints.Length - 1) + caps[i];
                // }
                // for (int i = 0; i < landingAreaPoints.Length - 1; i++)
                // {
                //     triangles3[18 * i + 0] = 4 * i + 0;
                //     triangles3[18 * i + 1] = 4 * i + 4;
                //     triangles3[18 * i + 2] = 4 * i + 1;
                //     triangles3[18 * i + 3] = 4 * i + 1;
                //     triangles3[18 * i + 4] = 4 * i + 4;
                //     triangles3[18 * i + 5] = 4 * i + 5;
                //     triangles3[18 * i + 6] = 4 * i + 1;
                //     triangles3[18 * i + 7] = 4 * i + 5;
                //     triangles3[18 * i + 8] = 4 * i + 2;
                //     triangles3[18 * i + 9] = 4 * i + 2;
                //     triangles3[18 * i + 10] = 4 * i + 5;
                //     triangles3[18 * i + 11] = 4 * i + 6;
                //     triangles3[18 * i + 12] = 4 * i + 2;
                //     triangles3[18 * i + 13] = 4 * i + 6;
                //     triangles3[18 * i + 14] = 4 * i + 3;
                //     triangles3[18 * i + 15] = 4 * i + 3;
                //     triangles3[18 * i + 16] = 4 * i + 6;
                //     triangles3[18 * i + 17] = 4 * i + 7;
                // }



                // for (int i = 0; i < inrunPoints.Length; i++)
                // {
                //     vertices3[4 * (i + landingAreaPoints.Length)] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - hill.s, -100);
                //     vertices3[4 * (i + landingAreaPoints.Length) + 1] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - hill.s, -2);
                //     vertices3[4 * (i + landingAreaPoints.Length) + 2] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - hill.s, 2);
                //     vertices3[4 * (i + landingAreaPoints.Length) + 3] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - hill.s, 100);
                // }

                // for (int i = 0; i < inrunPoints.Length - 1; i++)
                // {
                //     triangles3[12 * (i + landingAreaPoints.Length - 1)] = 4 * (i + landingAreaPoints.Length);
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 1] = 4 * (i + landingAreaPoints.Length) + 5;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 2] = 4 * (i + landingAreaPoints.Length) + 1;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 3] = 4 * (i + landingAreaPoints.Length);
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 4] = 4 * (i + landingAreaPoints.Length) + 4;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 5] = 4 * (i + landingAreaPoints.Length) + 5;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 6] = 4 * (i + landingAreaPoints.Length) + 2;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 7] = 4 * (i + landingAreaPoints.Length) + 7;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 8] = 4 * (i + landingAreaPoints.Length) + 3;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 9] = 4 * (i + landingAreaPoints.Length) + 2;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 10] = 4 * (i + landingAreaPoints.Length) + 6;
                //     triangles3[12 * (i + landingAreaPoints.Length - 1) + 11] = 4 * (i + landingAreaPoints.Length) + 7;
                // }

                terrainMesh.vertices = vertices3;
                terrainMesh.triangles = triangles3;
                terrain.GetComponent<MeshFilter>().mesh = terrainMesh;
                terrain.GetComponent<MeshRenderer>().material = terrainMaterial.material;
        */

        if (generateTerrain)
        {
            // Debug.Log("GENERATING TERRAIN");
            GetTerrain();
            // Transform hillTransform = GetComponent<Transform>().transform;
            const float offset = 300f;
            const float inrunFlatLength = 5f;
            //Terrain
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


                        float hillY = 0;
                        float b = 15;
                        float c = 0;

                        if (x < hill.A.x)
                        {
                            c = hill.A.x + inrunFlatLength - x;
                            hillY = hill.A.y * inrunTerrain - hill.s;
                        }
                        else if (x < hill.T.x)
                        {
                            hillY = hill.Inrun(x) * inrunTerrain - hill.s;
                            if (hill.A.x <= x) b = 15;
                        }
                        else if (hill.T.x <= x && x <= hill.U.x)
                        {
                            hillY = hill.LandingArea(x);
                            b = 15;
                        }
                        else if (x <= hill.U.x + hill.a)
                        {
                            hillY = hill.U.y;
                            b = 15;
                        }
                        else
                        {
                            c = x - hill.U.x - hill.a;
                            hillY = hill.U.y;
                        }

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
                        float blendFactor = Mathf.SmoothStep(0, 1, Mathf.Clamp01(new Vector2(Mathf.Clamp01((Mathf.Abs(z) - b) / offset), Mathf.Clamp01(c / offset)).magnitude));
                        float y = hillY * (1 - blendFactor) + terrainY * blendFactor;

                        // y += (Mathf.Abs((Mathf.Abs(z) - b)) <= 50 ? 2 * Mathf.Abs((Mathf.Abs(z) - b)) : 100) * 0.5f);


                        y = (y - center.y - 1) / terr.terrainData.size.y;


                        // if (i == 200 && j == 200) Debug.Log(x + " " + y);
                        // Debug.Log(x + " " + y);

                        tab[i, j] = Mathf.Clamp(y, 0, 1);
                    }
                }

                terr.terrainData.SetHeights(0, 0, tab);
            }
        }

        // Debug.Log(center);
        // Debug.Log(terrain1.terrainData.size);


        // center = terrain2.GetComponent<Transform>().position;
        // tab = new float[terrain1.terrainData.heightmapResolution, terrain1.terrainData.heightmapResolution];

        // for (int i = 0; i < terrain2.terrainData.heightmapResolution; i++)
        // {
        //     for (int j = 0; j < terrain2.terrainData.heightmapResolution; j++)
        //     {
        //         float x = (float)(j) / terrain2.terrainData.heightmapResolution * (terrain2.terrainData.size.x) + center.x;
        //         float z = (float)(i) / terrain2.terrainData.heightmapResolution * (terrain2.terrainData.size.z) + center.z;

        //         float y = 0;
        //         float b = 0;

        //         if (x < hill.T.x)
        //         {
        //             y = hill.Inrun(x) * inrunTerrain;
        //             if (hill.A.x <= x) b = 4;
        //         }
        //         else if (hill.T.x <= x && x <= hill.U.x)
        //         {
        //             y = hill.LandingArea(x);
        //             b = 25;
        //         }
        //         else if (x > hill.U.x)
        //         {
        //             y = hill.U.y;
        //             b = 25;
        //         }

        //         if (Mathf.Abs(z) - b > 0)
        //             y += (Mathf.Abs((Mathf.Abs(z) - b)) <= 50 ? 2 * Mathf.Abs((Mathf.Abs(z) - b)) : 100) * (Mathf.PerlinNoise(x / 100.0f + 2000, z / 100.0f + 2000) - 0.5f);
        //         y = (y - center.y - 1) / terrain2.terrainData.size.y;


        //         if (i == 200 && j == 200) Debug.Log(x + " " + y);
        //         // Debug.Log(x + " " + y);

        //         tab[i, j] = Mathf.Clamp(y, 0, 1);
        //     }
        // }

        // terrain2.terrainData.SetHeights(0, 0, tab);
        SaveMesh(inrun.gObj, "Inrun", true);
        SaveMesh(inrun.gObj, "InrunTrack");
        SaveMesh(landingArea.gObj, "LandingAreaCollider", true);
        SaveMesh(landingArea.gObj, "LandingArea");
        SaveMesh(gateStairsL.gObj, "GateStairsL");
        SaveMesh(gateStairsR.gObj, "GateStairsR");
        SaveMesh(inrunStairsL.gObj, "InrunStairsL");
        SaveMesh(inrunStairsR.gObj, "InrunStairsR");
        SaveMesh(landingAreaGuardrailL.gObj, "LandingAreaGuardrailL");
        SaveMesh(landingAreaGuardrailR.gObj, "LandingAreaGuardrailR");
        SaveMesh(inrunConstruction.gObj, "InrunConstruction");

        SetGate(hill, 1);

        lineRenderers[0].SetPosition(0, new Vector3(landingAreaPoints[Mathf.RoundToInt(hill.w)].x, landingAreaPoints[Mathf.RoundToInt(hill.w)].y, -7));
        lineRenderers[0].SetPosition(1, new Vector3(landingAreaPoints[Mathf.RoundToInt(hill.w)].x, landingAreaPoints[Mathf.RoundToInt(hill.w)].y, 7));
        lineRenderers[1].SetPosition(0, new Vector3(landingAreaPoints[Mathf.RoundToInt(hill.w + hill.l2)].x, landingAreaPoints[Mathf.RoundToInt(hill.w + hill.l2)].y, -10));
        lineRenderers[1].SetPosition(1, new Vector3(landingAreaPoints[Mathf.RoundToInt(hill.w + hill.l2)].x, landingAreaPoints[Mathf.RoundToInt(hill.w + hill.l2)].y, 10));
        lineRenderers[2].SetPosition(0, new Vector3(hill.U.x, hill.U.y, -12));
        lineRenderers[2].SetPosition(1, new Vector3(hill.U.x, hill.U.y, 12));

        DestroyLamps();
        if (time == 0)
        {
            sunLight.intensity = 0.1f;
            RenderSettings.skybox = nightSkybox;
            GenerateLamps();
        }
        else if (time == 1)
        {
            sunLight.intensity = 1f;
            RenderSettings.skybox = daySkybox;
        }


    }

    public void GetTerrain()
    {
        terrains = terrainObject.GetComponentsInChildren<Terrain>();
    }

    List<GameObject> lamps;

    public void CalculateHill()
    {
        hill.Calculate();
        inrunPoints = hill.InrunPoints();
        landingAreaPoints = hill.LandingAreaPoints(1000);
    }

    public void ObjectUpdate(ref GameObject gameObject, ref Mesh mesh, ref Material material, ref Vector3[] vertices, ref int[] triangles, ref Vector2[] uvs, bool hasCollider)
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = material;
        if (hasCollider)
        {
            gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    public int[] FacesToTriangles(ref List<Tuple<int, int, int, int>> facesList)
    {
        List<int> triangles = new List<int>();
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

    public void GenerateInrun()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[inrunPoints.Length * 2];
        Vector2[] uvs = new Vector2[inrunPoints.Length * 2];
        int[] triangles = new int[(inrunPoints.Length - 1) * 6];

        float[] len = new float[inrunPoints.Length];

        for (int i = 1; i < inrunPoints.Length; i++)
        {
            len[i] = len[i - 1] + (inrunPoints[i] - inrunPoints[i - 1]).magnitude;
        }

        for (int i = 0; i < inrunPoints.Length; i++)
        {
            vertices[2 * i] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, -profileData.b1 / 2);
            vertices[2 * i + 1] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, profileData.b1 / 2);
            uvs[2 * i] = new Vector2(len[i], -profileData.b1);
            uvs[2 * i + 1] = new Vector2(len[i], profileData.b1);
        }

        for (int i = 0; i < inrunPoints.Length - 1; i++)
        {
            triangles[6 * i + 0] = 2 * i + 0;
            triangles[6 * i + 1] = 2 * i + 3;
            triangles[6 * i + 2] = 2 * i + 1;
            triangles[6 * i + 3] = 2 * i + 0;
            triangles[6 * i + 4] = 2 * i + 2;
            triangles[6 * i + 5] = 2 * i + 3;
        }

        ObjectUpdate(ref inrun.gObj, ref mesh, ref inrun.materials[0], ref vertices, ref triangles, ref uvs, true);
        // inrun.gObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void GenerateInrunConstruction()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<Tuple<int, int, int, int>> facesList = new List<Tuple<int, int, int, int>>();

        const float width = 5f;
        int x = 0;
        float[] len = new float[inrunPoints.Length];
        for (int i = 1; i < inrunPoints.Length; i++)
        {
            len[i] = len[i - 1] + (inrunPoints[i] - inrunPoints[i - 1]).magnitude;
        }

        for (int i = 0; i < inrunPoints.Length; i++)
        {
            verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y, -2));
            uvsList.Add(new Vector2(inrunPoints[i].x, inrunPoints[i].y));
            verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y, 2));
            uvsList.Add(new Vector2(inrunPoints[i].x, inrunPoints[i].y));
            // verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y - 2 - (float)((inrunPoints.Length - i + 1) * (inrunPoints.Length - i + 1)) / inrunPoints.Length / inrunPoints.Length * 20.0f, -2));
            // uvsList.Add(new Vector2(i, 0));
            // verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y - 2 - (float)((inrunPoints.Length - i + 1) * (inrunPoints.Length - i + 1)) / inrunPoints.Length / inrunPoints.Length * 20.0f, 2));
            // uvsList.Add(new Vector2(i, 8));
            verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y - width, -2));
            uvsList.Add(new Vector2(inrunPoints[i].x, inrunPoints[i].y - width));
            verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y - width, 2));
            uvsList.Add(new Vector2(inrunPoints[i].x, inrunPoints[i].y - width));

            verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y - width, -2));
            uvsList.Add(new Vector2(inrunPoints[i].x, -2));
            verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y - width, 2));
            uvsList.Add(new Vector2(inrunPoints[i].x, 2));
            if (i > 0)
            {
                x = verticesList.Count;
                facesList.Add(Tuple.Create(x - 6, x - 12, x - 4, x - 10));
                facesList.Add(Tuple.Create(x - 11, x - 5, x - 9, x - 3));
                facesList.Add(Tuple.Create(x - 1, x - 2, x - 7, x - 8));
            }
        }
        // take-off table
        verticesList.Add(new Vector3(0, 0, -2));
        uvsList.Add(new Vector2(-2, 0));
        verticesList.Add(new Vector3(0, 0, 2));
        uvsList.Add(new Vector2(2, 0));
        verticesList.Add(new Vector3(0, -width, -2));
        uvsList.Add(new Vector2(-2, -width));
        verticesList.Add(new Vector3(0, -width, 2));
        uvsList.Add(new Vector2(2, -width));
        //top of the hill
        verticesList.Add(new Vector3(hill.A.x, hill.A.y, -2));
        uvsList.Add(new Vector2(-2, 0));
        verticesList.Add(new Vector3(hill.A.x, hill.A.y, 2));
        uvsList.Add(new Vector2(2, 0));
        verticesList.Add(new Vector3(hill.A.x, hill.A.y - width, -2));
        uvsList.Add(new Vector2(-2, -width));
        verticesList.Add(new Vector3(hill.A.x, hill.A.y - width, 2));
        uvsList.Add(new Vector2(2, -width));
        x = verticesList.Count;
        facesList.Add(Tuple.Create(x - 8, x - 7, x - 6, x - 5));
        facesList.Add(Tuple.Create(x - 3, x - 4, x - 1, x - 2));

        Vector3[] vertices = verticesList.ToArray();
        Vector2[] uvs = uvsList.ToArray();
        int[] triangles = FacesToTriangles(ref facesList);
        // Debug.Log("RAKAKANO: " + verticesList.Count + " " + uvsList.Count + " " + triangles.Length);
        ObjectUpdate(ref inrunConstruction.gObj, ref mesh, ref inrunConstruction.materials[0], ref vertices, ref triangles, ref uvs, false);
    }

    public void GenerateInrunTrack()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<Tuple<int, int, int, int>> facesList = new List<Tuple<int, int, int, int>>();

        float[] len = new float[inrunPoints.Length];

        for (int i = 1; i < inrunPoints.Length; i++)
        {
            len[i] = len[i - 1] + (inrunPoints[i] - inrunPoints[i - 1]).magnitude;
        }

        // float[] inrunTrackVals = new float[] { 0.27f, 0.225f, 0.09f, 0.035f, 0.036f, 0.001f };
        Vector2[] inrunTrackVals = new Vector2[] { new Vector2(profileData.b1 / 2, 0), new Vector2(0.27f, 0), new Vector2(0.27f, 0.05f), new Vector2(0.225f, 0.05f), new Vector2(0.225f, 0.001f), new Vector2(0.09f, 0.001f), new Vector2(0.09f, 0.05f) };
        Vector2 midPoint = new Vector2(0, 0.05f);
        float[] suff = new float[inrunTrackVals.Length];
        suff[inrunTrackVals.Length - 1] = (inrunTrackVals[inrunTrackVals.Length - 1] - midPoint).magnitude;
        for (int i = inrunTrackVals.Length - 2; i >= 0; i--)
        {
            suff[i] = (inrunTrackVals[i] - inrunTrackVals[i + 1]).magnitude + suff[i + 1];
        }

        for (int i = 0; i < inrunPoints.Length; i++)
        {
            for (int j = 0; j < inrunTrackVals.Length; j++)
            {
                verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y + inrunTrackVals[j].y, -inrunTrackVals[j].x));
                uvsList.Add(new Vector2(len[i], -suff[j]));
                if (j > 0)
                {
                    verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y + inrunTrackVals[j].y, -inrunTrackVals[j].x));
                    uvsList.Add(new Vector2(len[i], -suff[j]));
                }
            }
            for (int j = inrunTrackVals.Length - 1; j >= 0; j--)
            {
                verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y + inrunTrackVals[j].y, inrunTrackVals[j].x));
                uvsList.Add(new Vector2(len[i], suff[j]));
                if (j > 0)
                {
                    verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y + inrunTrackVals[j].y, inrunTrackVals[j].x));
                    uvsList.Add(new Vector2(len[i], suff[j]));
                }
            }

            if (i > 0)
            {
                int x = verticesList.Count;
                int y = 2 * inrunTrackVals.Length - 1;
                for (int j = 0; j < y; j++)
                {
                    facesList.Add(Tuple.Create(x - 2 * j - 2, x - 2 * j - 1, x - 2 * j - 2 - 2 * y, x - 2 * j - 1 - 2 * y));
                }
            }
        }


        Vector3[] vertices = verticesList.ToArray();
        Vector2[] uvs = uvsList.ToArray();
        int[] triangles = FacesToTriangles(ref facesList);

        ObjectUpdate(ref inrun.gObj, ref mesh, ref inrun.materials[0], ref vertices, ref triangles, ref uvs, false);
    }

    public void GenerateLandingAreaCollider()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        // 0 - green, 1 - white, 2 - blue, 3 - red
        List<Tuple<int, int, int, int>> facesList = new List<Tuple<int, int, int, int>>();

        float[] b = new float[landingAreaPoints.Length];

        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            b[i] = landingAreaPoints[i].x <= hill.K.x ? (profileData.b2 / 2) + landingAreaPoints[i].x / hill.K.x * ((profileData.bK - profileData.b2) / 2) :
                landingAreaPoints[i].x >= hill.U.x ? (profileData.bU / 2) : (profileData.bK / 2) + (landingAreaPoints[i].x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.bU - profileData.bK) / 2);
        }

        // vertices, uvs & triangles
        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -b[i]));
            uvsList.Add(new Vector2(i, -b[i]));

            verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, b[i]));
            uvsList.Add(new Vector2(i, b[i]));

            if (i > 0)
            {
                int x = verticesList.Count;
                facesList.Add(Tuple.Create(x - 4, x - 3, x - 2, x - 1));
            }
        }

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = FacesToTriangles(ref facesList);
        mesh.uv = uvsList.ToArray();
        landingArea.gObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    public void GenerateLandingArea()
    {
        const float lineWidth = 0.2f;
        const float sideLineWidth = 0.5f;
        const float whiteLineWidth = 3f;
        const int maxSL = 2;

        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        // 0 - green, 1 - white, 2 - blue, 3 - red
        List<Tuple<int, int, int, int>>[] facesList = new List<Tuple<int, int, int, int>>[4];
        for (int i = 0; i < 4; i++) facesList[i] = new List<Tuple<int, int, int, int>>();

        List<int> trianglesList = new List<int>();

        int[,] lines = new int[landingAreaPoints.Length, 3];
        int[,] lineColor = new int[landingAreaPoints.Length, 3];
        int[] linesMask = new int[landingAreaPoints.Length];

        int pLen = Mathf.RoundToInt(hill.w - hill.l1), kLen = Mathf.RoundToInt(hill.w), lLen = Mathf.RoundToInt(hill.w + hill.l2);

        // U point line
        int uLen = 0;
        while ((landingAreaPoints[uLen + 1] - hill.U).magnitude < (landingAreaPoints[uLen] - hill.U).magnitude) uLen++;

        // white lines
        foreach (var line in igelitLines)
        {
            linesMask[line - 1] |= (1 << 1);
            lines[line - 1, 2] |= (1 << 1);
            lines[line, 0] |= (1 << 1);
            lineColor[line, 0] |= (1 << 1);
        }

        // P point line
        linesMask[pLen - 1] |= (1 << 1);
        linesMask[pLen] |= (1 << 0);
        lineColor[pLen, 0] |= (1 << 1);
        lineColor[pLen, 1] |= (1 << 1);
        lines[pLen - 1, 2] = lines[pLen, 0] = lines[pLen, 1] = 0;
        // K point line
        linesMask[kLen - 1] |= (1 << 1);
        linesMask[kLen] |= (1 << 0);
        lineColor[kLen, 0] |= (1 << 1);
        lineColor[kLen, 1] |= (1 << 1);
        lines[kLen - 1, 2] = lines[kLen, 0] = lines[kLen, 1] = 0;
        // L point line
        linesMask[lLen - 1] |= (1 << 1);
        linesMask[lLen] |= (1 << 0);
        lineColor[lLen, 0] |= (1 << 1);
        lineColor[lLen, 1] |= (1 << 1);
        lines[lLen - 1, 2] = lines[lLen, 0] = lines[lLen, 1] = 0;
        // U point line
        Debug.Log("uLen:" + uLen);
        linesMask[uLen - 1] |= (1 << 1);
        // linesMask[uLen] |= (1 << 0);
        lineColor[uLen, 0] |= (1 << 1);
        // lineColor[uLen, 1] |= (1 << 1);
        lines[uLen - 1, 2] = lines[uLen, 0] = lines[uLen, 1] = 0;

        // sidelines from P to L
        for (int i = 3 * (pLen - 1) + 2; i <= 3 * lLen + 1; i++) lines[(i / 3), i % 3] |= (1 << 0);
        for (int i = 3 * kLen; i <= 3 * lLen + 1; i++) lineColor[(i / 3), i % 3] |= (1 << 0);


        List<float> b = new List<float>();
        List<float> d = new List<float>();
        List<Vector2> pts = new List<Vector2>();
        List<int> sideLines = new List<int>();
        List<int> colorList = new List<int>();

        for (int i = 0; i < landingAreaPoints.Length - 1; i++)
        {
            float b0 = landingAreaPoints[i].x <= hill.K.x ? (profileData.b2 / 2) + landingAreaPoints[i].x / hill.K.x * ((profileData.bK - profileData.b2) / 2) :
                landingAreaPoints[i].x >= hill.U.x ? (profileData.bU / 2) : (profileData.bK / 2) + (landingAreaPoints[i].x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.bU - profileData.bK) / 2);

            // |--|--------|--|--|---/
            // 0  1        2  0  1


            // level 0
            d.Add(i);
            b.Add(b0);
            pts.Add(landingAreaPoints[i]);

            sideLines.Add(lines[i, 0]);
            colorList.Add(lineColor[i, 0]);

            // break after last point
            if (i == landingAreaPoints.Length - 1) break;

            float b1 = landingAreaPoints[i + 1].x <= hill.K.x ? (profileData.b2 / 2) + landingAreaPoints[i + 1].x / hill.K.x * ((profileData.bK - profileData.b2) / 2) :
                landingAreaPoints[i + 1].x >= hill.U.x ? (profileData.bU / 2) : (profileData.bK / 2) + (landingAreaPoints[i + 1].x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.bU - profileData.bK) / 2);

            // level 1
            if ((linesMask[i] & (1 << 0)) != 0)
            {
                d.Add(i + lineWidth);
                b.Add(Mathf.Lerp(b0, b1, lineWidth));
                pts.Add(Vector2.Lerp(landingAreaPoints[i], landingAreaPoints[i + 1], lineWidth));

                sideLines.Add(lines[i, 1]);
                colorList.Add(lineColor[i, 1]);
            }

            // level 2
            if ((linesMask[i] & (1 << 1)) != 0)
            {
                d.Add(i + 1 - lineWidth);
                b.Add(Mathf.Lerp(b0, b1, 1 - lineWidth));
                pts.Add(Vector2.Lerp(landingAreaPoints[i], landingAreaPoints[i + 1], 1 - lineWidth));

                sideLines.Add(lines[i, 2]);
                colorList.Add(lineColor[i, 2]);
            }
        }

        // for (int i = 0; i < pts.Count; i++)
        // {
        //     Debug.Log(pts[i] + " " + b[i] + " " + d[i] + " " + sideLines[i]);
        // }

        // vertices, uvs & triangles
        for (int i = 0; i < pts.Count; i++)
        {
            verticesList.Add(new Vector3(pts[i].x, pts[i].y, -b[i]));
            uvsList.Add(new Vector2(d[i], -b[i]));

            if ((sideLines[i] & (1 << 0)) != 0)
            {
                verticesList.Add(new Vector3(pts[i].x, pts[i].y, -b[i] + sideLineWidth));
                uvsList.Add(new Vector2(d[i], -b[i] + sideLineWidth));
            }

            if ((sideLines[i] & (1 << 1)) != 0)
            {
                verticesList.Add(new Vector3(pts[i].x, pts[i].y, -whiteLineWidth));
                uvsList.Add(new Vector2(d[i], -whiteLineWidth));
                verticesList.Add(new Vector3(pts[i].x, pts[i].y, whiteLineWidth));
                uvsList.Add(new Vector2(d[i], whiteLineWidth));
            }

            if ((sideLines[i] & (1 << 0)) != 0)
            {
                verticesList.Add(new Vector3(pts[i].x, pts[i].y, b[i] - sideLineWidth));
                uvsList.Add(new Vector2(d[i], b[i] - sideLineWidth));
            }

            verticesList.Add(new Vector3(pts[i].x, pts[i].y, b[i]));
            uvsList.Add(new Vector2(d[i], b[i]));


            if (i > 0)
            {
                int x = verticesList.Count;
                int cnt0 = 2, cnt1 = 2;
                for (int j = 0; j < maxSL; j++)
                {
                    if (((1 << j) & sideLines[i - 1]) != 0) cnt0 += 2;
                    if (((1 << j) & sideLines[i]) != 0) cnt1 += 2;
                }

                int it0 = 0, it1 = 0, l0 = 0, l1 = 0, jj = -1, listId = 0;

                for (int j = 0; j < maxSL; j++)
                {
                    if (((1 << j) & sideLines[i - 1]) != 0) it0++;
                    if (((1 << j) & sideLines[i]) != 0) it1++;

                    if (((1 << j) & sideLines[i - 1] & sideLines[i]) != 0)
                    {
                        listId = 0;
                        if (j == 0) listId = 2 + (colorList[i] & 1);
                        facesList[listId].Add(Tuple.Create((x - cnt0 - cnt1) + l0, (x - cnt0 - cnt1) + it0, (x - cnt1) + l1, (x - cnt1) + it1));
                        facesList[listId].Add(Tuple.Create((x - cnt1 - 1) - it0, (x - cnt1 - 1) - l0, (x - 1) - it1, (x - 1) - l1));
                        l0 = it0; l1 = it1;
                        jj = j;
                    }
                }

                listId = 0;
                if ((colorList[i] & (1 << 1)) != 0)
                {
                    if (jj == 0) listId = 2 + (colorList[i] & 1);
                    else listId = 1;
                }
                // if (jj == 1 && (colorList[i] & (1 << 1)) != 0) listId = 1;
                // else if (jj == 0 && (colorList[i] & (1 << 1)) != 0) listId = 2 + (colorList[i] & 1);

                facesList[listId].Add(Tuple.Create((x - cnt0 - cnt1) + l0, (x - cnt1 - 1) - l0, (x - cnt1) + l1, (x - 1) - l1));
            }
        }

        mesh.vertices = verticesList.ToArray();
        mesh.uv = uvsList.ToArray();
        mesh.subMeshCount = 4;

        for (int i = 0; i < 4; i++)
        {
            mesh.SetTriangles(FacesToTriangles(ref facesList[i]), i);
            // Debug.Log("Submesh: " + i + " size: " + facesList[i].Count);
        }

        mesh.RecalculateNormals();
        landingArea.gObj.GetComponent<MeshFilter>().mesh = mesh;
        landingArea.gObj.GetComponent<MeshRenderer>().materials = landingArea.materials;
        // landingArea.gObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void GenerateGateStairs(ref ModelData gateStairs, int side, bool generate)
    {
        /* 0 - Left, 1 - Right */
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4 * (hill.gates + 2)];
        Vector2[] uvs = new Vector2[4 * (hill.gates + 2)];
        int[] triangles = new int[6 * (2 * (hill.gates + 2) - 1)];
        if (generate)
        {
            float width = 4f;
            float offset = ((side == 1) ? (width + profileData.b1) : 0);

            for (int i = 0; i < hill.gates + 2; i++)
            {
                Vector2 pos = hill.B + (hill.A - hill.B) * ((float)(i) / (float)(hill.gates - 1));
                Vector2 pos0 = hill.B + (hill.A - hill.B) * ((float)(i - 1) / (float)(hill.gates - 1));
                vertices[4 * i + 0] = new Vector3(pos0.x, pos.y, -(profileData.b1 / 2 + width) + offset);
                vertices[4 * i + 1] = new Vector3(pos0.x, pos.y, -profileData.b1 / 2 + offset);
                vertices[4 * i + 2] = new Vector3(pos.x, pos.y, -(profileData.b1 / 2 + width) + offset);
                vertices[4 * i + 3] = new Vector3(pos.x, pos.y, -profileData.b1 / 2 + offset);
                uvs[4 * i] = new Vector2(i, 0);
                uvs[4 * i + 1] = new Vector2(i, 1);
            }

            for (int i = 0; i < hill.gates + 1; i++)
            {
                triangles[6 * i + 0] = 4 * i + 0;
                triangles[6 * i + 1] = 4 * i + 3;
                triangles[6 * i + 2] = 4 * i + 1;
                triangles[6 * i + 3] = 4 * i + 0;
                triangles[6 * i + 4] = 4 * i + 2;
                triangles[6 * i + 5] = 4 * i + 3;
            }

            for (int i = 0; i < hill.gates; i++)
            {
                triangles[6 * (i + hill.gates + 1) + 0] = 4 * i + 2;
                triangles[6 * (i + hill.gates + 1) + 1] = 4 * i + 5;
                triangles[6 * (i + hill.gates + 1) + 2] = 4 * i + 3;
                triangles[6 * (i + hill.gates + 1) + 3] = 4 * i + 2;
                triangles[6 * (i + hill.gates + 1) + 4] = 4 * i + 4;
                triangles[6 * (i + hill.gates + 1) + 5] = 4 * i + 5;
            }
        }

        ObjectUpdate(ref gateStairs.gObj, ref mesh, ref gateStairs.materials[0], ref vertices, ref triangles, ref uvs, false);
    }

    public void GenerateInrunStairs(ref ModelData inrunStairs, int side, bool generate, int stepsNumber)
    {
        /* 0 - Left, 1 - Right */
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<int> trianglesList = new List<int>();

        List<Tuple<int, int, int, int>> facesList = new List<Tuple<int, int, int, int>>();

        if (generate)
        {

            float width = 1f;
            float heightDiff = hill.A.y - hill.T.y;
            Debug.Log(heightDiff);
            float stepHeight = heightDiff / stepsNumber;
            float offset = ((side == 1) ? (width + profileData.b1) : 0);
            int it = 0;
            Vector2[] pos = new Vector2[stepsNumber + 1];
            for (int i = 0; i < pos.Length; i++)
            {
                while (it < inrunPoints.Length - 2 && (inrunPoints[it + 1].y < i * stepHeight)) it++;

                pos[i] = new Vector2((stepHeight * i - inrunPoints[it].y) / (inrunPoints[it + 1].y - inrunPoints[it].y) * (inrunPoints[it + 1].x - inrunPoints[it].x) + inrunPoints[it].x, stepHeight * i);
            }

            for (int i = 0; i < pos.Length - 1; i++)
            {
                verticesList.Add(new Vector3(pos[i].x, pos[i].y, -(profileData.b1 / 2 + width) + offset));
                uvsList.Add(new Vector2(0, 0));
                verticesList.Add(new Vector3(pos[i].x, pos[i].y, -profileData.b1 / 2 + offset));
                uvsList.Add(new Vector2(0, width));

                float deltaY = pos[i + 1].y - pos[i].y, deltaX = pos[i + 1].x - pos[i].x;
                verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -(profileData.b1 / 2 + width) + offset));
                uvsList.Add(new Vector2(deltaY, 0));
                verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -profileData.b1 / 2 + offset));
                uvsList.Add(new Vector2(deltaY, width));


                verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -(profileData.b1 / 2 + width) + offset));
                uvsList.Add(new Vector2(deltaY, 0));
                verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -profileData.b1 / 2 + offset));
                uvsList.Add(new Vector2(deltaY, width));

                verticesList.Add(new Vector3(pos[i + 1].x, pos[i + 1].y, -(profileData.b1 / 2 + width) + offset));
                verticesList.Add(new Vector3(pos[i + 1].x, pos[i + 1].y, -profileData.b1 / 2 + offset));
                uvsList.Add(new Vector2(deltaY + deltaX, 0));
                uvsList.Add(new Vector2(deltaY + deltaX, width));

                int x = verticesList.Count;
                facesList.Add(Tuple.Create(x - 2, x - 1, x - 4, x - 3));
                facesList.Add(Tuple.Create(x - 6, x - 5, x - 8, x - 7));


                // if (i < pos.Length - 1)
                // {
                //     verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -(profileData.b1 / 2 + width) + offset));
                //     verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -profileData.b1 / 2 + offset));
                //     uvsList.Add(new Vector2(2 * i + 1, 0));
                //     uvsList.Add(new Vector2(2 * i + 1, 1));
                // }
            }


            // for (int i = 0; i < verticesList.Count / 2 - 1; i++)
            // {
            //     trianglesList.Add(2 * i + 0);
            //     trianglesList.Add(2 * i + 3);
            //     trianglesList.Add(2 * i + 1);
            //     trianglesList.Add(2 * i + 0);
            //     trianglesList.Add(2 * i + 2);
            //     trianglesList.Add(2 * i + 3);
            // }
        }

        Vector3[] vertices = verticesList.ToArray(); ;
        Vector2[] uvs = uvsList.ToArray();
        int[] triangles = FacesToTriangles(ref facesList);
        ObjectUpdate(ref inrunStairs.gObj, ref mesh, ref inrunStairs.materials[0], ref vertices, ref triangles, ref uvs, false);
    }

    public void GenerateGuardrail(ref ModelData guardrail, int side, bool generate, float height)
    {

        float width = 0.1f;
        int sgn = (side == 0 ? -1 : 1);
        float[] b = new float[landingAreaPoints.Length];
        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            b[i] = landingAreaPoints[i].x <= hill.K.x ? (profileData.b2 / 2) + landingAreaPoints[i].x / hill.K.x * ((profileData.bK - profileData.b2) / 2) :
              landingAreaPoints[i].x >= hill.U.x ? (profileData.bU / 2) : (profileData.bK / 2) + (landingAreaPoints[i].x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.bU - profileData.bK) / 2);
        }
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<Tuple<int, int, int, int>> facesList = new List<Tuple<int, int, int, int>>();

        const float toTerrain = 2f;
        if (generate)
        {
            for (int i = 0; i < landingAreaPoints.Length; i++)
            {
                verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y - toTerrain, sgn * (b[i] + width) - width));
                uvsList.Add(new Vector2(i, 0));

                verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + height, sgn * (b[i] + width) - width));
                uvsList.Add(new Vector2(i, height + toTerrain));
                verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + height, sgn * (b[i] + width) - width));
                uvsList.Add(new Vector2(i, height + toTerrain));

                verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + height, sgn * (b[i] + width) + width));
                uvsList.Add(new Vector2(i, height + width + toTerrain));
                verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + height, sgn * (b[i] + width) + width));
                uvsList.Add(new Vector2(i, height + width + toTerrain));

                verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y - toTerrain, sgn * (b[i] + width) + width));
                uvsList.Add(new Vector2(i, height + width + height + toTerrain + toTerrain));

                if (i > 0)
                {
                    int x = verticesList.Count;
                    facesList.Add(Tuple.Create(x - 12, x - 11, x - 6, x - 5));
                    facesList.Add(Tuple.Create(x - 10, x - 9, x - 4, x - 3));
                    facesList.Add(Tuple.Create(x - 8, x - 7, x - 2, x - 1));
                    // facesList.Add(Tuple.Create(x - 8, x - 7, x - 4, x - 3));
                    // facesList.Add(Tuple.Create(x - 7, x - 6, x - 3, x - 2));
                    // facesList.Add(Tuple.Create(x - 6, x - 5, x - 2, x - 1));
                }
            }
        }



        // for (int i = 0; i < facesList.Count; i++)
        // {
        //     Debug.Log("verticesList[" + i + "] = " + verticesList[i]);
        // }
        Vector3[] vertices = verticesList.ToArray();
        int[] triangles = FacesToTriangles(ref facesList);
        Vector2[] uvs = uvsList.ToArray();
        ObjectUpdate(ref guardrail.gObj, ref mesh, ref guardrail.materials[0], ref vertices, ref triangles, ref uvs, false);
    }
    public void DestroyLamps()
    {
        if (lamps != null)
            foreach (var it in lamps)
            {
                Destroy(it);
            }

        lamps = new List<GameObject>();
    }

    public void GenerateLamps()
    {
        for (int i = 0; i < inrunPoints.Length; i += 5)
        {
            lamps.Add(Instantiate(inrunLampPrefab, new Vector3(inrunPoints[i].x, inrunPoints[i].y, 2), Quaternion.identity));
            // lamps.Add(Instantiate(inrunLampPrefab, new Vector3(inrunPoints[i].x, inrunPoints[i].y + 1f, -2), Quaternion.identity));
        }

        for (int i = 0; i < landingAreaPoints.Length; i += 80)
        {
            lamps.Add(Instantiate(lampPrefab, new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, 45), Quaternion.identity));
        }
    }



    public void SaveMesh(GameObject gObj, string name, bool isCollider = false)
    {
#if UNITY_EDITOR
        string saveName = profileData.name + "_" + name;
        if (isCollider)
        {
            MeshCollider mc = gObj.GetComponent<MeshCollider>();
            if (mc)
            {
                string savePath = "Assets/" + saveName + ".asset";
                Debug.Log("Saved Mesh to:" + savePath);
                AssetDatabase.CreateAsset(mc.sharedMesh, savePath);
            }
        }
        else
        {
            MeshFilter mf = gObj.GetComponent<MeshFilter>();
            if (mf)
            {
                string savePath = "Assets/" + saveName + ".asset";
                Debug.Log("Saved Mesh to:" + savePath);
                AssetDatabase.CreateAsset(mf.mesh, savePath);
            }
        }

#endif
    }
}