using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[System.Serializable]
public class HillModel
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
}


[System.Serializable]
public class ModelData
{
    public GameObject gObj;
    [HideInInspector]
    public Mesh mesh;
    public Material[] materials;
}

public enum TerrainBase { currentTerrain, PerlinNoise, flat };

public class MeshScript : MonoBehaviour
{
    [Header("Hill data")]
    public bool saveMesh;
    public InrunTrack inrunTrackSO;
    public GateStairs gateStairsSO;
    public LandingArea landingAreaSO;
    public Guardrail inrunGuardrailSO;
    public Guardrail InrunOuterGuardrailSO;
    public Guardrail LandingAreaGuardrailSO;

    public HillProfileVariable profileData;

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

    [Range(0.0001f, 1)]
    public float inrunStairsAngle;

    /* Hill profile */
    [Space]
    [Header("Hill profile")]
    public HillModel[] hills;
    public ModelData inrun;
    public ModelData landingArea;


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
    public ModelData inrunOuterGuardrailL;
    public ModelData inrunOuterGuardrailR;
    public ModelData landingAreaGuardrailL;
    public ModelData landingAreaGuardrailR;
    public ModelData outrunGuardrail;

    /* Hill construction */
    [Space]
    [Header("Hill construction")]
    public GameObject startGate;
    public ModelData inrunConstruction;
    public ModelData digitsMarks;
    public GameObject lampPrefab;
    public GameObject inrunLampPrefab;

    /* Terrain */
    [Space]
    [Header("Terrain")]
    public GameObject terrainObject;
    private Terrain[] terrains;
    public Vector3 jumperPosition;
    public Quaternion jumperRotation;


    [Range(0, 1)]
    public float inrunTerrain;


    public TerrainBase terrainBase;

    //Lighting

    public Light sunLight;
    public Material daySkybox;
    public Material nightSkybox;
    public int time = 0;

    public void SetGate(Hill hill, int nr)
    {
        jumperPosition = new Vector3(hill.GatePoint(nr).x, hill.GatePoint(nr).y, 0);
        jumperRotation.eulerAngles = new Vector3(0, 0, -hill.gamma);
    }

