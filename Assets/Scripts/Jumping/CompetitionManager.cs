using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

using Calendar;

public class CompetitionManager : MonoBehaviour
{
    public ManagerScript managerScript;
    public JumpUIManager jumpUIManager;
    public JudgesController judges;

    public CalendarResults calendarResults;

    const string savesPath = "competition_saves.json";
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
        judges.PrepareHill(calendarResults.hillProfiles[calendarResults.calendar.events[calendarResults.eventIt].hillId]);
        // Debug.Log("HillId: " + calendarResults.calendar.events[calendarResults.eventIt].hillId);
        // Debug.LogAssertion(calendarResults.hillProfiles[calendarResults.calendar.events[calendarResults.eventIt].hillId] != null);
        calendarResults.EventInit();
        // competitorsList = new List<int>();
        // calendar = calendarResults.calendar;

        // currentEvent = calendarResults.calendar.events[calendarResults.eventIt];
        // List<Tuple<float, int>> tempList = new List<Tuple<float, int>>();

        // switch (currentEvent.qualRankType)
        // {
        //     case RankType.None:
        //         // Add all competitors
        //         competitorsList = new List<int>();
        //         for (int i = 0; i < calendar.competitors.Count; i++) { competitorsList.Add(i); }
        //         break;
        //     case RankType.Event:
        //         // Add competitors from some event output rank
        //         CompetitionClasses.EventResults qualRankEvent = calendarResults.eventResults[currentEvent.qualRankId];
        //         for (int i = 0; i < qualRankEvent.finalResults.Count; i++)
        //         {
        //             tempList.Add(Tuple.Create(qualRankEvent.finalResults[i].Item1, qualRankEvent.competitorsList[qualRankEvent.finalResults[i].Item2]));
        //         }
        //         break;
        //     case RankType.Classification:
        //         // Add competitors from some event output rank
        //         CompetitionClasses.ClassificationResults qualRankClassification = calendarResults.classificationResults[currentEvent.qualRankId];
        //         for (int i = 0; i < qualRankClassification.totalSortedResults.Count; i++)
        //         {
        //             tempList.Add(Tuple.Create(qualRankClassification.totalSortedResults[i].Item1, qualRankClassification.totalSortedResults[i].Item2));
        //         }
        //         break;
        // }

        // switch (currentEvent.inLimitType)
        // {
        //     case LimitType.None:
        //         for (int i = 0; i < tempList.Count; i++)
        //         {
        //             competitorsList.Add(tempList[i].Item2);
        //         }
        //         break;
        //     case LimitType.Normal:
        //         for (int i = 0; i < tempList.Count; i++)
        //         {
        //             if (tempList[i].Item2 < tempList[currentEvent.inLimit - 1].Item2) { break; }
        //             competitorsList.Add(tempList[i].Item2);
        //         }
        //         break;
        //     case LimitType.Exact:
        //         for (int i = 0; i < currentEvent.inLimit; i++)
        //         {
        //             competitorsList.Add(tempList[i].Item2);
        //         }
        //         break;
        // }

        // foreach (var item in competitorsList)
        // {
        //     Debug.Log(item);
        // }

        // calendarResults.CurrentEventResults = new EventResults(competitorsList);

        // for (int i = 0; i < competitorsList.Count; i++)
        // {
        //     calendarResults.CurrentEventResults.roundResults[i] = new List<CompetitionClasses.Jump>();
        // }

        // List<Tuple<float, int>> ordArr = new List<Tuple<float, int>>();
        // for (int i = 0; i < competitorsList.Count; i++)
        // {
        //     float key = i;
        //     if (currentEvent.ordRankType == CompetitionClasses.RankType.Event)
        //     {
        //         key = calendarResults.eventResults[currentEvent.ordRankId].totalResults[competitorsList[i]];
        //     }
        //     else if (currentEvent.ordRankType == CompetitionClasses.RankType.Classification)
        //     {
        //         key = calendarResults.classificationResults[currentEvent.ordRankId].totalResults[competitorsList[i]];
        //     }

        //     ordArr.Add(Tuple.Create(key, i));
        // }

