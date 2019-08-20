using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HillProfile;

public class MeshScript : MonoBehaviour
{
    public Material snowMaterial;
    public Material iceMaterial;
    public Material terrainMaterial;
    public Material stairsMaterial;

    public GameObject inrun;
    public GameObject landingArea;
    Mesh inrunMesh;
    Mesh landingAreaMesh;

    //Hill construction
    public GameObject gateStairs;
    public GameObject startGate;
    Mesh gateStairsMesh;
    Mesh startGateMesh;

    //Terain
    public GameObject terrain;
    Mesh terrainMesh;

    public Terrain terrain1;
    public Terrain terrain2;

    public LineRenderer lineRenderer;

    public Vector3 jumperPosition;
    public Quaternion jumperRotation;


    public Vector2[] landingAreaPoints;
    public Vector2[] inrunPoints;

    private void Start()
    {
        float[,] tab = new float[513, 513];
        for (int i = 0; i < 513; i++)
            for (int j = 0; j < 513; j++)
            {
                tab[i, j] = 0;
            }

        terrain1.terrainData.SetHeights(0, 0, tab);
        // Debug.Log(terrain1.terrainData.size);
        jumperPosition = new Vector3(0, 0, 0);
        jumperRotation = new Quaternion();
    }

    public void SetGate(Hill hill, int nr)
    {
        jumperPosition = new Vector3(hill.GatePoint(nr).x, hill.GatePoint(nr).y, 0);
        jumperRotation.eulerAngles = new Vector3(0, 0, -hill.gamma);
    }

