using System;
using System.Collections.Generic;

[Serializable]
public class GameSave
{
    public string name;
    public Competition.Calendar calendar;
    public Competition.ResultsDatabase resultsContainer;
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
    public List<Competition.RoundInfo> roundInfos;
    public RoundInfoPreset(string name, List<Competition.RoundInfo> roundInfos)
    {
        this.name = name;
        this.roundInfos = roundInfos;
    }
}