        // ordArr.Sort();
        // startList = new List<int>();
        // foreach (var item in ordArr)
        // {
        //     startList.Add(item.Item2);
        //     Debug.Log(calendarResults.calendar.competitors[competitorsList[item.Item2]].lastName);
        // }
        // calendarResults.roundIt = 0;
        // foreach (var it in calendarResults.CurrentEventResults.finalResults.Keys)
        // {
        //     Debug.Log(it.Item1 + " " + calendarResults.calendar.competitors[calendarResults.CurrentEventResults.competitorsList[it.Item3]].lastName);
        // }
        RoundInit();
        NextJump();
    }

    public void RoundInit()
    {
        calendarResults.RoundInit();
        foreach (var item in calendarResults.CurrentStartList)
        {
            Debug.Log(calendarResults.calendar.competitors[calendarResults.CurrentEventResults.competitorsList[item]].lastName);
        }
        calendarResults.jumpIt = -1;
    }

    public bool NextJump()
    {
        calendarResults.jumpIt++;
        if (calendarResults.jumpIt < calendarResults.CurrentStartList.Count)
        {
            Calendar.Competitor comp = calendarResults.CurrentCompetitor;
            int bib = calendarResults.CurrentEventResults.bibs[calendarResults.CurrentStartList[calendarResults.jumpIt]][calendarResults.roundIt];
            if (calendarResults.CurrentRoundInfo.roundType == RoundType.Normal)
            {
                if (calendarResults.roundIt == 0)
                {
                    jumpUIManager.ShowJumperInfoNormal(comp, bib);
                }
                else
                {
                    jumpUIManager.ShowJumperInfoNormal(comp, bib,
                    calendarResults.CurrentEventResults.totalResults[calendarResults.CurrentStartList[calendarResults.jumpIt]],
                    calendarResults.CurrentEventResults.lastRank[calendarResults.CurrentStartList[calendarResults.jumpIt]]);
                }
            }
            else if (calendarResults.CurrentRoundInfo.roundType == RoundType.KO)
            {
                if (calendarResults.jumpIt % 2 == 0 && calendarResults.jumpIt == calendarResults.CurrentStartList.Count - 1)
                {
                    jumpUIManager.ShowJumperInfoKO(comp, bib);
                }
                else
                {
                    int cnt = calendarResults.jumpIt - calendarResults.jumpIt % 2;
                    int id1 = calendarResults.CurrentStartList[cnt];
                    int id2 = calendarResults.CurrentStartList[cnt + 1];
                    Competitor comp1 = calendarResults.calendar.competitors[calendarResults.CurrentEventResults.competitorsList[id1]];
                    Competitor comp2 = calendarResults.calendar.competitors[calendarResults.CurrentEventResults.competitorsList[id2]];
                    int bib1 = calendarResults.CurrentEventResults.bibs[id1][calendarResults.roundIt];
                    int bib2 = calendarResults.CurrentEventResults.bibs[id2][calendarResults.roundIt];

                    if (calendarResults.jumpIt % 2 == 0)
                    {
                        jumpUIManager.ShowJumperInfoKO(comp1, bib1, comp2, bib2);
                    }
                    else
                    {
                        jumpUIManager.ShowJumperInfoKO(comp1, bib1, comp2, bib2, calendarResults.CurrentEventResults.totalResults[id1]);
                    }
                }
            }


            judges.NewJump();
        }
        else if (calendarResults.jumpIt == calendarResults.CurrentStartList.Count)
        {
            showingResults = true;
            jumpUIManager.ShowEventResults(calendarResults);
        }
        else if (calendarResults.jumpIt >= calendarResults.CurrentStartList.Count)
        {
            calendarResults.jumpIt = 0;
            return NextRound();
        }

        return true;
    }

    public bool NextRound()
    {
        calendarResults.RecalculateFinalResults();
        calendarResults.roundIt++;
        jumpUIManager.HideResults();
        showingResults = false;
        if (calendarResults.roundIt >= calendarResults.CurrentEvent.roundInfos.Count)
        {
            calendarResults.roundIt = 0;
            return NextEvent();
        }

        RoundInit();
        return true;
    }

    public bool NextEvent()
    {
        calendarResults.UpdateClassifications();
        calendarResults.eventIt++;
        if (calendarResults.eventIt >= calendarResults.calendar.events.Count)
        {
            ShowClassificationsResults();
            return false;
        }

        CompetitionInit();
        return true;
    }

    public void ShowClassificationsResults()
    {
        for (int i = 0; i < calendarResults.classificationResults.Length; i++)
        {
            Debug.Log(calendarResults.calendar.classifications[i].name);
            for (int j = 0; j < calendarResults.classificationResults[i].totalResults.Length; j++)
            {
                Debug.Log(calendarResults.calendar.competitors[j].lastName + " " + calendarResults.classificationResults[i].totalResults[j].ToString("#.0"));
            }
        }
    }

    void Start()
    {
        LoadData();
        calendarResults.eventIt = 0;
        CompetitionInit();
        // SaveData();
    }
}