    public void GenerateMesh()
    {
        hill.SetValues(profileData.Value);
        inrunTerrain = profileData.Value.terrainSteepness;

        GenerateInrunCollider();
        GenerateInrunTrack();
        GenerateLandingAreaCollider();
        GenerateLandingArea();

        GenerateGateStairs(gateStairsL, 0, generateGateStairsL);
        GenerateGateStairs(gateStairsR, 1, generateGateStairsR);
        int stepsCount = Mathf.RoundToInt((hill.A.y - hill.T.y) / inrunStairsStepHeigth);
        inrunStairsStepHeigth = (hill.A.y - hill.T.y) / stepsCount;
        GenerateInrunStairs(inrunStairsL, 0, generateInrunStairsL, generateGateStairsL, stepsCount);
        GenerateInrunStairs(inrunStairsR, 1, generateInrunStairsR, generateGateStairsR, stepsCount);
        GenerateLandingAreaGuardrail(landingAreaGuardrailL, 0, generateLandingAreaGuardrailL);
        GenerateLandingAreaGuardrail(landingAreaGuardrailR, 1, generateLandingAreaGuardrailR);
        GenerateInrunGuardrail(inrunGuardrailL, 0, true);
        GenerateInrunGuardrail(inrunGuardrailR, 1, true);
        GenerateInrunOuterGuardrail(inrunOuterGuardrailL, 0, true, generateGateStairsL);
        GenerateInrunOuterGuardrail(inrunOuterGuardrailR, 1, true, generateGateStairsR);
        GenerateInrunConstruction();
        GenerateMarks();

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

        if (saveMesh)
        {
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
            SaveMesh(digitsMarks.gObj, "DigitsMarks");
        }

        SetGate(hill, 1);
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

    public void ObjectUpdate(GameObject gameObject, Mesh mesh, Material material, Vector3[] vertices, int[] triangles, Vector2[] uvs, bool hasCollider)
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

    public int[] FacesToTriangles(List<(int, int, int, int)> facesList)
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

    public void GenerateInrunCollider()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[hill.inrunPoints.Length * 2];
        Vector2[] uvs = new Vector2[hill.inrunPoints.Length * 2];
        int[] triangles = new int[(hill.inrunPoints.Length - 1) * 6];

        float[] len = new float[hill.inrunPoints.Length];

        for (int i = 1; i < hill.inrunPoints.Length; i++)
        {
            len[i] = len[i - 1] + (hill.inrunPoints[i] - hill.inrunPoints[i - 1]).magnitude;
        }

        for (int i = 0; i < hill.inrunPoints.Length; i++)
        {
            vertices[2 * i] = new Vector3(hill.inrunPoints[i].x, hill.inrunPoints[i].y, -profileData.Value.b1 / 2);
            vertices[2 * i + 1] = new Vector3(hill.inrunPoints[i].x, hill.inrunPoints[i].y, profileData.Value.b1 / 2);
            uvs[2 * i] = new Vector2(len[i], -profileData.Value.b1);
            uvs[2 * i + 1] = new Vector2(len[i], profileData.Value.b1);
        }

        for (int i = 0; i < hill.inrunPoints.Length - 1; i++)
        {
            triangles[6 * i + 0] = 2 * i + 0;
            triangles[6 * i + 1] = 2 * i + 3;
            triangles[6 * i + 2] = 2 * i + 1;
            triangles[6 * i + 3] = 2 * i + 0;
            triangles[6 * i + 4] = 2 * i + 2;
            triangles[6 * i + 5] = 2 * i + 3;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        inrun.gObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void GenerateLandingAreaCollider()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<(int, int, int, int)> facesList = new List<(int, int, int, int)>();

        float[] b = new float[hill.landingAreaPoints.Length];

        for (int i = 0; i < hill.landingAreaPoints.Length; i++)
        {
            b[i] = hill.landingAreaPoints[i].x <= hill.K.x ? (profileData.Value.b2 / 2) + hill.landingAreaPoints[i].x / hill.K.x * ((profileData.Value.bK - profileData.Value.b2) / 2) :
                hill.landingAreaPoints[i].x >= hill.U.x ? (profileData.Value.bU / 2) : (profileData.Value.bK / 2) + (hill.landingAreaPoints[i].x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.Value.bU - profileData.Value.bK) / 2);
        }

        for (int i = 0; i < hill.landingAreaPoints.Length; i++)
        {
            verticesList.Add(new Vector3(hill.landingAreaPoints[i].x, hill.landingAreaPoints[i].y, -b[i]));
            uvsList.Add(new Vector2(i, -b[i]));

            verticesList.Add(new Vector3(hill.landingAreaPoints[i].x, hill.landingAreaPoints[i].y, b[i]));
            uvsList.Add(new Vector2(i, b[i]));

            if (i > 0)
            {
                int x = verticesList.Count;
                facesList.Add((x - 4, x - 3, x - 2, x - 1));
            }
        }

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = FacesToTriangles(facesList);
        mesh.uv = uvsList.ToArray();
        landingArea.gObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void GenerateInrunConstruction()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<(int, int, int, int)> facesList = new List<(int, int, int, int)>();

        // const float width = 5f;
        // Func<float, float> width = (xx => 5f);

        Func<float, float> width = (xx => 0.5f / hill.A.y / hill.A.y * (xx - hill.A.x) * (xx - hill.A.x) * (xx - hill.A.x) + 3f);

        float criticalPointX = Mathf.Lerp(hill.GatePoint(-1).x, hill.T.x, inrunStairsAngle);
        Vector2 p1 = hill.inrunPoints.Last(it => it.x > criticalPointX);
        Vector2 p2 = hill.inrunPoints.First(it => it.x <= criticalPointX);
        Vector2 criticalPoint = Vector2.Lerp(p1, p2, (criticalPointX - p1.x) / (p2.x - p1.x));


        int x = 0;
        List<Vector2> tmpList = new List<Vector2>();
        tmpList.AddRange(hill.inrunPoints.Where(it => it.x > criticalPoint.x));
        tmpList.Add(criticalPoint);
        tmpList.AddRange(hill.inrunPoints.Where(it => it.x <= criticalPoint.x && it.x > hill.GatePoint(-1).x));
        tmpList.Add(hill.GatePoint(-1));
        tmpList.AddRange(hill.inrunPoints.Where(it => it.x <= hill.GatePoint(-1).x));

        float[] len = new float[tmpList.Count];
        float[] b0 = (!generateGateStairsL ? tmpList.Select(it => -(hill.b1 / 2 + 0.7f)).ToArray() : tmpList.Select(it => -(it.x > hill.GatePoint(-1).x ? (it.x > criticalPoint.x ? hill.b1 / 2 + 0.7f : Mathf.Lerp(hill.b1 / 2 + 0.7f, hill.b1 / 2 + gateStairsSO.StepWidth, (it.x - criticalPointX) / (criticalPointX - hill.GatePoint(-1).x))) : (hill.b1 / 2 + gateStairsSO.StepWidth))).ToArray());
        float[] b1 = (!generateGateStairsR ? tmpList.Select(it => hill.b1 / 2 + 0.7f).ToArray() : tmpList.Select(it => (it.x > hill.GatePoint(-1).x ? (it.x > criticalPoint.x ? hill.b1 / 2 + 0.7f : Mathf.Lerp(hill.b1 / 2 + 0.7f, hill.b1 / 2 + gateStairsSO.StepWidth, (it.x - criticalPointX) / (criticalPointX - hill.GatePoint(-1).x))) : (hill.b1 / 2 + gateStairsSO.StepWidth))).ToArray());

        for (int i = 1; i < tmpList.Count; i++)
        {
            len[i] = len[i - 1] + (tmpList[i] - tmpList[i - 1]).magnitude;
        }

        for (int i = 0; i < tmpList.Count; i++)
        {
            int tmp = verticesList.Count;
            verticesList.Add(new Vector3(tmpList[i].x, tmpList[i].y - 0.1f, b0[i]));
            uvsList.Add(new Vector2(tmpList[i].x, tmpList[i].y));
            verticesList.Add(new Vector3(tmpList[i].x, tmpList[i].y - 0.1f, b1[i]));
            uvsList.Add(new Vector2(tmpList[i].x, tmpList[i].y));

            verticesList.Add(new Vector3(tmpList[i].x, tmpList[i].y, b0[i]));
            uvsList.Add(new Vector2(tmpList[i].x, tmpList[i].y));
            verticesList.Add(new Vector3(tmpList[i].x, tmpList[i].y, b1[i]));
            uvsList.Add(new Vector2(tmpList[i].x, tmpList[i].y));

            verticesList.Add(new Vector3(tmpList[i].x, tmpList[i].y - width(tmpList[i].x), b0[i]));
            uvsList.Add(new Vector2(tmpList[i].x, tmpList[i].y - width(tmpList[i].x)));
            verticesList.Add(new Vector3(tmpList[i].x, tmpList[i].y - width(tmpList[i].x), b1[i]));
            uvsList.Add(new Vector2(tmpList[i].x, tmpList[i].y - width(tmpList[i].x)));

            verticesList.Add(new Vector3(tmpList[i].x, tmpList[i].y - width(tmpList[i].x), b0[i]));
            uvsList.Add(new Vector2(tmpList[i].x, -2));
            verticesList.Add(new Vector3(tmpList[i].x, tmpList[i].y - width(tmpList[i].x), b1[i]));
            uvsList.Add(new Vector2(tmpList[i].x, 2));

            tmp = verticesList.Count - tmp;
            if (i > 0)
            {
                x = verticesList.Count;
                facesList.Add((x - 4, x - 6, x - (tmp + 4), x - (tmp + 6)));
                facesList.Add((x - 5, x - 3, x - (tmp + 5), x - (tmp + 3)));
                facesList.Add((x - 1, x - 2, x - (tmp + 1), x - (tmp + 2)));
                facesList.Add((x - 8, x - 7, x - (tmp + 8), x - (tmp + 7)));
            }
        }
        // take-off table
        verticesList.Add(new Vector3(0, 0, b0[0]));
        uvsList.Add(new Vector2(-2, 0));
        verticesList.Add(new Vector3(0, 0, b1[0]));
        uvsList.Add(new Vector2(2, 0));
        verticesList.Add(new Vector3(0, -width(0), b0[0]));
        uvsList.Add(new Vector2(-2, -width(0)));
        verticesList.Add(new Vector3(0, -width(0), b1[0]));
        uvsList.Add(new Vector2(2, -width(0)));
        //top of the hill
        verticesList.Add(new Vector3(hill.A.x, hill.A.y, b0[b0.Length - 1]));
        uvsList.Add(new Vector2(-2, 0));
        verticesList.Add(new Vector3(hill.A.x, hill.A.y, b1[b1.Length - 1]));
        uvsList.Add(new Vector2(2, 0));
        verticesList.Add(new Vector3(hill.A.x, hill.A.y - width(hill.A.x), b0[b0.Length - 1]));
        uvsList.Add(new Vector2(-2, -width(hill.A.x)));
        verticesList.Add(new Vector3(hill.A.x, hill.A.y - width(hill.A.x), b1[b1.Length - 1]));
        uvsList.Add(new Vector2(2, -width(hill.A.x)));
        x = verticesList.Count;
        facesList.Add((x - 8, x - 7, x - 6, x - 5));
        facesList.Add((x - 3, x - 4, x - 1, x - 2));

        Vector3[] vertices = verticesList.ToArray();
        Vector2[] uvs = uvsList.ToArray();
        int[] triangles = FacesToTriangles(facesList);
        ObjectUpdate(inrunConstruction.gObj, mesh, inrunConstruction.materials[0], vertices, triangles, uvs, false);
    }

