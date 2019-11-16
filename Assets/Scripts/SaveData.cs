using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int currentSaveId;
    public List<CompCal.CalendarResults> savesList;
    public SaveData(int currentSaveId, List<CompCal.CalendarResults> savesList)
    {
        this.currentSaveId = currentSaveId;
        this.savesList = savesList;
    }
}

[System.Serializable]
public class RoundInfoPreset
{
    public string name;
    public List<CompCal.RoundInfo> roundInfos;
    public RoundInfoPreset(string name, List<CompCal.RoundInfo> roundInfos)
    {
        this.name = name;
        this.roundInfos = roundInfos;
    }
}

