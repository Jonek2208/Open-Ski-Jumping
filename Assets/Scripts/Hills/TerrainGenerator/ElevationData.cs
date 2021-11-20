using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;

namespace OpenSkiJumping.Hills.TerrainGenerator
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ElevationData")]
    public class ElevationData : ScriptableObject
    {
        [SerializeField] private string rootFolder;

        [FormerlySerializedAs("path")] [SerializeField]
        public List<DataTile> data;

        private Dictionary<Vector2Int, DataTile> _dataDict = new();

        private const int SrtmSize = 1201;
        private const int Resolution = 3;

        public static string GetTileCodeFromFileName(string tileCode)
        {
            var match = Regex.Match(tileCode, "([NS])([0-9][0-9])([EW])([0-1][0-9][0-9])");
            return match.Groups[0].Value;
        }
        public static Vector2Int GetCoordsFromFileName(string tileCode)
        {
            var match = Regex.Match(tileCode, "([NS])([0-9][0-9])([EW])([0-1][0-9][0-9])");
            var latSymbol = match.Groups[1].Value;
            var latValue = int.Parse(match.Groups[2].Value);
            var latSign = latSymbol == "S" ? -1 : 1;

            var lonSymbol = match.Groups[3].Value;
            var lonValue = int.Parse(match.Groups[4].Value);
            var lonSign = latSymbol == "S" ? -1 : 1;

            return new Vector2Int(latSign * latValue, lonSign * lonValue);
        }

        public static bool LoadHgtFile(string absolutePath, out DataTile result)
        {
            result = default;
            if (!File.Exists(absolutePath)) return false;

            var fileName = Path.GetFileName(absolutePath);
            var coords = GetCoordsFromFileName(fileName);
            var buffer = File.ReadAllBytes(absolutePath);
            var tileCode = GetTileCodeFromFileName(fileName);

            var heights = new short[SrtmSize, SrtmSize];
            for (var i = 0; i < SrtmSize; i++)
            {
                for (var j = 0; j < SrtmSize; j++)
                {
                    heights[i, j] = (short) (buffer[2 * (i * SrtmSize + j)] << 8 | buffer[2 * (i * SrtmSize + j) + 1]);
                }
            }

            result = new DataTile(heights, coords, Resolution, tileCode);
            return true;
        }

        public int ReadFiles()
        {
            var absolutePath = Path.Combine(Application.streamingAssetsPath, rootFolder);
            if (!Directory.Exists(absolutePath)) return 0;

            var fileEntries = Directory.GetFiles(absolutePath);
            data.Clear();
            _dataDict.Clear();

            foreach (var fileName in fileEntries)
            {
                var extension = Path.GetExtension(fileName);
                if (extension != ".hgt") continue;
                var tmp = LoadHgtFile(fileName, out var item);
                if (!tmp) continue;
                data.Add(item);
                _dataDict.Add(item.coords, item);
            }

            return data.Count;
        }

        public bool GetTileContainingCoord(Vector2 coord, out DataTile result)
        {
            var latLo = Mathf.FloorToInt(coord.x);
            var latHi = Mathf.CeilToInt(coord.x);

            var lonLo = Mathf.FloorToInt(coord.y);
            var lonHi = Mathf.CeilToInt(coord.y);

            var targets = new List<Vector2Int>();

            for (var i = 0; i < 2 - (latHi - latLo); i++)
            {
                for (var j = 0; j < 2 - (lonHi - lonLo); j++)
                {
                    targets.Add(new Vector2Int(latLo + i, lonLo + j));
                }
            }

            result = default;

            foreach (var target in targets)
            {
                if (!_dataDict.ContainsKey(target)) continue;
                result = _dataDict[target];
                return true;
            }

            return false;
        }

        public bool GetElevation(Vector2 coords, out float elevation)
        {
            elevation = 0;
            var tmp = GetTileContainingCoord(coords, out var dataTile);
            if (!tmp) return false;
            elevation = dataTile.GetInterpolatedHeight(coords);
            return true;
        }

        private void OnEnable()
        {
            ReadFiles();
        }
    }
}