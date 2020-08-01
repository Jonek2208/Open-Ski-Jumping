using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    public abstract class RuntimeData : ScriptableObject
    {
        public abstract bool LoadData();
        public abstract void SaveData();
        public abstract void Reset();
    }

    public class DatabaseObject<T> : RuntimeData
    {
        [SerializeField] protected string fileName;
        [SerializeField] protected T data;
        [SerializeField] protected bool loaded;

        public T Data
        {
            get => data;
            protected set => data = value;
        }

        public bool Loaded
        {
            get => loaded;
            set => loaded = value;
        }

        public override void Reset()
        {
            data = default;
            loaded = false;
        }

        public override bool LoadData()
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(filePath))
            {
                var dataAsJson = File.ReadAllText(filePath);
                data = JsonConvert.DeserializeObject<T>(dataAsJson);
                loaded = true;
                return true;
            }

            loaded = false;
            return false;
        }

        public override void SaveData()
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            var dataAsJson = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, dataAsJson);
        }
    }
}