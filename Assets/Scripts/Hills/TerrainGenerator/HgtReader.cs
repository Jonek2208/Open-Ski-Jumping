using System.IO;

namespace OpenSkiJumping.Hills.TerrainGenerator
{
    public static class HgtReader
    {
        
        private const int SrtmSize = 1201;
        public static short[,] ReadFile(string path)
        {
            var height = new short[SrtmSize, SrtmSize];
            var buffer = File.ReadAllBytes(path);

            for (var i = 0; i < SrtmSize; i++)
            {
                for (var j = 0; j < SrtmSize; j++)
                {
                    height[i, j] = (short) (buffer[2 * (i * SrtmSize + j)] << 8 | buffer[2 * (i * SrtmSize + j) + 1]);
                }
            }

            return height;
        }
    }
}