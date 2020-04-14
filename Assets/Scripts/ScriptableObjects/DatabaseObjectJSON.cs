using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ScriptableObjects
{
    public class DatabaseObjectJSON<T> : DatabaseObject
    {
        public string fileName;
        public T data;
        public bool loaded;

        public override bool LoadData()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                data = JsonConvert.DeserializeObject<T>(dataAsJson);
                loaded = true;
                return true;
            }
            loaded = false;
            return false;
        }

        public override void SaveData()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            string dataAsJson = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, dataAsJson);
        }
    }
}