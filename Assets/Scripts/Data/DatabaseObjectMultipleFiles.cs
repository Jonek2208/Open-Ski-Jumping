using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenSkiJumping.Hills;
using UnityEngine;
using UnityEngine.Serialization;

namespace OpenSkiJumping.Data
{
    public class DatabaseObjectMultipleFiles<T> : DatabaseObject<List<T>>
    {
        [SerializeField] protected new List<T> data;
        [SerializeField] private List<string> fileNames;
        

        public override bool LoadData()
        {
            var absolutePath = Path.Combine(Application.streamingAssetsPath, path);
            if (File.Exists(absolutePath))
            {
                var dataAsJson = File.ReadAllText(absolutePath);
                data = JsonConvert.DeserializeObject<List<T>>(dataAsJson);
                loaded = true;
                return true;
            }

            loaded = false;
            return false;
        }

        private bool LoadMultipleFiles(string absolutePath)
        {
            if (!Directory.Exists(absolutePath))
            {
                return false;
            }

            var fileEntries = Directory.GetFiles(absolutePath);
            foreach (var fileName in fileEntries)
            {
                LoadSingleFile(fileName, newObject: out var item);
                data.Add(item);
            }

            // string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            // foreach (string subdirectory in subdirectoryEntries)
            //     ProcessDirectory(subdirectory);
            return true;
        }

        private bool LoadSingleFile(string absolutePath, out T newObject)
        {
            newObject = default;
            if (!File.Exists(absolutePath)) return false;
            
            var dataAsJson = File.ReadAllText(absolutePath);
            newObject = JsonConvert.DeserializeObject<T>(dataAsJson);
            loaded = true;
            return true;
        }

        public override void SaveData()
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, path);
            var dataAsJson = JsonConvert.SerializeObject(data, prettyPrint ? Formatting.Indented : Formatting.None);
            File.WriteAllText(filePath, dataAsJson);
        }
    }
}