using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace OpenSkiJumping.Data

{
    [Serializable]
    public class DatabaseObjectFileData<T>
    {
        public string fileName;
        public T value;

        public DatabaseObjectFileData(T value, string fileName)
        {
            this.value = value;
            this.fileName = fileName;
        }
    }

    public class DatabaseObjectMultipleFiles<T> : DatabaseObject<List<DatabaseObjectFileData<T>>>
    {
        protected Dictionary<string, int> _map = new Dictionary<string, int>();

        private void SetUpDict()
        {
            _map.Clear();
            for (var i = 0; i < data.Count; i++)
            {
                var it = data[i];
                _map.Add(it.fileName, i);
            }
        }

        public override bool LoadData()
        {
            var absolutePath = Path.Combine(Application.streamingAssetsPath, path);
            var tmp = LoadMultipleFiles(absolutePath);
            SetUpDict();
            return tmp;
        }

        private bool LoadMultipleFiles(string absolutePath)
        {
            if (!Directory.Exists(absolutePath))
            {
                return false;
            }

            data.Clear();
            var fileEntries = Directory.GetFiles(absolutePath);
            foreach (var fileName in fileEntries)
            {
                var extension = Path.GetExtension(fileName);
                if (extension != ".json") continue;
                var tmp = LoadSingleFile(fileName, newObject: out var item);
                if (!tmp) continue;
                data.Add(new DatabaseObjectFileData<T>(item, Path.GetFileName(fileName)));
            }

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
            var directoryPath = Path.Combine(Application.streamingAssetsPath, path);
            foreach (var it in data)
            {
                var absolutePath = Path.Combine(directoryPath, it.fileName);
                var dataAsJson =
                    JsonConvert.SerializeObject(it.value, prettyPrint ? Formatting.Indented : Formatting.None);
                File.WriteAllText(absolutePath, dataAsJson);
            }
        }
    }
}