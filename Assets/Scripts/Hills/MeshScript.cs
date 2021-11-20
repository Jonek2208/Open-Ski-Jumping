using System;
using System.Collections.Generic;
using OpenSkiJumping.Hills.Guardrails;
using OpenSkiJumping.Hills.InrunTracks;
using OpenSkiJumping.Hills.LandingAreas;
using OpenSkiJumping.Hills.StairsOld;
using OpenSkiJumping.ScriptableObjects.Variables;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace OpenSkiJumping.Hills
{
    [Serializable]
    public class ModelData
    {
        public GameObject gObj;
        public Material[] materials;
    }

    public class MeshScript : MonoBehaviour
    {
        [Header("Model datas")]
        public ModelData inrun;
        public ModelData digitsMarks;
        public ModelData landingArea;
        
        [Space]

        public Hill hill;


        /* Hill profile */

        public InrunTrack inrunTrackSO;
        public Vector3 jumperPosition;
        public Quaternion jumperRotation;

        private List<GameObject> lamps;
        public LandingArea landingAreaSO;

        public ProfileData profileData;
        

        public void SetGate(Hill hill, int nr)
        {
            jumperPosition = new Vector3(hill.GatePoint(nr).x, hill.GatePoint(nr).y, 0);
            jumperRotation.eulerAngles = new Vector3(0, 0, -hill.gamma);
        }

        public void GenerateMesh()
        {
            hill.SetValues(profileData);

            GenerateInrunCollider();
            GenerateInrunTrack();
            GenerateLandingAreaCollider();
            GenerateLandingArea();
            GenerateMarks();
            
            SetGate(hill, 1);
        }
        
        public static void ObjectUpdate(GameObject gObj, Mesh mesh, Material material, Vector3[] vertices,
            int[] triangles, Vector2[] uvs, bool hasCollider)
        {
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            gObj.GetComponent<MeshFilter>().mesh = mesh;
            gObj.GetComponent<MeshRenderer>().material = material;
            if (hasCollider)
            {
                gObj.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }
        
        public void GenerateInrunCollider()
        {
            var mesh = new Mesh();

            var vertices = new Vector3[hill.inrunPoints.Length * 2];
            var uvs = new Vector2[hill.inrunPoints.Length * 2];
            var triangles = new int[(hill.inrunPoints.Length - 1) * 6];

            var len = new float[hill.inrunPoints.Length];

            for (var i = 1; i < hill.inrunPoints.Length; i++)
            {
                len[i] = len[i - 1] + (hill.inrunPoints[i] - hill.inrunPoints[i - 1]).magnitude;
            }

            for (var i = 0; i < hill.inrunPoints.Length; i++)
            {
                vertices[2 * i] = new Vector3(hill.inrunPoints[i].x, hill.inrunPoints[i].y, -profileData.b1 / 2);
                vertices[2 * i + 1] =
                    new Vector3(hill.inrunPoints[i].x, hill.inrunPoints[i].y, profileData.b1 / 2);
                uvs[2 * i] = new Vector2(len[i], -profileData.b1);
                uvs[2 * i + 1] = new Vector2(len[i], profileData.b1);
            }

            for (var i = 0; i < hill.inrunPoints.Length - 1; i++)
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
            var mesh = new Mesh();
            var verticesList = new List<Vector3>();
            var uvsList = new List<Vector2>();
            var facesList = new List<(int, int, int, int)>();

            var b = new float[hill.landingAreaPoints.Length];

            for (var i = 0; i < hill.landingAreaPoints.Length; i++)
            {
                b[i] = hill.landingAreaPoints[i].x <= hill.K.x
                    ? profileData.b2 / 2 + hill.landingAreaPoints[i].x / hill.K.x *
                    ((profileData.bK - profileData.b2) / 2)
                    : hill.landingAreaPoints[i].x >= hill.U.x
                        ? profileData.bU / 2
                        : profileData.bK / 2 + (hill.landingAreaPoints[i].x - hill.K.x) /
                        (hill.U.x - hill.K.x) * ((profileData.bU - profileData.bK) / 2);
            }

            for (var i = 0; i < hill.landingAreaPoints.Length; i++)
            {
                verticesList.Add(new Vector3(hill.landingAreaPoints[i].x, hill.landingAreaPoints[i].y, -b[i]));
                uvsList.Add(new Vector2(i, -b[i]));

                verticesList.Add(new Vector3(hill.landingAreaPoints[i].x, hill.landingAreaPoints[i].y, b[i]));
                uvsList.Add(new Vector2(i, b[i]));

                if (i > 0)
                {
                    var x = verticesList.Count;
                    facesList.Add((x - 4, x - 3, x - 2, x - 1));
                }
            }

            mesh.vertices = verticesList.ToArray();
            mesh.triangles = MeshFunctions.FacesToTriangles(facesList);
            mesh.uv = uvsList.ToArray();
            landingArea.gObj.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        public void GenerateInrunTrack()
        {
            var mesh = inrunTrackSO.Generate(profileData.b1, hill.inrunPoints);
            inrun.gObj.GetComponent<MeshFilter>().mesh = mesh;
            inrun.gObj.GetComponent<MeshRenderer>().materials = inrunTrackSO.GetMaterials();
        }
        public void GenerateLandingArea()
        {
            var mesh = landingAreaSO.Generate(hill.landingAreaPoints, hill.w, hill.w - hill.l1, hill.hS, hill.b2, hill.bK,
                hill.bU, hill.P, hill.K, hill.L, hill.U, hill.landingAreaData);
            landingArea.gObj.GetComponent<MeshFilter>().mesh = mesh;
            landingArea.gObj.GetComponent<MeshRenderer>().materials = landingAreaSO.GetMaterials();
        }

        public void GenerateMarks()
        {
            var mesh = new Mesh();
            var verticesList = new List<Vector3>();
            var uvsList = new List<Vector2>();
            var facesList = new List<(int, int, int, int)>();
            var b = new float[hill.landingAreaPoints.Length];

            int pLen = Mathf.RoundToInt(hill.w - hill.l1),
                kLen = Mathf.RoundToInt(hill.w),
                lLen = Mathf.RoundToInt(hill.w + hill.l2);
            var uLen = 0;
            while ((hill.landingAreaPoints[uLen + 1] - hill.U).magnitude <
                   (hill.landingAreaPoints[uLen] - hill.U).magnitude) uLen++;

            var mn = hill.landingAreaData.metersLow == 0 ? kLen / 2 : hill.landingAreaData.metersLow;
            var mx = Mathf.Min(uLen, lLen + 5);
            mx = hill.landingAreaData.metersHigh == 0 ? mx : hill.landingAreaData.metersHigh;


            for (var i = mn; i <= mx; i++)
            {
                b[i] = hill.landingAreaPoints[i].x <= hill.K.x
                    ? profileData.b2 / 2 + hill.landingAreaPoints[i].x / hill.K.x *
                    ((profileData.bK - profileData.b2) / 2)
                    : hill.landingAreaPoints[i].x >= hill.U.x
                        ? profileData.bU / 2
                        : profileData.bK / 2 + (hill.landingAreaPoints[i].x - hill.K.x) /
                        (hill.U.x - hill.K.x) * ((profileData.bU - profileData.bK) / 2);
            }

            Vector2[] numbersUVs =
            {
                new(0, 1), new(0.25f, 1), new(0.5f, 1), new(0.75f, 1),
                new(0, 0.75f), new(0.25f, 0.75f), new(0.5f, 0.75f), new(0.75f, 0.75f),
                new(0, 0.5f), new(0.25f, 0.5f)
            };

            const float numberSizeX = 0.15f;
            const float numberSizeY = 0.25f;
            const float numberPlateOffset = 0.02f;

            for (var distNum = mn; distNum < mx; distNum++)
            {
                var num = distNum.ToString();
                for (var side = 0; side < 2; side++)
                {
                    var sgn = 2 * side - 1;
                    var pos = new Vector3(hill.landingAreaPoints[distNum].x, hill.landingAreaPoints[distNum].y + 0.3f,
                        (b[distNum] - numberPlateOffset) * sgn);
                    for (var j = 0; j < num.Length; j++)
                    {
                        var digit = num[j] - '0';

                        var plateAnchorX = numberSizeX * (sgn * (j - 1) + num.Length / 2.0f);
                        verticesList.Add(pos + new Vector3(plateAnchorX, numberSizeY, 0));
                        uvsList.Add(numbersUVs[digit] + new Vector2(0.05f, 0));
                        verticesList.Add(pos + new Vector3(plateAnchorX + sgn * numberSizeX, numberSizeY, 0));
                        uvsList.Add(numbersUVs[digit] + new Vector2(0.20f, 0));
                        verticesList.Add(pos + new Vector3(plateAnchorX, 0, 0));
                        uvsList.Add(numbersUVs[digit] + new Vector2(0.05f, -0.25f));
                        verticesList.Add(pos + new Vector3(plateAnchorX + sgn * numberSizeX, 0, 0));
                        uvsList.Add(numbersUVs[digit] + new Vector2(0.20f, -0.25f));
                        var x = verticesList.Count;
                        facesList.Add((x - 4, x - 3, x - 2, x - 1));
                    }
                }
            }

            var vertices = verticesList.ToArray();
            var triangles = MeshFunctions.FacesToTriangles(facesList);
            var uvs = uvsList.ToArray();
            ObjectUpdate(digitsMarks.gObj, mesh, digitsMarks.materials[0], vertices, triangles, uvs, false);
        }


        public void SaveMesh(GameObject gObj, string name, bool isCollider = false)
        {
#if UNITY_EDITOR
            var saveName = profileData.name + "_" + name;
            if (isCollider)
            {
                var mc = gObj.GetComponent<MeshCollider>();
                if (mc)
                {
                    var savePath = "Assets/" + saveName + ".asset";
                    Debug.Log("Saved Mesh to:" + savePath);
                    AssetDatabase.CreateAsset(mc.sharedMesh, savePath);
                }
            }
            else
            {
                var mf = gObj.GetComponent<MeshFilter>();
                if (mf)
                {
                    var savePath = "Assets/" + saveName + ".asset";
                    Debug.Log("Saved Mesh to:" + savePath);
                    AssetDatabase.CreateAsset(mf.mesh, savePath);
                }
            }

#endif
        }
    }
}