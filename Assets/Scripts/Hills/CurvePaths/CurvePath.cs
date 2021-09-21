using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace OpenSkiJumping.Hills.CurvePaths
{
    public class CurvePath
    {
        private readonly List<ICurvePathSegment> _segments = new List<ICurvePathSegment>();
        private readonly List<Vector3> _points = new List<Vector3>();
        private readonly List<float> _lengths = new List<float>();
        private bool _started = false;

        public void AddStartPoint(Vector3 p)
        {
            _points.Add(p);
            _lengths.Add(0);
            _started = true;
        }

        private void AddHelper(ICurvePathSegment segment, Vector3 endPoint)
        {
            if (!_started)
            {
                AddStartPoint(endPoint);
                return;
            }

            _segments.Add(segment);
            _points.Add(endPoint);
            _lengths.Add(Length + segment.Length);
        }

        private Vector3 LastPoint => _points.Count > 0 ? _points[_points.Count - 1] : Vector3.zero;

        public void AddArc(Vector3 p, Vector3 c)
        {
            AddHelper(Arc.FromCenterPoint(LastPoint, p, c), p);
        }

        public void AddLine(Vector3 p)
        {
            AddHelper(new Line(LastPoint, p), p);
        }

        public void AddBezier3(Vector3 c0, Vector3 c1, Vector3 p)
        {
            AddHelper(new Bezier3(LastPoint, c0, c1, p), p);
        }

        public void AddBezier2(Vector3 c, Vector3 p)
        {
            AddHelper(Bezier3.FromBezier2(LastPoint, c, p), p);
        }

        public float Length => _lengths[_lengths.Count - 1];

        public Vector3 EvalByLength(float l)
        {
            var it = 0;
            while (_lengths[it + 1] <= l && it < _segments.Count - 1) it++;
            var v0 = _lengths[it];
            var v1 = _lengths[it + 1];
            var u = (l - v0) / (v1 - v0);
            return _segments[it].Eval(u);
        }

        public Vector3 Eval(float t)
        {
            return EvalByLength(t * Length);
        }
    }
}