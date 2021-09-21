using UnityEngine;

namespace OpenSkiJumping.Hills.CurvePaths
{
    public class Line : ICurvePathSegment
    {
        private readonly Vector3 _p0, _p1;

        public Line(Vector3 p0, Vector3 p1)
        {
            _p0 = p0;
            _p1 = p1;
            Length = (p0 - p1).magnitude;
        }

        public Vector3 Eval(float t) => Vector3.Lerp(_p0, _p1, t);
        public float Length { get; }
    }
}