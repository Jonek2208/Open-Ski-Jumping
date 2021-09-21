using UnityEngine;

namespace OpenSkiJumping.Hills.CurvePaths
{
    public interface ICurvePathSegment
    {
        Vector3 Eval(float t);
        float Length { get; }
    }
}