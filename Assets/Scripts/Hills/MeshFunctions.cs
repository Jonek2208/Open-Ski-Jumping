using System.Collections.Generic;

namespace OpenSkiJumping.Hills
{
    public class MeshFunctions
    {
        public static int[] FacesToTriangles(List<(int, int, int, int)> facesList)
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
    }
}