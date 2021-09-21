using UnityEngine;

namespace OpenSkiJumping.Hills.CurvePaths
{
    public static class Interpolation
    {
        public static Vector3 Lerp2(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(a, b, t), Vector3.Lerp(b, c, t), t);
        }

        public static Vector3 Lerp3(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return Vector3.Lerp(Lerp2(a, b, c, t), Lerp2(b, c, d, t), t);
        }
    }
}