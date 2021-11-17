using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.Hills.CurvePaths
{
    public class LengthFunction
    {
        private readonly List<float> _args, _values;
        private readonly int _n;

        public LengthFunction(List<float> args, IReadOnlyList<Vector3> points)
        {
            _args = args;
            _n = Mathf.Min(args.Count, points.Count);
            _values = Calculate(args, points);
            Length = _values[_n - 1];
            for (var i = 0; i < _n; i++) _values[i] /= Length;
        }

        private List<float> Calculate(List<float> args, IReadOnlyList<Vector3> points)
        {
            var res = new List<float> {0};
            for (var i = 1; i < _n; i++)
                res.Add(res[i - 1] + (points[i] - points[i - 1]).magnitude);
            return res;
        }

        private float EvalHelper(float t, IReadOnlyList<float> args, IReadOnlyList<float> values)
        {
            var i = 1;
            while (args[i] < t && i < _n)
                i++;
            var lo = args[i - 1];
            var hi = args[i];
            var u = (t - lo) / (hi - lo);
            return Mathf.Lerp(values[i - 1], values[i], u);
        }

        public float Eval(float t)
        {
            return EvalHelper(t, _args, _values);
        }

        public float EvalInverse(float t)
        {
            return EvalHelper(t, _values, _args);
        }

        public float Length { get; }
    }

    public class Bezier3 : ICurvePathSegment
    {
        private readonly Vector3 _p0, _c0, _c1, _p1;
        private readonly LengthFunction _lengthFunction;

        public Bezier3(Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1)
        {
            _p0 = p0;
            _c0 = c0;
            _c1 = c1;
            _p1 = p1;
            _lengthFunction = GetLengthFunction(100);
            Length = _lengthFunction.Length;
        }

        public static Bezier3 FromBezier2(Vector3 p0, Vector3 c, Vector3 p1)
        {
            var c0 = Vector3.Lerp(p0, c, (float) 2 / 3);
            var c1 = Vector3.Lerp(p1, c, (float) 2 / 3);
            return new Bezier3(p0, c0, c1, p1);
        }

        public Vector3 EvalRaw(float t)
        {
            return Interpolation.Lerp3(_p0, _c0, _c1, _p1, t);
        }

        private LengthFunction GetLengthFunction(int segments)
        {
            var args = Enumerable.Range(0, segments + 1).Select(i => (float) i / segments).ToList();
            var points = args.Select(EvalRaw).ToList();
            return new LengthFunction(args, points);
        }

        public Vector3 Eval(float t)
        {
            return EvalRaw(_lengthFunction.EvalInverse(t));
        }

        public float Length { get; }
    }
}