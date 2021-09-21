using UnityEngine;

namespace OpenSkiJumping.Hills.TerrainGenerator
{
    public static class Geodetic
    {
        public const float R = 6371009;

        public static Vector3 Geog2Ecef(Vector3 point, bool radians)
        {
            var scalar = radians ? 1.0f : Mathf.Deg2Rad;
            var phi = scalar * point.x;
            var lam = scalar * point.y;
            var height = point.z;

            var coords = new Vector3(Mathf.Cos(phi) * Mathf.Cos(lam), Mathf.Sin(phi), Mathf.Cos(phi) * Mathf.Sin(lam));
            return (R + height) * coords;
        }

        public static Vector3 Ecef2Geog(Vector3 point, bool radians)
        {
            var scalar = radians ? 1.0f : Mathf.Rad2Deg;

            var pointMagnitude = point.magnitude;
            point /= pointMagnitude;

            var height = pointMagnitude - R;

            var phi = Mathf.Asin(point.y) * scalar;
            var lam = Mathf.Atan2(point.z, point.x) * scalar;
            return new Vector3(phi, lam, height);
        }

        public static float GeoDist(Vector3 point1, Vector3 point2, bool radians)
        {
            return (Geog2Ecef(point1, radians) - Geog2Ecef(point2, radians)).magnitude;
        }

        public static Quaternion Rotation(float phi, float lam, float azimuth, bool radians)
        {
            var scalar = radians ? Mathf.Rad2Deg : 1.0f;
            phi *= scalar;
            lam *= scalar;
            azimuth *= scalar;
            return Quaternion.Euler(-phi + 90, 0, 0) * Quaternion.Euler(0, 90 + lam, 0);
        }
    }
}