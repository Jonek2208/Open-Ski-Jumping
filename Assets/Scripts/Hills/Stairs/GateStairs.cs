using System.Collections.Generic;
using UnityEngine;

namespace OpenSkiJumping.Hills.Stairs
{
    [CreateAssetMenu(menuName = "HillElements/GateStairs")]
    public class GateStairs : ScriptableObject
    {
        [SerializeField]
        float stepAngleDeg;

        [SerializeField]
        float stepWidth;

        [SerializeField]
        private Material material;

        public float StepWidth { get => stepWidth; set => stepWidth = value; }

        public Mesh Generate(int side, Vector2 A, Vector2 B, float b1, int gates)
        {
            /* 0 - Left, 1 - Right */
            Mesh mesh = new Mesh();
            List<Vector3> verticesList = new List<Vector3>();
            List<Vector2> uvsList = new List<Vector2>();
            List<int> trianglesList = new List<int>();


            float offset = ((side == 1) ? (StepWidth + b1) : 0);
            float extra = (A - B).y / (gates - 1) * Mathf.Tan(Mathf.Deg2Rad * stepAngleDeg);

            for (int i = 0; i < gates + 1; i++)
            {
                Vector2 pos = B + (A - B) * (i / (float)(gates - 1));
                Vector2 pos0 = B + (A - B) * ((i - 1) / (float)(gates - 1));

                verticesList.Add(new Vector3(pos0.x, pos0.y, -(b1 / 2 + StepWidth) + offset));
                uvsList.Add(new Vector2((pos0 - B).x + (pos0 - B).y, 0));
                verticesList.Add(new Vector3(pos0.x, pos0.y, -b1 / 2 + offset));
                uvsList.Add(new Vector2((pos0 - B).x + (pos0 - B).y, StepWidth));
                verticesList.Add(new Vector3(pos0.x + extra, pos.y, -(b1 / 2 + StepWidth) + offset));
                uvsList.Add(new Vector2((pos0 - B).x + (pos - B).y, 0));
                verticesList.Add(new Vector3(pos0.x + extra, pos.y, -b1 / 2 + offset));
                uvsList.Add(new Vector2((pos0 - B).x + (pos - B).y, StepWidth));

                verticesList.Add(new Vector3(pos0.x + extra, pos.y, -(b1 / 2 + StepWidth) + offset));
                uvsList.Add(new Vector2((pos0 - B).x + (pos - B).y, 0));
                verticesList.Add(new Vector3(pos0.x + extra, pos.y, -b1 / 2 + offset));
                uvsList.Add(new Vector2((pos0 - B).x + (pos - B).y, StepWidth));
                verticesList.Add(new Vector3(pos.x, pos.y, -(b1 / 2 + StepWidth) + offset));
                uvsList.Add(new Vector2((pos - B).x + (pos - B).y, 0));
                verticesList.Add(new Vector3(pos.x, pos.y, -b1 / 2 + offset));
                uvsList.Add(new Vector2((pos - B).x + (pos - B).y, StepWidth));

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


            for (int i = 0; i < gates + 2; i++)
            {
                Vector2 pos = B + (A - B) * ((i - 1) / (float)(gates - 1));
                Vector2 pos0 = B + (A - B) * ((i - 2) / (float)(gates - 1));

                if (i > 0)
                {
                    verticesList.Add(new Vector3(pos0.x + extra, pos.y, -(b1 / 2 + StepWidth) + offset));
                    uvsList.Add(new Vector2((pos0 - B).x, (pos - B).y));
                    verticesList.Add(new Vector3(pos0.x + extra, pos.y, -b1 / 2 + offset));
                    uvsList.Add(new Vector2((pos0 - B).x, (pos - B).y));
                }

                verticesList.Add(new Vector3(pos.x, pos.y, -(b1 / 2 + StepWidth) + offset));
                uvsList.Add(new Vector2((pos - B).x, (pos - B).y));
                verticesList.Add(new Vector3(pos.x, pos.y, -b1 / 2 + offset));
                uvsList.Add(new Vector2((pos - B).x, (pos - B).y));

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



            // for (int i = 0; i < pos.Length - 1; i++)
            // {
            //     if (i > 0)
            //     {
            //         float deltaY = pos[i].y - pos[i - 1].y, deltaX = pos[i].x - pos[i - 1].x;
            //         verticesList.Add(new Vector3(pos[i - 1].x, pos[i].y, -(profileData.b1 / 2 + width) + offset));
            //         uvsList.Add(new Vector2(pos[i - 1].x, pos[i].y));
            //         verticesList.Add(new Vector3(pos[i - 1].x, pos[i].y, -profileData.b1 / 2 + offset));
            //         uvsList.Add(new Vector2(pos[i - 1].x, pos[i].y));
            //     }


            //     verticesList.Add(new Vector3(pos[i].x, pos[i].y, -(profileData.b1 / 2 + width) + offset));
            //     uvsList.Add(new Vector2(pos[i].x, pos[i].y));
            //     verticesList.Add(new Vector3(pos[i].x, pos[i].y, -profileData.b1 / 2 + offset));
            //     uvsList.Add(new Vector2(pos[i].x, pos[i].y));

            //     int x = verticesList.Count;
            //     if (i > 0)
            //     {
            //         trianglesList.Add(x - 5);
            //         trianglesList.Add(x - 3);
            //         trianglesList.Add(x - 1);

            //         trianglesList.Add(x - 6);
            //         trianglesList.Add(x - 2);
            //         trianglesList.Add(x - 4);
            //     }

            // }


            mesh.vertices = verticesList.ToArray();
            mesh.uv = uvsList.ToArray();
            mesh.triangles = trianglesList.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        public Material GetMaterial()
        {
            return material;
        }


    }
}