using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Newtonsoft.Json;
using OpenSkiJumping.Hills;
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

    public static class DatabaseIO
    {
        public enum DataType
        {
            Json,
            Xml,
            Binary
        }

        public static readonly string[] Extensions = {".json", ".xml", ".osj"};
        public static string GetExtension(DataType dataType) => Extensions[(int) dataType];

        public static bool LoadJsonFile<T>(string path, out T data)
        {
            if (File.Exists(path))
            {
                var dataAsJson = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<T>(dataAsJson);
                return true;
            }

            data = default;
            return false;
        }

        public static void SaveJsonFile<T>(string path, T data, Formatting formatting = Formatting.None)
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, path);
            var dataAsJson = JsonConvert.SerializeObject(data, formatting);
            File.WriteAllText(filePath, dataAsJson);
        }

        public static bool LoadXmlFile<T>(string path, out T data)
        {
            if (File.Exists(path))
            {
                var serializer = new XmlSerializer(typeof(T));
                var fs = new FileStream(path, FileMode.Open);
                data = (T) serializer.Deserialize(fs);
                fs.Close();
                return true;
            }

            data = default;
            return false;
        }

        public static void SaveXmlFile<T>(string path, T data)
        {
            var serializer = new XmlSerializer(typeof(T));
            TextWriter writer = new StreamWriter(path);
            serializer.Serialize(writer, data);
            writer.Close();
        }

        public static bool LoadBinaryFile<T>(string path, out T data) where T : class
        {
            if (File.Exists(path))
            {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(path, FileMode.Open);
                data = formatter.Deserialize(stream) as T;
                stream.Close();
                return true;
            }

            data = default;
            return false;
        }

        public static void SaveBinaryFile<T>(string path, T data)
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    public class DatabaseObject<T> : RuntimeData
    {
        [FormerlySerializedAs("fileName")] [SerializeField]
        protected string path;

        [SerializeField] protected DatabaseIO.DataType dataType;

        [SerializeField] protected T data;
        [SerializeField] protected bool loaded;
        [SerializeField] protected Formatting prettyPrint;


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
                var serializedData = File.ReadAllText(absolutePath);
                data = JsonConvert.DeserializeObject<T>(serializedData);
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
            var dataAsJson = JsonConvert.SerializeObject(data, prettyPrint);
            File.WriteAllText(filePath, dataAsJson);
        }
    }
}