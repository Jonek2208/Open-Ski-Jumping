using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    public class OffsetFunction
    {
        private readonly List<Vector3> _points;
        private readonly int _n;
        private readonly float _minX, _maxX;

        public OffsetFunction(IEnumerable<Vector3> points)
        {
            _points = ProjectPathToFunction(points).ToList();
            _n = _points.Count;
            _minX = _points[0].x;
            _maxX = _points[_n - 1].x;
        }

        public static OffsetFunction FromRenderedPath(RenderedPath renderedPath)
        {
            return renderedPath == null ? Zero : new OffsetFunction(renderedPath.data);
        }

        public static OffsetFunction Zero => new(new[] {Vector3.zero, Vector3.right});

        public static IEnumerable<Vector3> ProjectPathToFunction(IEnumerable<Vector3> points)
        {
            float? minVal = null;
            foreach (var p in points)
            {
                if (minVal.HasValue && p.x <= minVal) continue;
                minVal = p.x;
                yield return p;
            }
        }

        public Vector3 EvalAbs(float t)
        {
            if (t < _minX) return _points[0];
            if (t > _maxX) return _points[_n - 1];

            var l = 1;
            var r = _n - 1;
            var i = 1;
            while (l <= r)
            {
                var mid = (l + r) / 2;
                if (_points[mid].x < t)
                {
                    l = mid + 1;
                }
                else if (_points[mid].x > t)
                {
                    r = mid - 1;
                    i = mid;
                }
                else
                {
                    return _points[mid];
                }
            }

            var lo = _points[i - 1].x;
            var hi = _points[i].x;
            var u = (t - lo) / (hi - lo);
            return Vector3.Lerp(_points[i - 1], _points[i], u);
        }

        public Vector3 EvalNorm(float t)
        {
            return EvalAbs(Mathf.Lerp(_minX, _maxX, t));
        }
    }
}