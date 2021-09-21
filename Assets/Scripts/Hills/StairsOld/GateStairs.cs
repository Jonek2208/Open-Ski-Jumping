using System.Collections.Generic;
using UnityEngine;

namespace OpenSkiJumping.Hills.StairsOld
{
    [CreateAssetMenu(menuName = "HillElements/GateStairs")]
    public class GateStairs : ScriptableObject
    {
        [SerializeField] float stepAngleDeg;

        [SerializeField] float stepWidth;

        [SerializeField] private Material material;

        public float StepWidth
        {
            get => stepWidth;
            set => stepWidth = value;
        }

        public Mesh Generate(int side, Vector2 A, Vector2 B, float b1, int gates)
        {
            /* 0 - Left, 1 - Right */
            var mesh = new Mesh();
            var verticesList = new List<Vector3>();
            var uvsList = new List<Vector2>();
            var trianglesList = new List<int>();


            var offset = ((side == 1) ? (StepWidth + b1) : 0);
            var extra = (A - B).y / (gates - 1) * Mathf.Tan(Mathf.Deg2Rad * stepAngleDeg);

            for (var i = 0; i < gates; i++)
            {
                var pos = B + (A - B) * (i / (float) (gates - 1));
                var pos0 = B + (A - B) * ((i - 1) / (float) (gates - 1));

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

                var x = verticesList.Count;
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


            for (var i = 0; i < gates + 1; i++)
            {
                var pos = B + (A - B) * ((i - 1) / (float) (gates - 1));
                var pos0 = B + (A - B) * ((i - 2) / (float) (gates - 1));

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

                var x = verticesList.Count;
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