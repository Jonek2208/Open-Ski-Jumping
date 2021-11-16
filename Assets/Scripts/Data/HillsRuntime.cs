using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/HillsRuntime")]
    public class HillsRuntime : DatabaseObject<List<ProfileData>>
    {
        public HillInfo GetHillInfo(string hillName)
        {
            var hill = Data.Find(it => it.name == hillName);

            return new HillInfo((decimal) hill.w, (decimal) (hill.w + hill.l2), 0, 0, 0,
                (decimal) hill.es / (hill.gates - 1));
        }

        public ProfileData GetProfileData(string hillName)
        {
            var hill = Data.Find(it => it.name == hillName);
            return hill;
        }

        public IEnumerable<ProfileData> GetSortedData()
        {
            return Data.OrderBy(it => it.name, StringComparer.InvariantCulture);
        }

        public void Add(ProfileData item)
        {
            data.Add(item);
        }

        public bool Remove(ProfileData item)
        {
            return data.Remove(item);
        }

        public override void SaveData()
        {
            data.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.InvariantCulture));

            var directoryPath = System.IO.Path.Combine(Application.streamingAssetsPath, "hills");
            foreach (var itx in data)
            {
                var it = new HillsMap {profiles = new List<MapHillData> {new MapHillData {profileData = itx}}};
                var absolutePath = System.IO.Path.Combine(directoryPath, $"{itx.name.ToLower().Replace(" ", "_")}.json");
                var dataAsJson =
                    JsonConvert.SerializeObject(it, prettyPrint);
                File.WriteAllText(absolutePath, dataAsJson);
            }

            base.SaveData();
        }

        public override bool LoadData()
        {
            var tmp = base.LoadData();
            // data = new List<ProfileData>();
            // var tmp = LoadMultipleFiles(path);
            return tmp;
        }

        private bool LoadMultipleFiles(string absolutePath)
        {
            if (!Directory.Exists(absolutePath))
            {
                return false;
            }

            var fileEntries = Directory.GetFiles(absolutePath);
            foreach (var fileName in fileEntries)
                LoadSingleFile(fileName);

            // string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            // foreach (string subdirectory in subdirectoryEntries)
            //     ProcessDirectory(subdirectory);
            return true;
        }

        private bool LoadSingleFile(string absolutePath)
        {
            if (File.Exists(absolutePath))
            {
                var dataAsJson = File.ReadAllText(absolutePath);
                var hill = JsonConvert.DeserializeObject<ProfileData>(dataAsJson);
                data.Add(hill);
                loaded = true;
                return true;
            }

            loaded = false;
            return false;
        }
    }
}