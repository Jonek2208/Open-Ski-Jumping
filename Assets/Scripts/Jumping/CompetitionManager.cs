using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using CompetitionClasses;

public class CompetitionManager : MonoBehaviour
{
    public ManagerScript managerScript;
    public JumpUIManager jumpUIManager;
    public JudgesController judges;
    public Calendar calendar;
    public CalendarResults calendarResults;
    public CompetitionClasses.Event currentEvent;
    const string savesPath = "competition_saves.json";
    public List<int> competitorsList;
    public List<int> startList;
    private void LoadData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, savesPath);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            calendarResults = JsonConvert.DeserializeObject<CalendarResults>(dataAsJson);
        }
    }

    private void SaveData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, savesPath);
        string dataAsJson = JsonConvert.SerializeObject(calendarResults);
        File.WriteAllText(filePath, dataAsJson);
    }

    public void CompetitionInit()
    {
        calendar = calendarResults.calendar;
        calendarResults.eventResults[calendarResults.eventIt] = new EventResults();
        CompetitionClasses.Event currentEvent = calendarResults.calendar.events[calendarResults.eventIt];

        switch (currentEvent.qualRankType)
        {
            case RankType.None:
                competitorsList = new List<int>();
                for (int i = 0; i < calendar.competitors.Count; i++) { competitorsList.Add(i); }
                break;
            case RankType.Event:
                CompetitionClasses.EventResults qualRankEvent = calendarResults.eventResults[currentEvent.qualRankId];
                if (currentEvent.inLimitType == LimitType.None)
                {
                    for (int i = 0; i < qualRankEvent.finalResults.Count; i++)
                    {
                        competitorsList.Add(qualRankEvent.competitorsList[qualRankEvent.finalResults[i].Item1]);
                    }
                }
                else
                {
                    for (int i = 0; i < currentEvent.inLimit; i++)
                    {
                        competitorsList.Add(qualRankEvent.competitorsList[qualRankEvent.finalResults[i].Item1]);
                    }
                    if (currentEvent.inLimitType == LimitType.Normal)
                    {
                        for (int i = currentEvent.inLimit; i < qualRankEvent.competitorsList.Count; i++)
                        {
                            if (qualRankEvent.finalResults[i].Item2 >= qualRankEvent.finalResults[currentEvent.inLimit].Item2)
                            {
                                competitorsList.Add(qualRankEvent.competitorsList[qualRankEvent.finalResults[i].Item1]);
                            }
                            else
                            { break; }
                        }
                    }
                }
                break;
            case RankType.Classification:
                CompetitionClasses.ClassificationResults qualRankClassification = calendarResults.classificationResults[currentEvent.qualRankId];
                for (int i = 0; i < qualRankClassification.totalResults.Count; i++)
                {
                    competitorsList.Add(qualRankClassification.totalResults[i].Item1);
                }
                break;
        }

        switch (currentEvent.inLimitType)
        {
            case LimitType.None:
                break;
            case LimitType.Normal:
                break;
            case LimitType.Exact:
                break;
        }

    }
}
