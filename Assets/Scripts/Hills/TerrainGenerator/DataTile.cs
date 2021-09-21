using System;
using UnityEngine;

namespace OpenSkiJumping.Hills.TerrainGenerator
{
    [Serializable]
    public class DataTile
    {
        public DataTile(short[,] heights, Vector2Int coords, int resolution, string code)
        {
            Heights = heights;
            this.coords = coords;
            this.resolution = resolution;
            tileCode = code;
        }

        public string tileCode;
        public short[,] Heights { get; }
        public int resolution;

        public Vector2Int coords;

        public float GetHeight(int x, int y)
        {
            var mx = 3600 / resolution;
            var trueX = Mathf.Clamp(mx - x, 0, mx);
            var trueY = Mathf.Clamp(y, 0, mx);
            return Heights[trueX, trueY];
        }

        public float GetInterpolatedHeight(Vector2 point)
        {
            point = CoordsMap(point);
            var x0 = Mathf.FloorToInt(point.x);
            var x1 = Mathf.CeilToInt(point.x);

            var y0 = Mathf.FloorToInt(point.y);
            var y1 = Mathf.CeilToInt(point.y);

            var xd = point.x - x0;
            var yd = point.y - y0;

            // if (x0 == x1 && y0 == y1) return Heights[x0, y0];

            var h = new float[4][];

            h[0] = new[]
            {
                GetHeight(x0 - 1, y0 - 1),
                GetHeight(x0 - 1, y0),
                GetHeight(x0 - 1, y1),
                GetHeight(x0 - 1, y1 + 1)
            };
            h[1] = new[]
            {
                GetHeight(x0, y0 - 1),
                GetHeight(x0, y0),
                GetHeight(x0, y1),
                GetHeight(x0, y1 + 1)
            };
            h[2] = new[]
            {
                GetHeight(x1, y0 - 1),
                GetHeight(x1, y0),
                GetHeight(x1, y1),
                GetHeight(x1, y1 + 1)
            };
            h[3] = new[]
            {
                GetHeight(x1 + 1, y0 - 1),
                GetHeight(x1 + 1, y0),
                GetHeight(x1 + 1, y1),
                GetHeight(x1 + 1, y1 + 1)
            };

            // var d00 = 1.0f / (point.x - x0 + point.y - y0);
            // var d01 = 1.0f / (point.x - x0 + y1 - point.y);
            // var d10 = 1.0f / (x1 - point.x + point.y - y0);
            // var d11 = 1.0f / (x1 - point.x + y1 - point.y);

            // return (h00 * d00 + h01 * d01 + h10 * d10 + h11 * d11) / (d00 + d01 + d10 + d11);
            // var l0 = Mathf.Lerp(h00, h10, point.x - x0);
            // var l1 = Mathf.Lerp(h01, h11, point.x - x0);
            // return Mathf.Lerp(l0, l1, point.y - y0);

            // return BilinearInterpolation(h[1][1], h[1][2], h[2][1], h[2][2], xd, yd);
            return BicubicInterpolation(h, xd, yd);
        }

        public float GetInterpolatedHeight(float x, float y)
        {
            return GetInterpolatedHeight(new Vector2(x, y));
        }

        public static float BilinearInterpolation(float a00, float a01, float a10, float a11, float x, float y)
        {
            var l0 = Mathf.Lerp(a00, a10, x);
            var l1 = Mathf.Lerp(a01, a11, x);
            return Mathf.Lerp(l0, l1, y);
        }

        public static float CubicInterpolation(float[] p, float t)
        {
            return p[1] + 0.5f * t * (p[2] - p[0] +
                                      t * (2f * p[0] - 5f * p[1] + 4f * p[2] - p[3] +
                                           t * (3f * (p[1] - p[2]) + p[3] - p[0])));
        }

        public static float BicubicInterpolation(float[][] p, float x, float y)
        {
            var arr = new float[]
            {
                CubicInterpolation(p[0], y),
                CubicInterpolation(p[1], y),
                CubicInterpolation(p[2], y),
                CubicInterpolation(p[3], y)
            };
            return CubicInterpolation(arr, x);
        }

        public Vector2 CoordsMap(Vector2 point)
        {
            return (point - coords) * 3600.0f / resolution;
        }
    }
}