using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("fileName")] [SerializeField]
        protected string path;

        [SerializeField] protected T data;
        [SerializeField] protected bool loaded;
        [SerializeField] protected bool prettyPrint;


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
            var absolutePath = Path.Combine(Application.streamingAssetsPath, path);
            if (File.Exists(absolutePath))
            {
                var dataAsJson = File.ReadAllText(absolutePath);
                data = JsonConvert.DeserializeObject<T>(dataAsJson);
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
                data = JsonConvert.DeserializeObject<T>(dataAsJson);
                loaded = true;
                return true;
            }

            loaded = false;
            return false;
        }

        public override void SaveData()
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, path);
            var dataAsJson = JsonConvert.SerializeObject(data, prettyPrint ? Formatting.Indented : Formatting.None);
            File.WriteAllText(filePath, dataAsJson);
        }
    }
}