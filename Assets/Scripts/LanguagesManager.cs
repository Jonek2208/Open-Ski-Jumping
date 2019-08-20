using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using LanguagesSerialization;
using Newtonsoft.Json;

using JumpersSerialization;

public class LanguagesManager : MonoBehaviour
{
    GameDictionary gameDict;
    DictionaryData dictData;
    const string dataFileName = "dictionary.json";
    void LoadData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            DictionaryData loadedData = JsonUtility.FromJson<DictionaryData>(dataAsJson);
            dictData = loadedData;
        }
        else
        {
            Debug.LogError("No data!");
        }
    }
    void SaveData()
    {
        string dataAsJson = JsonUtility.ToJson(dictData);
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
        File.WriteAllText(filePath, dataAsJson);
    }

    void Test1()
    {
        Dictionary<string, Dictionary<string, string>> slownik = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, string> p1 = new Dictionary<string, string> { { "en", "Play" }, { "pl", "Graj" } };
        Dictionary<string, string> p2 = new Dictionary<string, string> { { "en", "Help" }, { "pl", "Pomoc" } };
        Dictionary<string, string> p3 = new Dictionary<string, string> { { "en", "Jumpers" }, { "pl", "Skoczkowie" } };
        Dictionary<string, string> p4 = new Dictionary<string, string> { { "en", "Hills" }, { "pl", "Skocznie" } };

        Dictionary<string, Dictionary<string, string>> slowniczek = new Dictionary<string, Dictionary<string, string>> { { "play", p1 }, { "help", p2 }, { "jumpers", p3 }, { "hills", p4 } };
        Debug.Log(JsonConvert.SerializeObject(slowniczek));
        DateTime dt = new DateTime(2018, 1, 19, 16, 00, 00);
        Debug.Log(dt.ToString());
        // Debug.Log(dictData.phrases);
        SaveData();


        LoadData();

        // Debug.Log(dictData.phrases);

        // foreach (var x in dictData.phrases)
        // {
        //     Debug.Log(x.id + " " + x.translations[0]);
        // }

    }

    void Test2()
    {
        SerializableJumper[] jumpers = new SerializableJumper[] { new SerializableJumper(0, "Kamil", "Stoch", "male", new DateTime(1987, 5, 25), "pl"),
        new SerializableJumper(1, "Dawid", "Kubacki", "male", new DateTime(1990, 3, 12), "pl"),
        new SerializableJumper(2, "Markus", "Eisenbichler", "male", new DateTime(1991, 4, 3), "de") };
        JumpersSerialization.Jump sampleJump = new JumpersSerialization.Jump(120, 18, 18, 18, 18, 18, 15, 89.6f, 1.2f, 1, 50);
        DateTime dt = DateTime.Parse("19.01.2018 17:52:00", new System.Globalization.CultureInfo("pl-PL"));
        Debug.Log("19.01.2018 17:52:00" + " " + dt);
        Result[] results = new Result[3];
        results[0] = new Result();
        results[1] = new Result();
        results[2] = new Result();
        results[0].competitor = jumpers[0];
        results[1].competitor = jumpers[1];
        results[2].competitor = jumpers[2];
        results[0].total = results[1].total = results[2].total = 0;
        results[0].rank = results[1].rank = results[2].rank = 1;
        results[0].jumps = results[1].jumps = results[2].jumps = new List<JumpersSerialization.Jump> { sampleJump, sampleJump };
        Debug.Log(JsonUtility.ToJson(results[0]));
        Debug.Log(JsonUtility.ToJson(results[1]));
        Debug.Log(JsonUtility.ToJson(results[2]));
        JumpersSerialization.Competition comp = new JumpersSerialization.Competition();
        comp.results = new List<Result>() { results[0], results[1], results[2] };
        comp.roundsInfo = new List<RoundInfo>() { new RoundInfo(new DateTime(2018, 1, 19, 16, 0, 0), new DateTime(2018, 1, 19, 16, 53, 0), 17),
        new RoundInfo(new DateTime(2018, 1, 19, 17, 12, 0), new DateTime(2018, 1, 19, 17, 52, 0), 25),
        new RoundInfo(new DateTime(2018, 1, 20, 16, 30, 0), new DateTime(2018, 1, 20, 17, 06, 0), 23)};
        string dataAsJson = JsonUtility.ToJson(comp);
        Debug.Log(dataAsJson);
    }

    void Start()
    {
        Test1();
        Test2();
    }
    void Update()
    {

    }
}