    public void GenerateMesh(Hill hill)
    {
        landingAreaMesh = new Mesh();
        inrunMesh = new Mesh();
        gateStairsMesh = new Mesh();
        terrainMesh = new Mesh();

        // Hill oberstdorf = new Hill(Hill.ProfileType.ICR1996, 120, 0.575f, 35, 11, 99, 23, 6.5f, 115, 37.43f, 35.5f, 32.4f, 3.38f, 11.15f, 17.42f, 321, 100, 100);
        hill.Calculate();

        //Landing Area
        landingAreaPoints = hill.LandingAreaPoints(1000);

        Vector3[] vertices = new Vector3[landingAreaPoints.Length * 2];
        Vector2[] uv = new Vector2[landingAreaPoints.Length * 2];
        int[] triangles = new int[(landingAreaPoints.Length - 1) * 6];

        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            vertices[2 * i] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -10);
            vertices[2 * i + 1] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, 10);
            uv[2 * i] = new Vector2(i, 0);
            uv[2 * i + 1] = new Vector2(i, 20);
        }

        for (int i = 0; i < landingAreaPoints.Length - 1; i++)
        {
            triangles[6 * i] = 2 * i;
            triangles[6 * i + 1] = 2 * i + 1;
            triangles[6 * i + 2] = 2 * i + 2;
            triangles[6 * i + 3] = 2 * i + 1;
            triangles[6 * i + 4] = 2 * i + 3;
            triangles[6 * i + 5] = 2 * i + 2;
        }

        landingAreaMesh.vertices = vertices;
        landingAreaMesh.triangles = triangles;
        landingAreaMesh.uv = uv;
        landingAreaMesh.RecalculateNormals();

        landingArea.GetComponent<MeshFilter>().mesh = landingAreaMesh;
        landingArea.GetComponent<MeshRenderer>().material = snowMaterial;
        landingArea.GetComponent<MeshCollider>().sharedMesh = landingAreaMesh;


        //Inrun
        inrunPoints = hill.InrunPoints();

        Vector3[] vertices1 = new Vector3[inrunPoints.Length * 2];
        Vector2[] uv1 = new Vector2[inrunPoints.Length * 2];
        int[] triangles1 = new int[(inrunPoints.Length - 1) * 6];

        for (int i = 0; i < inrunPoints.Length; i++)
        {
            vertices1[2 * i] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, -2);
            vertices1[2 * i + 1] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, 2);
            uv1[2 * i] = new Vector2(i, 0);
            uv1[2 * i + 1] = new Vector2(i, 4);
        }

        for (int i = 0; i < inrunPoints.Length - 1; i++)
        {
            triangles1[6 * i] = 2 * i;
            triangles1[6 * i + 1] = 2 * i + 3;
            triangles1[6 * i + 2] = 2 * i + 1;
            triangles1[6 * i + 3] = 2 * i;
            triangles1[6 * i + 4] = 2 * i + 2;
            triangles1[6 * i + 5] = 2 * i + 3;
        }

        inrunMesh.vertices = vertices1;
        inrunMesh.triangles = triangles1;
        inrunMesh.uv = uv1;
        inrunMesh.RecalculateNormals();

        inrun.GetComponent<MeshFilter>().mesh = inrunMesh;
        inrun.GetComponent<MeshRenderer>().material = iceMaterial;
        inrun.GetComponent<MeshCollider>().sharedMesh = inrunMesh;

        //Gate Stairs
        Vector3[] vertices2 = new Vector3[4 * (hill.gates + 2)];
        Vector2[] uv2 = new Vector2[4 * (hill.gates + 2)];
        int[] triangles2 = new int[6 * (2 * (hill.gates + 2) - 1)];

        for (int i = 0; i < hill.gates + 2; i++)
        {
            Vector2 pos = hill.B + (hill.A - hill.B) * ((float)(i) / (float)(hill.gates - 1));
            Vector2 pos0 = hill.B + (hill.A - hill.B) * ((float)(i - 1) / (float)(hill.gates - 1));
            vertices2[4 * i] = new Vector3(pos0.x, pos.y, -6);
            vertices2[4 * i + 1] = new Vector3(pos0.x, pos.y, -2);
            vertices2[4 * i + 2] = new Vector3(pos.x, pos.y, -6);
            vertices2[4 * i + 3] = new Vector3(pos.x, pos.y, -2);
            uv2[4 * i] = new Vector2(i, 0);
            uv2[4 * i + 1] = new Vector2(i, 1);
        }

        for (int i = 0; i < hill.gates + 1; i++)
        {
            triangles2[6 * i] = 4 * i;
            triangles2[6 * i + 1] = 4 * i + 3;
            triangles2[6 * i + 2] = 4 * i + 1;
            triangles2[6 * i + 3] = 4 * i;
            triangles2[6 * i + 4] = 4 * i + 2;
            triangles2[6 * i + 5] = 4 * i + 3;
        }

        for (int i = 0; i < hill.gates; i++)
        {
            triangles2[6 * (i + hill.gates + 1)] = 4 * i + 2;
            triangles2[6 * (i + hill.gates + 1) + 1] = 4 * i + 5;
            triangles2[6 * (i + hill.gates + 1) + 2] = 4 * i + 3;
            triangles2[6 * (i + hill.gates + 1) + 3] = 4 * i + 2;
            triangles2[6 * (i + hill.gates + 1) + 4] = 4 * i + 4;
            triangles2[6 * (i + hill.gates + 1) + 5] = 4 * i + 5;
        }

        gateStairsMesh.vertices = vertices2;
        gateStairsMesh.triangles = triangles2;
        gateStairsMesh.uv = uv2;
        gateStairsMesh.RecalculateNormals();


        gateStairs.GetComponent<MeshFilter>().mesh = gateStairsMesh;
        gateStairs.GetComponent<MeshRenderer>().material = stairsMaterial;


        //Barriers
        Vector3[] vertices3 = new Vector3[4 * (landingAreaPoints.Length + inrunPoints.Length)];
        Vector2[] uv3 = new Vector2[2 * landingAreaPoints.Length];
        int[] triangles3 = new int[(landingAreaPoints.Length + inrunPoints.Length - 2) * 18];

        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            vertices3[4 * i] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, 10.2f);
            vertices3[4 * i + 1] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + 1, 10.2f);
            vertices3[4 * i + 2] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y + 1, 10);
            vertices3[4 * i + 3] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, 10);
        }

        for (int i = 0; i < landingAreaPoints.Length - 1; i++)
        {
            triangles3[18 * i + 0] = 4 * i + 0;
            triangles3[18 * i + 1] = 4 * i + 4;
            triangles3[18 * i + 2] = 4 * i + 1;
            triangles3[18 * i + 3] = 4 * i + 1;
            triangles3[18 * i + 4] = 4 * i + 4;
            triangles3[18 * i + 5] = 4 * i + 5;
            triangles3[18 * i + 6] = 4 * i + 1;
            triangles3[18 * i + 7] = 4 * i + 5;
            triangles3[18 * i + 8] = 4 * i + 2;
            triangles3[18 * i + 9] = 4 * i + 2;
            triangles3[18 * i + 10] = 4 * i + 5;
            triangles3[18 * i + 11] = 4 * i + 6;
            triangles3[18 * i + 12] = 4 * i + 2;
            triangles3[18 * i + 13] = 4 * i + 6;
            triangles3[18 * i + 14] = 4 * i + 3;
            triangles3[18 * i + 15] = 4 * i + 3;
            triangles3[18 * i + 16] = 4 * i + 6;
            triangles3[18 * i + 17] = 4 * i + 7;
        }

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
        terrain.GetComponent<MeshRenderer>().material = terrainMaterial;


        //Terrain

        Vector3 center = terrain1.GetComponent<Transform>().position;
        Debug.Log(center);
        Debug.Log(terrain1.terrainData.size);
        float[,] tab = new float[terrain1.terrainData.heightmapResolution, terrain1.terrainData.heightmapResolution];
        for (int i = 0; i < terrain1.terrainData.heightmapResolution; i++)
        {
            for (int j = 0; j < terrain1.terrainData.heightmapResolution; j++)
            {
                float x = (float)(j) / terrain1.terrainData.heightmapResolution * (terrain1.terrainData.size.x) + center.x;
                float z = (float)(i) / terrain1.terrainData.heightmapResolution * (terrain1.terrainData.size.z) + center.z;

                float y = 0;
                float b = 0;

                if (x < hill.T.x)
                {
                    y = hill.Inrun(x);
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
                    y += ((Mathf.Abs(z) - b) <= 50 ? 2 * (Mathf.Abs(z) - b) : 100) * (Mathf.PerlinNoise(x / 100.0f, z / 100.0f) - 0.5f);
                y = (y - center.y - 1) / terrain1.terrainData.size.y;


                if (i == 200 && j == 200) Debug.Log(x + " " + y);
                // Debug.Log(x + " " + y);

                tab[i, j] = Mathf.Clamp(y, 0, 1);
            }
        }

        terrain1.terrainData.SetHeights(0, 0, tab);

        center = terrain2.GetComponent<Transform>().position;
        tab = new float[terrain1.terrainData.heightmapResolution, terrain1.terrainData.heightmapResolution];

        for (int i = 0; i < terrain2.terrainData.heightmapResolution; i++)
        {
            for (int j = 0; j < terrain2.terrainData.heightmapResolution; j++)
            {
                float x = (float)(j) / terrain2.terrainData.heightmapResolution * (terrain2.terrainData.size.x) + center.x;
                float z = (float)(i) / terrain2.terrainData.heightmapResolution * (terrain2.terrainData.size.z) + center.z;

                float y = 0;
                float b = 0;

                if (x < hill.T.x)
                {
                    y = hill.Inrun(x);
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
                    y += ((Mathf.Abs(z) - b) <= 50 ? 2 * (Mathf.Abs(z) - b) : 100) * (Mathf.PerlinNoise(x / 100.0f, z / 100.0f) - 0.5f);
                y = (y - center.y - 1) / terrain2.terrainData.size.y;


                if (i == 200 && j == 200) Debug.Log(x + " " + y);
                // Debug.Log(x + " " + y);

                tab[i, j] = Mathf.Clamp(y, 0, 1);
            }
        }

        terrain2.terrainData.SetHeights(0, 0, tab);

        SetGate(hill, 1);

        lineRenderer.SetPosition(0, new Vector3(landingAreaPoints[Mathf.RoundToInt(hill.w)].x, landingAreaPoints[Mathf.RoundToInt(hill.w)].y, -7));
        lineRenderer.SetPosition(1, new Vector3(landingAreaPoints[Mathf.RoundToInt(hill.w)].x, landingAreaPoints[Mathf.RoundToInt(hill.w)].y, 7));
        lineRenderer.SetPosition(2, new Vector3(landingAreaPoints[Mathf.RoundToInt(hill.w + hill.l2)].x, landingAreaPoints[Mathf.RoundToInt(hill.w + hill.l2)].y, -7));
        lineRenderer.SetPosition(3, new Vector3(landingAreaPoints[Mathf.RoundToInt(hill.w + hill.l2)].x, landingAreaPoints[Mathf.RoundToInt(hill.w + hill.l2)].y, 7));
    }

    // public void SaveMesh()
    // {
    //     string saveName = "inrun";
    //     MeshFilter mf = inrun.GetComponent<MeshFilter>();
    //     if (mf)
    //     {
    //         string savePath = "Assets/" + saveName + ".asset";
    //         Debug.Log("Saved Mesh to:" + savePath);
    //         AssetDatabase.CreateAsset(mf.mesh, savePath);
    //     }
    // }
}
