using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping
{
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
}