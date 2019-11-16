using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

using CompCal;

public class CompetitionManager : MonoBehaviour
{
    public DatabaseManager databaseManager;
    public ManagerScript managerScript;
    public JumpUIManager jumpUIManager;
    public JudgesController judges;

    public CalendarResults calendarResults;

    public bool showingResults;
    private void LoadData()
    {
        calendarResults = databaseManager.dbSaveData.savesList[databaseManager.dbSaveData.currentSaveId];
    }

    public void CompetitionInit()
    {
        judges.PrepareHill(calendarResults.hillProfiles[calendarResults.calendar.events[calendarResults.eventIt].hillId]);
        calendarResults.EventInit();

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
            CompCal.Competitor comp = calendarResults.CurrentCompetitor;
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

    public bool NextJumpTeam()
    {
        calendarResults.jumpIt++;
        if (calendarResults.jumpIt < calendarResults.CurrentStartList.Count)
        {
            CompCal.Competitor comp = calendarResults.CurrentCompetitor;
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
        LoadTournamentMenu();
        // if (calendarResults.eventIt >= calendarResults.calendar.events.Count)
        // {
        //     ShowClassificationsResults();
        //     return false;
        // }

        // CompetitionInit();
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

    public void LoadTournamentMenu()
    {
        databaseManager.Save();
        MainMenuController.LoadTournamentMenu();
    }

    public void LoadMainMenu()
    {
        MainMenuController.LoadMainMenu();
    }

    void Start()
    {
        LoadData();
        CompetitionInit();
        // SaveData();
    }
}
