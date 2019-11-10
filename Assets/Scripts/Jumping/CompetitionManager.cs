using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

using CompetitionClasses;

public class CompetitionManager : MonoBehaviour
{
    public ManagerScript managerScript;
    public JumpUIManager jumpUIManager;
    public JudgesController judges;

    public Calendar calendar;
    public CalendarResults calendarResults;
    public CompetitionClasses.Event currentEvent;
    public CompetitionClasses.HillInfo hillInfo;
    const string savesPath = "competition_saves.json";
    public List<int> competitorsList;
    public List<int> startList;
    public bool showingResults;
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
        judges.HillInit(calendarResults.hillProfiles[calendarResults.calendar.events[calendarResults.eventIt].hillId]);
        competitorsList = new List<int>();
        calendar = calendarResults.calendar;

        calendarResults.CurrentEventResults = new EventResults();
        currentEvent = calendarResults.calendar.events[calendarResults.eventIt];
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
                for (int i = 0; i < qualRankClassification.totalSortedResults.Count; i++)
                {
                    tempList.Add(Tuple.Create(qualRankClassification.totalSortedResults[i].Item1, qualRankClassification.totalSortedResults[i].Item2));
                }
                break;
        }

        switch (currentEvent.inLimitType)
        {
            case LimitType.None:
                for (int i = 0; i < tempList.Count; i++)
                {
                    competitorsList.Add(tempList[i].Item2);
                }
                break;
            case LimitType.Normal:
                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i].Item2 < tempList[currentEvent.inLimit - 1].Item2) { break; }
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

        foreach (var item in competitorsList)
        {
            Debug.Log(item);
        }

        calendarResults.CurrentEventResults.roundResults = new List<float>[competitorsList.Count];
        calendarResults.CurrentEventResults.totalResults = new float[competitorsList.Count];
        calendarResults.CurrentEventResults.rank = new int[competitorsList.Count];
        calendarResults.CurrentEventResults.lastRank = new int[competitorsList.Count];
        calendarResults.CurrentEventResults.competitorsList = competitorsList;

        for (int i = 0; i < competitorsList.Count; i++)
        {
            calendarResults.CurrentEventResults.roundResults[i] = new List<float>();
        }

        List<Tuple<float, int>> ordArr = new List<Tuple<float, int>>();
        for (int i = 0; i < competitorsList.Count; i++)
        {
            float key = i;
            if (currentEvent.ordRankType == CompetitionClasses.RankType.Event)
            {
                key = calendarResults.eventResults[currentEvent.ordRankId].totalResults[competitorsList[i]];
            }
            else if (currentEvent.ordRankType == CompetitionClasses.RankType.Classification)
            {
                key = calendarResults.classificationResults[currentEvent.ordRankId].totalResults[competitorsList[i]];
            }

            ordArr.Add(Tuple.Create(key, i));
        }

        ordArr.Sort();
        startList = new List<int>();
        foreach (var item in ordArr)
        {
            startList.Add(item.Item2);
            Debug.Log(calendarResults.calendar.competitors[competitorsList[item.Item2]].lastName);
        }
        calendarResults.roundIt = 0;
        calendarResults.jumpIt = -1;
        NextJump();
    }

    public void RoundInit()
    {
        startList = new List<int>();
        for (int i = calendarResults.CurrentEventResults.finalResults.Count - 1; i >= 0; i--)
        {
            startList.Add(calendarResults.CurrentEventResults.finalResults[i].Item2);
        }

        int it = 0;
        foreach (var item in startList)
        {
            Debug.Log(++it + " " + calendarResults.calendar.competitors[competitorsList[item]].lastName);
        }

        calendarResults.CurrentEventResults.finalResults.Clear();
        calendarResults.jumpIt = -1;
        NextJump();
    }

    public bool NextJump()
    {
        calendarResults.jumpIt++;
        // Debug.Log(calendarResults.jumpIt + " " + startList.Count);
        if (calendarResults.jumpIt == startList.Count)
        {
            showingResults = true;
            Debug.Log("SHOW RESULTS");
            jumpUIManager.ShowResults(calendarResults);
            return false;
        }
        if (calendarResults.jumpIt >= startList.Count)
        {
            calendarResults.jumpIt = 0;
            Debug.Log("LAST JUMP OF THE ROUND");
            return NextRound();
        }
        Debug.Log(calendarResults.jumpIt);
        Debug.Log(startList[calendarResults.jumpIt]);
        Debug.Log(competitorsList[startList[calendarResults.jumpIt]]);

        CompetitionClasses.Competitor comp = calendarResults.calendar.competitors[competitorsList[startList[calendarResults.jumpIt]]];
        if (calendarResults.roundIt == 0)
        {
            jumpUIManager.ShowJumperInfo(comp.firstName, comp.lastName, comp.countryCode);
        }
        else
        {
            jumpUIManager.ShowJumperInfo(comp.firstName, comp.lastName, comp.countryCode, true, calendarResults.CurrentEventResults.totalResults[startList[calendarResults.jumpIt]], calendarResults.CurrentEventResults.lastRank[startList[calendarResults.jumpIt]]);
        }

        judges.NewJump();
        return true;
    }

    public bool NextRound()
    {
        RecalculateFinalResults();
        calendarResults.roundIt++;
        jumpUIManager.HideResults();
        showingResults = false;
        if (calendarResults.roundIt >= currentEvent.roundInfos.Count)
        {
            Debug.Log("LAST ROUND OF THE EVENT");
            calendarResults.roundIt = 0;
            return NextEvent();
        }

        RoundInit();
        return true;
    }

    public bool NextEvent()
    {
        calendarResults.eventIt++;
        if (calendarResults.eventIt >= calendarResults.calendar.events.Count)
        {
            ShowClassificationsResults();
            return false;
        }

        CompetitionInit();
        return true;
    }

    public void RecalculateFinalResults()
    {
        calendarResults.CurrentEventResults.lastRank = calendarResults.CurrentEventResults.rank;

        switch (currentEvent.roundInfos[calendarResults.roundIt].outLimitType)
        {
            case LimitType.Normal:

                float minPoints = calendarResults.CurrentEventResults.finalResults[Mathf.Min(calendarResults.CurrentEventResults.finalResults.Count - 1, currentEvent.roundInfos[calendarResults.roundIt].outLimit - 1)].Item1;
                for (int i = calendarResults.CurrentEventResults.finalResults.Count - 1; i >= 0; i--)
                {
                    if (calendarResults.CurrentEventResults.finalResults[i].Item1 < minPoints)
                    {
                        calendarResults.CurrentEventResults.finalResults.RemoveAt(i);
                    }
                }
                break;
            case LimitType.Exact:
                int index = Mathf.Min(currentEvent.roundInfos[calendarResults.roundIt - 1].outLimit, calendarResults.CurrentEventResults.finalResults.Count - 1);
                int count = calendarResults.CurrentEventResults.finalResults.Count - index;
                calendarResults.CurrentEventResults.finalResults.RemoveRange(index, count);
                break;
        }

        foreach (var item in calendarResults.CurrentEventResults.finalResults)
        {
            Debug.Log(item.Item1 + " " + item.Item2 + " " + calendarResults.calendar.competitors[competitorsList[item.Item2]].lastName);
        }
    }

    public void ClassificationUpdate()
    {
        int[] individualPlacePoints = { 100, 80, 60, 50, 45, 40, 36, 32, 29, 26, 24, 22, 20, 18, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        int[] teamPlacePoints = { 400, 350, 300, 250, 200, 150, 100, 50 };
        foreach (var it in calendarResults.calendar.events[calendarResults.eventIt].classifications)
        {
            switch (calendarResults.calendar.classifications[it].classificationType)
            {
                case ClassificationType.IndividualPlace:
                    foreach (var jt in calendarResults.CurrentEventResults.finalResults)
                    {
                        float pts = 0;
                        int rnk = calendarResults.CurrentEventResults.rank[jt.Item2];
                        if (0 < rnk && rnk < 30) { pts = individualPlacePoints[rnk]; }
                        calendarResults.classificationResults[it].AddResult(competitorsList[jt.Item2], pts);
                    }
                    break;
                case ClassificationType.IndividualPoints:
                    for (int i = 0; i < competitorsList.Count; i++)
                    {
                        float pts = calendarResults.CurrentEventResults.totalResults[i];
                        calendarResults.classificationResults[it].AddResult(competitorsList[i], pts);
                    }
                    break;
                case ClassificationType.TeamPlace:
                    break;
                case ClassificationType.TeamPoints:
                    break;
            }
        }
    }

    public void ShowClassificationsResults()
    {
        for (int i = 0; i < calendarResults.classificationResults.Length; i++)
        {
            Debug.Log(calendarResults.calendar.classifications[i].name);
            for (int j = 0; j < calendarResults.classificationResults[i].totalResults.Length; j++)
            {
                Debug.Log(calendarResults.calendar.competitors[j].lastName + " " + calendarResults.classificationResults[i].totalResults[j]);
            }
        }
    }

    public float GetResult()
    {
        return 0;
    }

    public Tuple<int, float> AddJump(CompetitionClasses.Jump jp)
    {
        jp.distancePoints = hillInfo.DistancePoints(jp.distance);
        jp.totalPoints = Mathf.Max(0f, jp.judgesTotalPoints + jp.distancePoints);
        return Tuple.Create(calendarResults.CurrentEventResults.AddResult(startList[calendarResults.jumpIt], jp.totalPoints), calendarResults.CurrentEventResults.totalResults[(startList[calendarResults.jumpIt])]);
    }

    void Start()
    {
        LoadData();
        // calendarResults = new CalendarResults();
        // if (File.Exists(Path.Combine(Application.streamingAssetsPath, "data.json")))
        // {
        //     string dataAsJson = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "data.json"));
        //     calendarResults.hillProfiles = JsonConvert.DeserializeObject<HillProfile.AllData>(dataAsJson).profileData;
        // }

        // calendarResults.calendar = new Calendar();
        // calendarResults.calendar.competitors.Add(new Competitor("Kubacki", "Dawid", "POL", Gender.Male, 1990, 1, 1));
        // calendarResults.calendar.competitors.Add(new Competitor("Johansson", "Robert", "NOR", Gender.Male, 1990, 1, 1));
        // calendarResults.calendar.competitors.Add(new Competitor("Eisenbichler", "Markus", "GER", Gender.Male, 1990, 1, 1));
        // calendarResults.calendar.competitors.Add(new Competitor("Stoch", "Kamil", "POL", Gender.Male, 1987, 5, 25));
        // calendarResults.calendar.competitors.Add(new Competitor("Kraft", "Stefan", "AUT", Gender.Male, 1993, 1, 1));
        // calendarResults.calendar.competitors.Add(new Competitor("Kobayashi", "Ryoyu", "JPN", Gender.Male, 1990, 1, 1));
        // calendarResults.calendar.classifications.Add(new Classification("World Cup", ClassificationType.IndividualPlace));
        // calendarResults.calendar.events.Add(new CompetitionClasses.Event("Wisla Ind", 0, CompetitionClasses.EventType.Individual, new List<RoundInfo>(), new List<int>(), RankType.None, -1, RankType.None, -1));
        // calendarResults.calendar.events[0].roundInfos.Add(new RoundInfo(RoundType.Normal, LimitType.Normal, 3));
        // calendarResults.calendar.events[0].roundInfos.Add(new RoundInfo(RoundType.Normal, LimitType.None, -1));
        // calendarResults.eventResults = new EventResults[calendarResults.calendar.events.Count];
        // calendarResults.classificationResults = new ClassificationResults[calendarResults.calendar.classifications.Count];
        calendarResults.eventIt = 0;
        CompetitionInit();
        foreach (var item in startList)
        {
            Debug.Log(calendar.competitors[competitorsList[item]].lastName.ToUpper() + " " + calendar.competitors[competitorsList[item]].firstName);
        }
        // SaveData();
    }
}
