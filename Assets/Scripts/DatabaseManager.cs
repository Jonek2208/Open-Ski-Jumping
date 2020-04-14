using System.Collections.Generic;
using Competition.Persistent;
using Data;
using Hills;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public List<RuntimeData> objects;
    public bool loadRoundInfoPresets;
    public DatabaseObject<List<RoundInfoPreset>> dbRoundInfoPresets;

    public bool loadCalendars;
    public DatabaseObject<List<Calendar>> dbCalendars;

    public bool loadCompetitors;
    public DatabaseObject<List<Competitor>> dbCompetitors;

    public bool loadHills;
    public DatabaseObject<AllData> dbHills;

    public bool loadSavesData;
    public SaveData dbSaveData;


    private void Awake()
    {
        foreach (var item in objects) { item.LoadData(); }
        if (loadCalendars) { dbCalendars.LoadData(); }
        if (loadCompetitors) { dbCompetitors.LoadData(); }
        if (loadHills) { dbHills.LoadData(); }
        if (loadRoundInfoPresets) { dbRoundInfoPresets.LoadData(); }

        if (loadSavesData)
        {
            dbSaveData = SavesSystem.Load();
            if (dbSaveData == null)
            {
                dbSaveData = new SaveData(-1, new List<GameSave>());
            }
        }
    }

    public void Save()
    {
        if (loadCalendars) { dbCalendars.SaveData(); }
        if (loadCompetitors) { dbCompetitors.SaveData(); }
        if (loadHills) { dbHills.SaveData(); }
        if (loadRoundInfoPresets) { dbRoundInfoPresets.SaveData(); }
        if (loadSavesData) { SavesSystem.Save(dbSaveData); }
    }

}
