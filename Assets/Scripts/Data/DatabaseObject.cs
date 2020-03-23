using System.IO;
using Newtonsoft.Json;
using UnityEngine;

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
        string filePath = Path.Combine(Application.streamingAssetsPath, this.fileName);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            this.data = JsonConvert.DeserializeObject<T>(dataAsJson);
            this.loaded = true;
            return true;
        }
        this.loaded = false;
        return false;
    }

    public override void SaveData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, this.fileName);
        string dataAsJson = JsonConvert.SerializeObject(this.data);
        File.WriteAllText(filePath, dataAsJson);
    }
}
