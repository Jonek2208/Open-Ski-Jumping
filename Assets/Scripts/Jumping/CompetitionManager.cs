using System;
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
        List<Tuple<float, int>> tempList = new List<Tuple<float, int>>();

        switch (currentEvent.qualRankType)
        {
            case RankType.None:
                // Add all competitors
                competitorsList = new List<int>();
                for (int i = 0; i < calendar.competitors.Count; i++) { competitorsList.Add(i); }
                break;
            case RankType.Event:
                // Add competitors from some event output rank
                CompetitionClasses.EventResults qualRankEvent = calendarResults.eventResults[currentEvent.qualRankId];
                for (int i = 0; i < qualRankEvent.finalResults.Count; i++)
                {
                    tempList.Add(Tuple.Create(qualRankEvent.finalResults[i].Item1, qualRankEvent.competitorsList[qualRankEvent.finalResults[i].Item2]));
                }
                break;
            case RankType.Classification:
                // Add competitors from some event output rank
                CompetitionClasses.ClassificationResults qualRankClassification = calendarResults.classificationResults[currentEvent.qualRankId];
                for (int i = 0; i < qualRankClassification.totalResults.Count; i++)
                {
                    tempList.Add(Tuple.Create(qualRankClassification.totalResults[i].Item1, qualRankClassification.totalResults[i].Item2));
                }
                break;
        }

        switch (currentEvent.inLimitType)
        {
            case LimitType.Normal:
                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i].Item2 < tempList[currentEvent.inLimit].Item2) { break; }
                    competitorsList.Add(tempList[i].Item2);
                }

                break;
            case LimitType.Exact:
                for (int i = 0; i < currentEvent.inLimit; i++)
                {
                    competitorsList.Add(tempList[i].Item2);
                }
                break;
        }

        Tuple<float, int>[] ordArr = new Tuple<float, int>[calendar.competitors.Count];

    }

    public void AddJump()
    {

    }

    void Start()
    {
        LoadData();
        // CompetitionInit();
        calendarResults = new CalendarResults();
        calendarResults.calendar = new Calendar();
        calendarResults.eventResults = new List<EventResults>();
        calendarResults.classificationResults = new List<ClassificationResults>();
        calendarResults.calendar.classifications = new List<Classification>();
        calendarResults.calendar.classifications.Add(new Classification("World Cup", ClassificationType.IndividualPlace));
        calendarResults.calendar.events = new List<CompetitionClasses.Event>();
        calendarResults.calendar.events.Add(new CompetitionClasses.Event("Wisla Ind", 0, CompetitionClasses.EventType.Individual, new List<RoundInfo>(), new List<int>(), RankType.None, -1, RankType.None, -1));
        SaveData();
    }
}
