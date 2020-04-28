using System.Collections.Generic;
using UnityEngine;

namespace OpenSkiJumping.Hills.InrunTracks
{
    [CreateAssetMenu(menuName = "HillElements/InrunTrack")]
    public class InrunTrack : ScriptableObject
    {
        [SerializeField]
        private Vector2[] inrunTrackVals;
        [SerializeField]
        private Material[] materials;
        public Mesh Generate(float b1, Vector2[] inrunPoints)
        {
            int segmentsCount = inrunTrackVals.Length + 1;
            Mesh mesh = new Mesh();
            List<Vector3> verticesList = new List<Vector3>();
            List<Vector2> uvsList = new List<Vector2>();
            List<int>[] trianglesList = new List<int>[segmentsCount];


            Vector2[] hillInrunTrackVals = GenerateRealVals(b1);
            float[] widths = GeneratePrefLenghts(hillInrunTrackVals);
            float[] lengths = GeneratePrefLenghts(inrunPoints);

            int cnt = 0;
            mesh.subMeshCount = materials.Length;

            for (int j = 0; j < segmentsCount; j++)
            {
                trianglesList[j] = new List<int>();
                for (int i = 0; i < inrunPoints.Length; i++)
                {
                    verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y) + new Vector3(0, hillInrunTrackVals[j].y, hillInrunTrackVals[j].x));
                    uvsList.Add(new Vector2(widths[j], lengths[i]));
                    verticesList.Add(new Vector3(inrunPoints[i].x, inrunPoints[i].y) + new Vector3(0, hillInrunTrackVals[j + 1].y, hillInrunTrackVals[j + 1].x));
                    uvsList.Add(new Vector2(widths[j + 1], lengths[i]));
                    cnt = verticesList.Count - 1;

                    if (i > 0)
                    {
                        trianglesList[j].Add(cnt - 3);
                        trianglesList[j].Add(cnt - 1);
                        trianglesList[j].Add(cnt);

                        trianglesList[j].Add(cnt - 3);
                        trianglesList[j].Add(cnt);
                        trianglesList[j].Add(cnt - 2);
                    }
                }

            }

            mesh.vertices = verticesList.ToArray();
            mesh.uv = uvsList.ToArray();
            for (int i = 0; i < segmentsCount; i++)
                mesh.SetTriangles(trianglesList[i].ToArray(), i);

            mesh.RecalculateNormals();
        
            return mesh;
        }

        public Material[] GetMaterials()
        {
            return materials;
        }

        private Vector2[] GenerateRealVals(float b1)
        {
            Vector2[] tmp = new Vector2[inrunTrackVals.Length + 2];
            tmp[0] = new Vector2(-b1 / 2, inrunTrackVals[0].y);
            tmp[tmp.Length - 1] = new Vector2(b1 / 2, inrunTrackVals[inrunTrackVals.Length - 1].y);
            for (int i = 0; i < inrunTrackVals.Length; i++)
            {
                tmp[i + 1] = inrunTrackVals[i];
            }
            return tmp;
        }

        private float[] GeneratePrefLenghts(Vector2[] vals)
        {
            float[] result = new float[vals.Length];
            result[0] = 0;
            for (int i = 1; i < vals.Length; i++)
            {
                result[i] = result[i - 1] + (vals[i] - vals[i - 1]).magnitude;
            }
            return result;
        }

    }
}