    public void GenerateInrunTrack()
    {
        Mesh mesh = inrunTrackSO.Generate(profileData.Value.b1, hill.inrunPoints);
        inrun.gObj.GetComponent<MeshFilter>().mesh = mesh;
        inrun.gObj.GetComponent<MeshRenderer>().materials = inrunTrackSO.GetMaterials();
    }

    public void GenerateGateStairs(ModelData gateStairs, int side, bool generate)
    {
        /* 0 - Left, 1 - Right */
        if (!generate)
        {
            gateStairs.gObj.GetComponent<MeshFilter>().mesh = null;
            gateStairs.gObj.GetComponent<MeshRenderer>().material = null;
            return;
        }

        Mesh mesh = gateStairsSO.Generate(side, hill.A, hill.B, hill.b1, hill.gates);

        gateStairs.gObj.GetComponent<MeshFilter>().mesh = mesh;
        gateStairs.gObj.GetComponent<MeshRenderer>().material = gateStairsSO.GetMaterial();
    }

    public void GenerateInrunStairs(ModelData inrunStairs, int side, bool generate, bool generate2, int stepsNumber)
    {
        /* 0 - Left, 1 - Right */
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<int> trianglesList = new List<int>();

        if (generate)
        {

            float width = 0.7f;
            float heightDiff = (generate2 ? hill.B.y - hill.T.y : hill.A.y - hill.T.y);
            // Debug.Log(heightDiff);
            float stepHeight = heightDiff / stepsNumber;
            float offset = ((side == 1) ? (width + profileData.Value.b1) : 0);
            int it = 0;
            Vector2[] pos = new Vector2[stepsNumber + 1];
            for (int i = 0; i < pos.Length; i++)
            {
                while (it < hill.inrunPoints.Length - 2 && (hill.inrunPoints[it + 1].y < i * stepHeight)) it++;

                pos[i] = new Vector2((stepHeight * i - hill.inrunPoints[it].y) / (hill.inrunPoints[it + 1].y - hill.inrunPoints[it].y) * (hill.inrunPoints[it + 1].x - hill.inrunPoints[it].x) + hill.inrunPoints[it].x, stepHeight * i);
            }

            float b = profileData.Value.b1 / 2;

            for (int i = 0; i < pos.Length - 1; i++)
            {
                verticesList.Add(new Vector3(pos[i].x, pos[i].y, -(b + width) + offset));
                uvsList.Add(new Vector2(0, 0));
                verticesList.Add(new Vector3(pos[i].x, pos[i].y, -b + offset));
                uvsList.Add(new Vector2(0, width));

                float deltaY = pos[i + 1].y - pos[i].y, deltaX = pos[i + 1].x - pos[i].x;
                verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -(b + width) + offset));
                uvsList.Add(new Vector2(deltaY, 0));
                verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -b + offset));
                uvsList.Add(new Vector2(deltaY, width));


                verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -(b + width) + offset));
                uvsList.Add(new Vector2(deltaY, 0));
                verticesList.Add(new Vector3(pos[i].x, pos[i + 1].y, -b + offset));
                uvsList.Add(new Vector2(deltaY, width));

                verticesList.Add(new Vector3(pos[i + 1].x, pos[i + 1].y, -(b + width) + offset));
                verticesList.Add(new Vector3(pos[i + 1].x, pos[i + 1].y, -b + offset));
                uvsList.Add(new Vector2(deltaY + deltaX, 0));
                uvsList.Add(new Vector2(deltaY + deltaX, width));

                int x = verticesList.Count;
                trianglesList.Add(x - 2);
                trianglesList.Add(x - 1);
                trianglesList.Add(x - 4);

                trianglesList.Add(x - 1);
                trianglesList.Add(x - 3);
                trianglesList.Add(x - 4);

                trianglesList.Add(x - 6);
                trianglesList.Add(x - 5);
                trianglesList.Add(x - 8);

                trianglesList.Add(x - 5);
                trianglesList.Add(x - 7);
                trianglesList.Add(x - 8);
            }

            for (int i = 0; i < pos.Length - 1; i++)
            {
                if (i > 0)
                {
                    float deltaY = pos[i].y - pos[i - 1].y, deltaX = pos[i].x - pos[i - 1].x;
                    verticesList.Add(new Vector3(pos[i - 1].x, pos[i].y, -(b + width) + offset));
                    uvsList.Add(new Vector2(pos[i - 1].x, pos[i].y));
                    verticesList.Add(new Vector3(pos[i - 1].x, pos[i].y, -b + offset));
                    uvsList.Add(new Vector2(pos[i - 1].x, pos[i].y));
                }


                verticesList.Add(new Vector3(pos[i].x, pos[i].y, -(b + width) + offset));
                uvsList.Add(new Vector2(pos[i].x, pos[i].y));
                verticesList.Add(new Vector3(pos[i].x, pos[i].y, -b + offset));
                uvsList.Add(new Vector2(pos[i].x, pos[i].y));

                int x = verticesList.Count;
                if (i > 0)
                {
                    trianglesList.Add(x - 5);
                    trianglesList.Add(x - 3);
                    trianglesList.Add(x - 1);

                    trianglesList.Add(x - 6);
                    trianglesList.Add(x - 2);
                    trianglesList.Add(x - 4);
                }

            }

        }

        Vector3[] vertices = verticesList.ToArray(); ;
        Vector2[] uvs = uvsList.ToArray();
        int[] triangles = trianglesList.ToArray();
        ObjectUpdate(inrunStairs.gObj, mesh, inrunStairs.materials[0], vertices, triangles, uvs, false);
    }

    public void GenerateLandingArea()
    {
        Mesh mesh = landingAreaSO.Generate(hill.landingAreaPoints, hill.w, hill.l1, hill.l2, hill.b2, hill.bK, hill.bU, hill.P, hill.K, hill.L, hill.U);
        landingArea.gObj.GetComponent<MeshFilter>().mesh = mesh;
        landingArea.gObj.GetComponent<MeshRenderer>().materials = landingAreaSO.GetMaterials();
    }

    public void GenerateMarks()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<(int, int, int, int)> facesList = new List<(int, int, int, int)>();
        float[] b = new float[hill.landingAreaPoints.Length];

        int pLen = Mathf.RoundToInt(hill.w - hill.l1), kLen = Mathf.RoundToInt(hill.w), lLen = Mathf.RoundToInt(hill.w + hill.l2);
        int uLen = 0;
        while ((hill.landingAreaPoints[uLen + 1] - hill.U).magnitude < (hill.landingAreaPoints[uLen] - hill.U).magnitude) uLen++;

        int mn = kLen / 2;
        int mx = Mathf.Min(uLen, lLen + 5);


        for (int i = mn; i <= mx; i++)
        {
            b[i] = hill.landingAreaPoints[i].x <= hill.K.x ? (profileData.Value.b2 / 2) + hill.landingAreaPoints[i].x / hill.K.x * ((profileData.Value.bK - profileData.Value.b2) / 2) :
              hill.landingAreaPoints[i].x >= hill.U.x ? (profileData.Value.bU / 2) : (profileData.Value.bK / 2) + (hill.landingAreaPoints[i].x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.Value.bU - profileData.Value.bK) / 2);
        }
        Vector2[] pts = { new Vector2(0, 1), new Vector2(0.25f, 1), new Vector2(0.5f, 1), new Vector2(0.75f, 1),
            new Vector2(0, 0.75f), new Vector2(0.25f, 0.75f), new Vector2(0.5f, 0.75f), new Vector2(0.75f, 0.75f), new Vector2(0, 0.5f), new Vector2(0.25f, 0.5f) };
        float sx = 0.15f, sy = 0.25f;

        for (int i = mn; i < mx; i++)
        {
            Vector3 pos = new Vector3(hill.landingAreaPoints[i].x, hill.landingAreaPoints[i].y + 0.3f, b[i] - 0.02f);
            string num = i.ToString();
            for (int j = 0; j < num.Length; j++)
            {
                int dig = num[j] - '0';
                // Debug.Log(i + " " + dig);
                float t0 = sx * (j - 1 + num.Length / 2.0f);
                verticesList.Add(pos + new Vector3(t0, sy, 0));
                uvsList.Add(pts[dig] + new Vector2(0.05f, 0));
                verticesList.Add(pos + new Vector3(t0 + sx, sy, 0));
                uvsList.Add(pts[dig] + new Vector2(0.20f, 0));
                verticesList.Add(pos + new Vector3(t0, 0, 0));
                uvsList.Add(pts[dig] + new Vector2(0.05f, -0.25f));
                verticesList.Add(pos + new Vector3(t0 + sx, 0, 0));
                uvsList.Add(pts[dig] + new Vector2(0.20f, -0.25f));
                int x = verticesList.Count;
                facesList.Add((x - 4, x - 3, x - 2, x - 1));
            }
        }

        Vector3[] vertices = verticesList.ToArray();
        int[] triangles = FacesToTriangles(facesList);
        Vector2[] uvs = uvsList.ToArray();
        ObjectUpdate(digitsMarks.gObj, mesh, digitsMarks.materials[0], vertices, triangles, uvs, false);
    }

    public void GenerateLandingAreaGuardrail(ModelData guardrail, int side, bool generate)
    {
        if (!generate)
        {
            guardrail.gObj.GetComponent<MeshFilter>().mesh = null;
            guardrail.gObj.GetComponent<MeshRenderer>().material = null;
            return;
        }
        int sgn = (side == 0 ? -1 : 1);
        Vector3[] points = hill.landingAreaPoints.Select(it => new Vector3(it.x, it.y, sgn * (it.x <= hill.K.x ? (profileData.Value.b2 / 2) + it.x / hill.K.x * ((profileData.Value.bK - profileData.Value.b2) / 2) :
              it.x >= hill.U.x ? (profileData.Value.bU / 2) : (profileData.Value.bK / 2) + (it.x - hill.K.x) / (hill.U.x - hill.K.x) * ((profileData.Value.bU - profileData.Value.bK) / 2)))).ToArray();

        Mesh mesh = LandingAreaGuardrailSO.Generate(points, side);
        guardrail.gObj.GetComponent<MeshFilter>().mesh = mesh;
        guardrail.gObj.GetComponent<MeshRenderer>().material = LandingAreaGuardrailSO.GetMaterial();
    }

    public void GenerateInrunGuardrail(ModelData guardrail, int side, bool generate)
    {
        if (!generate)
        {
            guardrail.gObj.GetComponent<MeshFilter>().mesh = null;
            guardrail.gObj.GetComponent<MeshRenderer>().material = null;
            return;
        }

        int sgn = (side == 0 ? -1 : 1);
        Vector3[] points = hill.inrunPoints.Where(it => it.x >= hill.B.x).Select(it => new Vector3(it.x, it.y, sgn * (hill.b1 / 2 - 2 * inrunGuardrailSO.Width))).Reverse().ToArray();

        Mesh mesh = inrunGuardrailSO.Generate(points, side);
        guardrail.gObj.GetComponent<MeshFilter>().mesh = mesh;
        guardrail.gObj.GetComponent<MeshRenderer>().material = inrunGuardrailSO.GetMaterial();
    }

    public void GenerateInrunOuterGuardrail(ModelData guardrail, int side, bool generate, bool generate2)
    {
        if (!generate)
        {
            guardrail.gObj.GetComponent<MeshFilter>().mesh = null;
            guardrail.gObj.GetComponent<MeshRenderer>().material = null;
            return;
        }

        float criticalPointX = Mathf.Lerp(hill.GatePoint(-1).x, hill.T.x, inrunStairsAngle);
        Vector2 p1 = hill.inrunPoints.Last(it => it.x > criticalPointX);
        Vector2 p2 = hill.inrunPoints.First(it => it.x <= criticalPointX);
        Vector2 criticalPoint = Vector2.Lerp(p1, p2, (criticalPointX - p1.x) / (p2.x - p1.x));
        List<Vector2> tmpList = new List<Vector2>();
        tmpList.AddRange(hill.inrunPoints.Where(it => it.x > criticalPoint.x));
        tmpList.Add(criticalPoint);
        tmpList.AddRange(hill.inrunPoints.Where(it => it.x <= criticalPoint.x && it.x > hill.GatePoint(-1).x));
        tmpList.Add(hill.GatePoint(-1));
        tmpList.AddRange(hill.inrunPoints.Where(it => it.x <= hill.GatePoint(-1).x));
        float[] len = new float[tmpList.Count];
        float[] b;
        if (generate2)
        {
            b = tmpList.Select(it => (it.x > hill.GatePoint(-1).x ? (it.x > criticalPoint.x ? hill.b1 / 2 + 0.7f : Mathf.Lerp(hill.b1 / 2 + 0.7f, hill.b1 / 2 + gateStairsSO.StepWidth, (it.x - criticalPointX) / (criticalPointX - hill.GatePoint(-1).x))) : (hill.b1 / 2 + gateStairsSO.StepWidth))).ToArray();
        }
        else
        {
            b = tmpList.Select(it => hill.b1 / 2 + 0.7f).ToArray();
        }

        int sgn = (side == 0 ? -1 : 1);
        Vector3[] points = tmpList.Select((val, ind) => new Vector3(val.x, val.y, sgn * b[ind])).Reverse().ToArray();

        Mesh mesh = InrunOuterGuardrailSO.Generate(points, side);
        guardrail.gObj.GetComponent<MeshFilter>().mesh = mesh;
        guardrail.gObj.GetComponent<MeshRenderer>().material = InrunOuterGuardrailSO.GetMaterial();
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
        for (int i = 0; i < hill.inrunPoints.Length; i += 5)
        {
            lamps.Add(Instantiate(inrunLampPrefab, new Vector3(hill.inrunPoints[i].x, hill.inrunPoints[i].y, 2), Quaternion.identity));
            // lamps.Add(Instantiate(inrunLampPrefab, new Vector3(hill.inrunPoints[i].x, hill.inrunPoints[i].y + 1f, -2), Quaternion.identity));
        }

        for (int i = 0; i < hill.landingAreaPoints.Length; i += 80)
        {
            lamps.Add(Instantiate(lampPrefab, new Vector3(hill.landingAreaPoints[i].x, hill.landingAreaPoints[i].y, 45), Quaternion.identity));
        }
    }



    public void SaveMesh(GameObject gObj, string name, bool isCollider = false)
    {
#if UNITY_EDITOR
        string saveName = profileData.Value.name + "_" + name;
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