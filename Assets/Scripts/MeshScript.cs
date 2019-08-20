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
public class MaterialData
{
    public Vector2 dim;
    public Material material;
}

[System.Serializable]
public class ModelData
{
    public GameObject gObj;
    [HideInInspector]
    public Mesh mesh;
    public MaterialData materialData;
}
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

        inrunConstruction.mesh = new Mesh();

        GenerateInrun();
        GenerateLandingArea();

        // GenerateOutrun();
        GenerateGateStairs(ref gateStairsL, 0, generateGateStairsL);
        GenerateGateStairs(ref gateStairsR, 1, generateGateStairsR);
        int stepsCount = Mathf.RoundToInt((hill.A.y - hill.T.y) / inrunStairsStepHeigth);
        inrunStairsStepHeigth = (hill.A.y - hill.T.y) / stepsCount;
        GenerateInrunStairs(ref inrunStairsL, 0, generateInrunStairsL, stepsCount);
        GenerateInrunStairs(ref inrunStairsR, 1, generateInrunStairsR, stepsCount);


        Vector3[] inrunConstructionVertices = new Vector3[inrunPoints.Length * 4];
        Vector2[] inrunConstructionUv = new Vector2[inrunPoints.Length * 4];


        for (int i = 0; i < inrunPoints.Length; i++)
        {
            inrunConstructionVertices[4 * i] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, -2);
            inrunConstructionVertices[4 * i + 1] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, 2);
            inrunConstructionVertices[4 * i + 2] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - 2 - (float)((inrunPoints.Length - i + 1) * (inrunPoints.Length - i + 1)) / inrunPoints.Length / inrunPoints.Length * 20.0f, -2);
            inrunConstructionVertices[4 * i + 3] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - 2 - (float)((inrunPoints.Length - i + 1) * (inrunPoints.Length - i + 1)) / inrunPoints.Length / inrunPoints.Length * 20.0f, 2);

            inrunConstructionUv[4 * i] = new Vector2(i, 2);
            inrunConstructionUv[4 * i + 1] = new Vector2(i, 6);
            inrunConstructionUv[4 * i + 2] = new Vector2(i, 0);
            inrunConstructionUv[4 * i + 3] = new Vector2(i, 8);
        }

        int[] segment = { 0, 2, 6, 0, 6, 4, 2, 7, 6, 2, 3, 7, 1, 5, 7, 1, 7, 3 };

        int[] inrunConstructionTriangles = new int[(inrunPoints.Length - 1) * segment.Length + 12];

        for (int i = 0; i < inrunPoints.Length - 1; i++)
        {
            for (int j = 0; j < segment.Length; j++)
            {
                inrunConstructionTriangles[segment.Length * i + j] = 4 * i + segment[j];
            }
        }

        int[] caps = { 0, 1, 3, 0, 3, 2, 0, 2, 3, 0, 3, 1 };

        for (int i = 0; i < 6; i++)
        {
            inrunConstructionTriangles[segment.Length * (inrunPoints.Length - 1) + i] = caps[i];
        }
        for (int i = 6; i < 12; i++)
        {
            inrunConstructionTriangles[segment.Length * (inrunPoints.Length - 1) + i] = 4 * (inrunPoints.Length - 1) + caps[i];
        }

        Debug.Log(inrunConstructionTriangles);

        inrunConstruction.mesh.vertices = inrunConstructionVertices;
        inrunConstruction.mesh.triangles = inrunConstructionTriangles;
        inrunConstruction.mesh.uv = inrunConstructionUv;
        inrunConstruction.mesh.RecalculateNormals();

        inrunConstruction.gObj.GetComponent<MeshFilter>().mesh = inrunConstruction.mesh;
        inrunConstruction.gObj.GetComponent<MeshRenderer>().material = inrunConstruction.materialData.material;
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
            GetTerrain();
            //Terrain
            foreach (var terr in terrains)
            {
                Vector3 center = terr.GetComponent<Transform>().position;
                float[,] tab = new float[terr.terrainData.heightmapResolution, terr.terrainData.heightmapResolution];
                for (int i = 0; i < terr.terrainData.heightmapResolution; i++)
                {
                    for (int j = 0; j < terr.terrainData.heightmapResolution; j++)
                    {
                        float x = (float)(j) / terr.terrainData.heightmapResolution * (terr.terrainData.size.x) + center.x;
                        float z = (float)(i) / terr.terrainData.heightmapResolution * (terr.terrainData.size.z) + center.z;

                        float y = 0;
                        float b = 0;

                        if (x < hill.T.x)
                        {
                            y = hill.Inrun(x) * inrunTerrain;
                            if (hill.A.x <= x) b = 4;
                        }
                        else if (hill.T.x <= x && x <= hill.U.x)
                        {
                            y = hill.LandingArea(x);
                            b = 25;
                        }
                        else if (x > hill.U.x)
                        {
                            y = hill.U.y;
                            b = 25;
                        }

                        if (Mathf.Abs(z) - b > 0)
                            y += (Mathf.Abs((Mathf.Abs(z) - b)) <= 50 ? 2 * Mathf.Abs((Mathf.Abs(z) - b)) : 100) * (Mathf.PerlinNoise(x / 100.0f + 2000, z / 100.0f + 2000) - 0.5f);
                        y = (y - center.y - 1) / terr.terrainData.size.y;


                        if (i == 200 && j == 200) Debug.Log(x + " " + y);
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
        SaveMesh(inrun.gObj, "Inrun");
        SaveMesh(landingArea.gObj, "LandingArea");
        SaveMesh(gateStairsL.gObj, "GateStairsL");
        SaveMesh(gateStairsR.gObj, "GateStairsR");
        SaveMesh(inrunStairsL.gObj, "InrunStairsL");
        SaveMesh(inrunStairsR.gObj, "InrunStairsR");

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
            uvs[2 * i] = new Vector2(len[i] / inrun.materialData.dim.x, (inrun.materialData.dim.y - profileData.b1) / 2 / inrun.materialData.dim.y);
            uvs[2 * i + 1] = new Vector2(len[i] / inrun.materialData.dim.x, (inrun.materialData.dim.y - (inrun.materialData.dim.y - profileData.b1) / 2) / inrun.materialData.dim.y);
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

        ObjectUpdate(ref inrun.gObj, ref mesh, ref inrun.materialData.material, ref vertices, ref triangles, ref uvs, true);
    }

    public void GenerateInrunTrack()
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

            uvs[2 * i] = new Vector2(len[i] / inrun.materialData.dim.x, (inrun.materialData.dim.y - profileData.b1) / 2 / inrun.materialData.dim.y);
            uvs[2 * i + 1] = new Vector2(len[i] / inrun.materialData.dim.x, (inrun.materialData.dim.y - (inrun.materialData.dim.y - profileData.b1) / 2) / inrun.materialData.dim.y);
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

        ObjectUpdate(ref inrun.gObj, ref mesh, ref inrun.materialData.material, ref vertices, ref triangles, ref uvs, true);
    }

    public void GenerateLandingArea()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<int> trianglesList = new List<int>();

        // float[] len = new float[landingAreaPoints.Length];

        // for (int i = 1; i < landingAreaPoints.Length; i++)
        // {
        //     len[i] = len[i - 1] + Mathf.Round((landingAreaPoints[i] - landingAreaPoints[i - 1]).magnitude);
        // }

        float[] b = new float[landingAreaPoints.Length];

        int faceCount = 0;

        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            b[i] = landingAreaPoints[i].x <= hill.K.x ? (profileData.b2 / 2) + landingAreaPoints[i].x / hill.K.x * ((profileData.bK - profileData.b2) / 2) : landingAreaPoints[i].x >= hill.U.x ? (profileData.bU / 2) : (profileData.bK / 2) + (landingAreaPoints[i].x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.bU - profileData.bK) / 2);
        }

        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            
        }

        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            bool sideLines = (igelitLines[i] & (1 << 0)) == (1 << 0);

            verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -b[i]));
            verticesList.Add(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, b[i]));
            uvsList.Add(new Vector2(i /* / landingArea.materialData.dim.x*/, (profileData.bU / 2 - b[i]) / landingArea.materialData.dim.y));
            uvsList.Add(new Vector2(i /* / landingArea.materialData.dim.x*/, (profileData.bU / 2 + b[i]) / landingArea.materialData.dim.y));

            if ((igelitLines[i] & (1 << 1)) == (1 << 1))
            {
                uvsList.Add(new Vector2(i + 0.2f /* / landingArea.materialData.dim.x*/, (profileData.bU / 2 - Mathf.Lerp(b[i], b[i+1], 0.2f)) / landingArea.materialData.dim.y));
                uvsList.Add(new Vector2(i + 0.2f /* / landingArea.materialData.dim.x*/, (profileData.bU / 2 + Mathf.Lerp(b[i], b[i+1], 0.2f)) / landingArea.materialData.dim.y));
                verticesList.Add(Vector3.Lerp(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -b[i]), new Vector3(landingAreaPoints[i + 1].x, landingAreaPoints[i + 1].y, -b[i + 1]), 0.2f));
                verticesList.Add(Vector3.Lerp(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, b[i]), new Vector3(landingAreaPoints[i + 1].x, landingAreaPoints[i + 1].y, b[i + 1]), 0.2f));
            }

            if ((igelitLines[i] & (1 << 2)) == (1 << 2))
            {
                uvsList.Add(new Vector2(i + 0.8f /* / landingArea.materialData.dim.x*/, (profileData.bU / 2 - Mathf.Lerp(b[i], b[i+1], 0.8f)) / landingArea.materialData.dim.y));
                uvsList.Add(new Vector2(i + 0.8f /* / landingArea.materialData.dim.x*/, (profileData.bU / 2 + Mathf.Lerp(b[i], b[i+1], 0.8f)) / landingArea.materialData.dim.y));
                verticesList.Add(Vector3.Lerp(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -b[i]), new Vector3(landingAreaPoints[i + 1].x, landingAreaPoints[i + 1].y, -b[i + 1]), 0.8f));
                verticesList.Add(Vector3.Lerp(new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, b[i]), new Vector3(landingAreaPoints[i + 1].x, landingAreaPoints[i + 1].y, b[i + 1]), 0.8f));
            }
        }

        for (int i = 0; i < faceCount - 1; i++)
        {
            trianglesList.Add(2 * i + 0);
            trianglesList.Add(2 * i + 1);
            trianglesList.Add(2 * i + 2);
            trianglesList.Add(2 * i + 1);
            trianglesList.Add(2 * i + 3);
            trianglesList.Add(2 * i + 2);
        }

        Vector3[] vertices = verticesList.ToArray();
        Vector2[] uvs = uvsList.ToArray();
        int[] triangles = trianglesList.ToArray();

        ObjectUpdate(ref landingArea.gObj, ref mesh, ref landingArea.materialData.material, ref vertices, ref triangles, ref uvs, true);
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

        ObjectUpdate(ref gateStairs.gObj, ref mesh, ref gateStairs.materialData.material, ref vertices, ref triangles, ref uvs, false);
    }

    public void GenerateInrunStairs(ref ModelData inrunStairs, int side, bool generate, int stepsNumber)
    {
        /* 0 - Left, 1 - Right */
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<int> trianglesList = new List<int>();

        Vector3[] vertices = new Vector3[0];
        Vector2[] uvs = new Vector2[0];
        int[] triangles = new int[0];

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

            for (int i = 0; i < pos.Length; i++)
            {
                verticesList.Add(new Vector3(pos[i].x, pos[i].y, -(profileData.b1 / 2 + width) + offset));
                verticesList.Add(new Vector3(pos[i].x, pos[i].y, -profileData.b1 / 2 + offset));
                uvsList.Add(new Vector2(2 * i, 0));
                uvsList.Add(new Vector2(2 * i, 1));

                if (i < pos.Length - 1)
                {
                    verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -(profileData.b1 / 2 + width) + offset));
                    verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -profileData.b1 / 2 + offset));
                    uvsList.Add(new Vector2(2 * i + 1, 0));
                    uvsList.Add(new Vector2(2 * i + 1, 1));
                }
            }

            vertices = verticesList.ToArray();

            for (int i = 0; i < verticesList.Count / 2 - 1; i++)
            {
                trianglesList.Add(2 * i + 0);
                trianglesList.Add(2 * i + 3);
                trianglesList.Add(2 * i + 1);
                trianglesList.Add(2 * i + 0);
                trianglesList.Add(2 * i + 2);
                trianglesList.Add(2 * i + 3);
            }

            triangles = trianglesList.ToArray();
            uvs = uvsList.ToArray();
        }

        ObjectUpdate(ref inrunStairs.gObj, ref mesh, ref inrunStairs.materialData.material, ref vertices, ref triangles, ref uvs, false);
    }

    // public void GenerateOutrun()
    // {
    //     Mesh mesh = new Mesh();
    //     Vector2[] outrunPoints = { landingAreaPoints[landingAreaPoints.Length-1], new Vector2(hill.U.x + 100.0f, hill.U.y) };
    //     Vector3[] vertices = new Vector3[outrunPoints.Length * 2];
    //     Vector2[] uvs = new Vector2[outrunPoints.Length * 2];
    //     int[] triangles = new int[(outrunPoints.Length - 1) * 6];

    //     for (int i = 0; i < outrunPoints.Length; i++)
    //     {
    //         vertices[2 * i + 0] = new Vector3(outrunPoints[i].x, outrunPoints[i].y, -12);
    //         vertices[2 * i + 1] = new Vector3(outrunPoints[i].x, outrunPoints[i].y, 12);
    //         uvs[2 * i + 0] = new Vector2(i, 0);
    //         uvs[2 * i + 1] = new Vector2(i, 20);
    //     }

    //     for (int i = 0; i < outrunPoints.Length - 1; i++)
    //     {
    //         triangles[6 * i + 0] = 2 * i + 0;
    //         triangles[6 * i + 1] = 2 * i + 1;
    //         triangles[6 * i + 2] = 2 * i + 2;
    //         triangles[6 * i + 3] = 2 * i + 1;
    //         triangles[6 * i + 4] = 2 * i + 3;
    //         triangles[6 * i + 5] = 2 * i + 2;
    //     }

    //     ObjectUpdate(ref outrun, ref mesh, ref terrainMaterial, ref vertices, ref triangles, ref uvs, true);
    // }

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



    public void SaveMesh(GameObject gObj, string name)
    {
#if UNITY_EDITOR
        string saveName = profileData.name + "_" + name;
        MeshFilter mf = gObj.GetComponent<MeshFilter>();
        if (mf)
        {
            string savePath = "Assets/" + saveName + ".asset";
            Debug.Log("Saved Mesh to:" + savePath);
            AssetDatabase.CreateAsset(mf.mesh, savePath);
        }
#endif
    }
}
