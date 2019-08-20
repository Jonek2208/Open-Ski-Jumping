using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HillProfile;

public class MeshScript : MonoBehaviour
{
    public Material material;
    public Material terrainMaterial;

    public GameObject inrun;
    public GameObject landingArea;
    Mesh inrunMesh;
    Mesh landingAreaMesh;

    //Hill construction
    public GameObject gateStairs;
    Mesh gateStairsMesh;

    //Terain
    public GameObject terrain;
    Mesh terrainMesh;

    public Terrain terrain1;

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
    }

    public void GenerateMesh(Hill hill, int gates)
    {
        landingAreaMesh = new Mesh();
        inrunMesh = new Mesh();
        gateStairsMesh = new Mesh();
        terrainMesh = new Mesh();

        // Hill oberstdorf = new Hill(Hill.ProfileType.ICR1996, 120, 0.575f, 35, 11, 99, 23, 6.5f, 115, 37.43f, 35.5f, 32.4f, 3.38f, 11.15f, 17.42f, 321, 100, 100);
        hill.Calculate();

        //Landing Area
        Vector2[] landingAreaPoints = hill.LandingAreaPoints(1000);

        Vector3[] vertices = new Vector3[landingAreaPoints.Length * 2];
        Vector2[] uv = new Vector2[landingAreaPoints.Length * 2];
        int[] triangles = new int[(landingAreaPoints.Length - 1) * 6];

        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            vertices[2 * i] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -10);
            vertices[2 * i + 1] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, 10);
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

        landingArea.GetComponent<MeshFilter>().mesh = landingAreaMesh;
        landingArea.GetComponent<MeshRenderer>().material = material;


        //Inrun
        Vector2[] inrunPoints = hill.InrunPoints();

        Vector3[] vertices1 = new Vector3[inrunPoints.Length * 2];
        Vector2[] uv1 = new Vector2[inrunPoints.Length * 2];
        int[] triangles1 = new int[(inrunPoints.Length - 1) * 6];

        for (int i = 0; i < inrunPoints.Length; i++)
        {
            vertices1[2 * i] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, -2);
            vertices1[2 * i + 1] = new Vector3(inrunPoints[i].x, inrunPoints[i].y, 2);
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

        inrun.GetComponent<MeshFilter>().mesh = inrunMesh;
        inrun.GetComponent<MeshRenderer>().material = material;

        //Gate Stairs
        Vector3[] vertices2 = new Vector3[4 * gates];
        Vector2[] uv2 = new Vector2[4 * gates];
        int[] triangles2 = new int[6 * (2 * gates - 1)];

        for (int i = 0; i < gates; i++)
        {
            Vector2 pos = hill.B + (hill.A - hill.B) * ((float)(i + 1) / (float)gates);
            Vector2 pos0 = hill.B + (hill.A - hill.B) * ((float)i / (float)gates);
            vertices2[4 * i] = new Vector3(pos0.x, pos.y, -6);
            vertices2[4 * i + 1] = new Vector3(pos0.x, pos.y, -2);
            vertices2[4 * i + 2] = new Vector3(pos.x, pos.y, -6);
            vertices2[4 * i + 3] = new Vector3(pos.x, pos.y, -2);
        }

        for (int i = 0; i < gates; i++)
        {
            triangles2[6 * i] = 4 * i;
            triangles2[6 * i + 1] = 4 * i + 3;
            triangles2[6 * i + 2] = 4 * i + 1;
            triangles2[6 * i + 3] = 4 * i;
            triangles2[6 * i + 4] = 4 * i + 2;
            triangles2[6 * i + 5] = 4 * i + 3;
        }

        for (int i = 0; i < gates - 1; i++)
        {
            triangles2[6 * (i + gates)] = 4 * i + 2;
            triangles2[6 * (i + gates) + 1] = 4 * i + 5;
            triangles2[6 * (i + gates) + 2] = 4 * i + 3;
            triangles2[6 * (i + gates) + 3] = 4 * i + 2;
            triangles2[6 * (i + gates) + 4] = 4 * i + 4;
            triangles2[6 * (i + gates) + 5] = 4 * i + 5;
        }

        // vertices2[0] = new Vector3(hill.B.x, hill.B.y, -4);
        // vertices2[1] = new Vector3(hill.B.x, hill.B.y, -2);
        // vertices2[2] = new Vector3(hill.A.x, hill.A.y, -4);
        // vertices2[3] = new Vector3(hill.A.x, hill.A.y, -2);
        // int[] triangles2 = {0,3,1,0,2,3};

        gateStairsMesh.vertices = vertices2;
        gateStairsMesh.triangles = triangles2;

        gateStairs.GetComponent<MeshFilter>().mesh = gateStairsMesh;
        gateStairs.GetComponent<MeshRenderer>().material = material;


        //Terrain
        Vector3[] vertices3 = new Vector3[4 * (landingAreaPoints.Length + inrunPoints.Length)];
        Vector2[] uv3 = new Vector2[2 * landingAreaPoints.Length];
        int[] triangles3 = new int[(landingAreaPoints.Length + inrunPoints.Length - 2) * 12];

        for (int i = 0; i < landingAreaPoints.Length; i++)
        {
            vertices3[4 * i] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -100);
            vertices3[4 * i + 1] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, -10);
            vertices3[4 * i + 2] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, 10);
            vertices3[4 * i + 3] = new Vector3(landingAreaPoints[i].x, landingAreaPoints[i].y, 100);
        }

        for (int i = 0; i < landingAreaPoints.Length - 1; i++)
        {
            triangles3[12 * i] = 4 * i;
            triangles3[12 * i + 1] = 4 * i + 1;
            triangles3[12 * i + 2] = 4 * i + 4;
            triangles3[12 * i + 3] = 4 * i + 1;
            triangles3[12 * i + 4] = 4 * i + 5;
            triangles3[12 * i + 5] = 4 * i + 4;
            triangles3[12 * i + 6] = 4 * i + 2;
            triangles3[12 * i + 7] = 4 * i + 3;
            triangles3[12 * i + 8] = 4 * i + 6;
            triangles3[12 * i + 9] = 4 * i + 3;
            triangles3[12 * i + 10] = 4 * i + 7;
            triangles3[12 * i + 11] = 4 * i + 6;
        }

        for (int i = 0; i < inrunPoints.Length; i++)
        {
            vertices3[4 * (i + landingAreaPoints.Length)] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - hill.s, -100);
            vertices3[4 * (i + landingAreaPoints.Length) + 1] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - hill.s, -2);
            vertices3[4 * (i + landingAreaPoints.Length) + 2] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - hill.s, 2);
            vertices3[4 * (i + landingAreaPoints.Length) + 3] = new Vector3(inrunPoints[i].x, inrunPoints[i].y - hill.s, 100);
        }

        for (int i = 0; i < inrunPoints.Length - 1; i++)
        {
            triangles3[12 * (i + landingAreaPoints.Length - 1)] = 4 * (i + landingAreaPoints.Length);
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 1] = 4 * (i + landingAreaPoints.Length) + 5;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 2] = 4 * (i + landingAreaPoints.Length) + 1;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 3] = 4 * (i + landingAreaPoints.Length);
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 4] = 4 * (i + landingAreaPoints.Length) + 4;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 5] = 4 * (i + landingAreaPoints.Length) + 5;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 6] = 4 * (i + landingAreaPoints.Length) + 2;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 7] = 4 * (i + landingAreaPoints.Length) + 7;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 8] = 4 * (i + landingAreaPoints.Length) + 3;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 9] = 4 * (i + landingAreaPoints.Length) + 2;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 10] = 4 * (i + landingAreaPoints.Length) + 6;
            triangles3[12 * (i + landingAreaPoints.Length - 1) + 11] = 4 * (i + landingAreaPoints.Length) + 7;
        }

        terrainMesh.vertices = vertices3;
        terrainMesh.triangles = triangles3;

        terrain.GetComponent<MeshFilter>().mesh = terrainMesh;
        terrain.GetComponent<MeshRenderer>().material = terrainMaterial;

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

                if (x < hill.T.x)
                {
                    y = hill.Inrun(x);
                }
                else if (hill.T.x <= x && x <= hill.U.x)
                {
                    y = hill.LandingArea(x);
                }
                else if (x > hill.U.x)
                {
                    y = hill.U.y;
                }

                if ((z < -20 || 20 < z) && (hill.T.x <= x) || (z < -4 || 4 < z) && (hill.A.x <= x && x <= hill.T.x) || (x < hill.A.x)) y += (Mathf.Abs(z) <= 50 ? 2*Mathf.Abs(z) : 100) *(Mathf.PerlinNoise(x / 100.0f, z / 100.0f) - 0.5f);
                y = (y - center.y - 2) / terrain1.terrainData.size.y;


                if (i == 200 && j == 200) Debug.Log(x + " " + y);
                // Debug.Log(x + " " + y);

                tab[i, j] = Mathf.Clamp(y, 0, 1);
            }
        }

        terrain1.terrainData.SetHeights(0, 0, tab);
    }

    public void SaveMesh()
    {
        string saveName = "inrun";
        MeshFilter mf = inrun.GetComponent<MeshFilter>();
        if (mf)
        {
            string savePath = "Assets/" + saveName + ".asset";
            Debug.Log("Saved Mesh to:" + savePath);
            AssetDatabase.CreateAsset(mf.mesh, savePath);
        }
    }
}
