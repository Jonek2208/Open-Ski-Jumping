using System;
using System.Collections.Generic;
using Competition;
using Competition.Persistent;

[Serializable]
public class GameSave
{
    public string name;
    public Calendar calendar;
    public ResultsDatabase resultsContainer;
}

[Serializable]
public class SaveData
{
    public int currentSaveId;
    public List<GameSave> savesList;
    public SaveData(int currentSaveId, List<GameSave> savesList)
    {
        this.currentSaveId = currentSaveId;
        this.savesList = savesList;
    }
}

[Serializable]
public class RoundInfoPreset
{
    public string name;
    public List<RoundInfo> roundInfos;
    public RoundInfoPreset(string name, List<RoundInfo> roundInfos)
    {
        this.name = name;
        this.roundInfos = roundInfos;
    }
}

