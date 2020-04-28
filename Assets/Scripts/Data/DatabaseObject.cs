using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    public abstract class RuntimeData : ScriptableObject
    {
        public abstract bool LoadData();
        public abstract void SaveData();
    }

    public class DatabaseObject<T> : RuntimeData
    {
        [SerializeField] protected string fileName;
        [SerializeField] protected T data;
        [SerializeField] protected bool loaded;

        public T Data { get => data; set => data = value; }
        public bool Loaded { get => loaded; set => loaded = value; }

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