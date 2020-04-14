using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SavesSystem
{
    const string dataFileName = "saves.osj";
    public static void Save(SaveData saveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, dataFileName);
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, saveData);
        stream.Close();
        Debug.Log("Data saved to: " + dataFileName);
    }

    public static SaveData Load()
    {
        string path = Path.Combine(Application.persistentDataPath, dataFileName);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData saveData = formatter.Deserialize(stream) as SaveData; 
            stream.Close();
            Debug.Log(saveData.savesList.Count);
            foreach (var item in saveData.savesList)
            {
                Debug.Log(item.name + " " + item.calendar.name);
            }
            return saveData;
        }

        Debug.Log("Loading saves error");
        return null;
    }
}
