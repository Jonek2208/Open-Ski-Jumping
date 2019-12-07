using System.IO;
using Newtonsoft.Json;
using UnityEngine;
public class DatabaseObjectJSON<T> : DatabaseObject
{
    public string fileName;
    public T data;
    public bool loaded;

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