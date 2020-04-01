using System.Collections.Generic;
using Competition;
using UnityEngine;
using UnityEngine.U2D;

public class DatabaseManager : MonoBehaviour
{
    public List<RuntimeData> objects;
    public bool loadRoundInfoPresets;
    public DatabaseObject<List<RoundInfoPreset>> dbRoundInfoPresets;

    public bool loadCalendars;
    public DatabaseObject<List<Competition.Calendar>> dbCalendars;

    public bool loadCompetitors;
    public DatabaseObject<List<Competition.Competitor>> dbCompetitors;

    public bool loadHills;
    public DatabaseObject<HillProfile.AllData> dbHills;

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
            this.dbSaveData = SavesSystem.Load();
            if (this.dbSaveData == null)
            {
                this.dbSaveData = new SaveData(-1, new List<GameSave>());
            }
        }
    }

    public void Save()
    {
        if (loadCalendars) { this.dbCalendars.SaveData(); }
        if (loadCompetitors) { this.dbCompetitors.SaveData(); }
        if (loadHills) { this.dbHills.SaveData(); }
        if (loadRoundInfoPresets) { dbRoundInfoPresets.SaveData(); }
        if (loadSavesData) { SavesSystem.Save(this.dbSaveData); }
    }

}
