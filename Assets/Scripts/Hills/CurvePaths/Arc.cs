using UnityEngine;

namespace OpenSkiJumping.Hills.CurvePaths
{
    public class Arc : ICurvePathSegment
    {
        private readonly Vector3 _p0, _p1, _c;

        private Arc(Vector3 p0, Vector3 p1, Vector3 c)
        {
            _p0 = p0;
            _p1 = p1;
            _c = c;
            Length = GetArcLength(_p0 - c, _p1 - c, (_p0 - c).magnitude);
        }

        private static float GetArcLength(Vector3 u, Vector3 v, float radius)
        {
            var angle = Vector3.Angle(u, v);
            return angle * Mathf.Deg2Rad * radius;
        }


        // public static Arc FromThreePoints(Vector3 p0, Vector3 p1, Vector3 p2)
        // {
        //     return new Arc();
        // }

        public static Arc FromCenterPoint(Vector3 p0, Vector3 p1, Vector3 c)
        {
            return new(p0, p1, c);
        }

        public Vector3 Eval(float t)
        {
            return Vector3.Slerp(_p0 - _c, _p1 - _c, t) + _c;
        }

        public float Length { get; }
    }
}