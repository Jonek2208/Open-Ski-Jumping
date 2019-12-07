using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class JumpUIManager : MonoBehaviour
{
    #region Properties
    public FlagsData flagsData;
    public JudgesController judgesController;
    public Slider gateSlider;
    public TMPro.TMP_Text gateText;
    public Slider windSlider;
    public TMPro.TMP_Text windText;
    public TMPro.TMP_Text distText;
    public TMPro.TMP_Text speedText;
    public GameObject pointsPanel;
    public TMPro.TMP_Text hillNameText;
    private List<GameObject> resultsList;
    public GameObject resultsWrapperPrefab;
    public GameObject resultPrefab;
    public GameObject resultMetersPrefab;
    public GameObject resultPointsPrefab;
    public GameObject scrollViewObject;
    public GameObject resultsObject;
    public TMPro.TMP_Text resultsInfoText;
    public GameObject contentObject;
    #endregion
    public void UIReset()
    {
        speedText.text = "";
        distText.text = "";
        pointsPanel.SetActive(false);
    }

    // public void DisableJumperResults()
    // {
    //     jumperResult.SetActive(false);
    //     jumperResultBlue.SetActive(false);
    //     jumperResultRed.SetActive(false);
    //     jumperResultTeamTop.SetActive(false);
    //     jumperResultTeamBottom.SetActive(false);
    // }

    public void GateSliderChange()
    {
        judgesController.SetGate((int)gateSlider.value);
        gateText.text = "Gate: " + (int)gateSlider.value;
    }

    public void SetGateSlider(int gate)
    {
        gateSlider.value = gate;
        gateText.text = "Gate: " + (int)gateSlider.value;
    }

    public void WindSliderChange()
    {
        judgesController.SetWind(windSlider.value);
        windText.text = "Wind: " + windSlider.value;
    }

    public void SetGateSliderRange(int gates)
    {
        gateSlider.minValue = 1;
        gateSlider.maxValue = gates;
        GateSliderChange();
    }

    public void SetSpeed(float val)
    {
        speedText.text = val.ToString("#0.00") + " km/h";
        Debug.Log(val);
    }

    public void SetDistance(decimal val)
    {
        distText.text = (int)val + "." + (int)(val * 10 % 10) + " m";
    }

    // public void SetPoints(CompCal.JumpResult jmp, int rank, decimal total)
    // {
    //     SetJudgesPoints(jmp);
    //     currentJumperInfo.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = total.ToString("#0.0") + "\t\t" + rank;
    // }

    // public void SetPoints(CompCal.JumpResult jmp, int rank1, decimal total1, int rank2, decimal total2)
    // {
    //     SetJudgesPoints(jmp);
    //     jumperResultRed.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = total1.ToString("#0.0") + "\t\t" + rank1;
    //     jumperResultBlue.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = total2.ToString("#0.0") + "\t\t" + rank2;
    // }

    // public void SetPoints(CompCal.JumpResult jmp, int rank, decimal totalTeam, decimal totalJumper)
    // {
    //     SetJudgesPoints(jmp);
    //     jumperResultTeamTop.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = totalTeam.ToString("#0.0") + "\t\t" + rank;
    //     jumperResultTeamBottom.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = totalJumper.ToString("#0.0");
    // }

    // public string CompetitorData(CompCal.Competitor competitor, int bib) => bib + "\t\t" + competitor.firstName + " " + competitor.lastName.ToUpper();
    // public string TeamData(CompCal.Team team, int bib) => bib + "\t\t" + team.countryCode.ToUpper();

    // public void ShowJumperInfo(GameObject gameObject, string infoString, string countryCode, string resultsString)
    // {
    //     gameObject.SetActive(true);
    //     gameObject.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = infoString;
    //     gameObject.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = countryCode;
    //     gameObject.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = resultsString;
    //     gameObject.GetComponentsInChildren<Image>()[1].sprite = flagsData.GetFlag(countryCode);
    // }

    // public void ShowJumperInfoNormal(CompCal.Competitor competitor, int bib)
    // {
    //     DisableJumperResults();
    //     ShowJumperInfo(jumperResult, CompetitorData(competitor, bib), competitor.countryCode, "");
    //     currentJumperInfo = jumperResult;
    // }

    // public void ShowJumperInfoNormal(CompCal.Competitor competitor, int bib, decimal lastPoints, int lastRank)
    // {
    //     DisableJumperResults();
    //     ShowJumperInfo(jumperResult, CompetitorData(competitor, bib), competitor.countryCode, lastPoints.ToString("#0.0") + "\t\t" + lastRank);
    //     currentJumperInfo = jumperResult;
    // }

    // public void ShowJumperInfoKO(CompCal.Competitor comp1, int bib1)
    // {
    //     DisableJumperResults();
    //     ShowJumperInfo(jumperResultRed, CompetitorData(comp1, bib1), comp1.countryCode, "");
    //     jumperResultRed.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition;
    //     currentJumperInfo = jumperResultRed;
    // }

    // public void ShowJumperInfoKO(CompCal.Competitor comp1, int bib1, CompCal.Competitor comp2, int bib2)
    // {
    //     DisableJumperResults();
    //     ShowJumperInfo(jumperResultBlue, CompetitorData(comp1, bib1), comp1.countryCode, "");
    //     jumperResultBlue.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 50);
    //     ShowJumperInfo(jumperResultRed, CompetitorData(comp2, bib2), comp2.countryCode, "");
    //     jumperResultRed.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition;
    //     currentJumperInfo = jumperResultBlue;
    // }

    // public void ShowJumperInfoKO(CompCal.Competitor comp1, int bib1, CompCal.Competitor comp2, int bib2, decimal points1)
    // {
    //     DisableJumperResults();
    //     ShowJumperInfo(jumperResultBlue, CompetitorData(comp1, bib1), comp1.countryCode, points1.ToString("#0.0"));
    //     jumperResultBlue.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition;
    //     ShowJumperInfo(jumperResultRed, CompetitorData(comp2, bib2), comp2.countryCode, "");
    //     jumperResultRed.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 50);
    //     currentJumperInfo = jumperResultRed;
    // }

    // private void ShowJumperInfoTeamHelper(string txt1, string txt2)
    // {
    //     jumperResultTeamBottom.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = txt1;
    //     jumperResultTeamBottom.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = txt2;
    // }

    // public void ShowJumperInfoTeam(CompCal.Team team, CompCal.Competitor jumper, int bibTeam, int bibJumper)
    // {
    //     DisableJumperResults();
    //     ShowJumperInfo(jumperResultTeamTop, TeamData(team, bibTeam), team.countryCode, "");
    //     ShowJumperInfoTeamHelper(CompetitorData(jumper, bibJumper), "");
    // }

    // public void ShowJumperInfoTeam(CompCal.Team team, CompCal.Competitor jumper, int bibTeam, int bibJumper, int lastRank, decimal lastPointsTeam)
    // {
    //     DisableJumperResults();
    //     ShowJumperInfo(jumperResultTeamTop, TeamData(team, bibTeam), team.countryCode, lastPointsTeam.ToString("#0.0") + "\t\t" + lastRank);
    //     ShowJumperInfoTeamHelper(CompetitorData(jumper, bibJumper), "");
    // }

    // public void ShowJumperInfoTeam(CompCal.Team team, CompCal.Competitor jumper, int bibTeam, int bibJumper, int lastRank, decimal lastPointsTeam, decimal lastPointsJumper)
    // {
    //     DisableJumperResults();
    //     ShowJumperInfo(jumperResultTeamTop, TeamData(team, bibTeam), team.countryCode, lastPointsTeam.ToString("#0.0") + "\t\t" + lastRank);
    //     ShowJumperInfoTeamHelper(CompetitorData(jumper, bibJumper), lastPointsJumper.ToString("#0.0"));
    // }

    public void SetHillNameText(string s = "")
    {
        hillNameText.text = s;
    }

    public void ShowEventResults(CompCal.CalendarResults calendarResults)
    {
        resultsList = new List<GameObject>();
        int hillId = calendarResults.calendar.events[calendarResults.eventIt].hillId;
        resultsInfoText.text = calendarResults.hillProfiles[hillId].name + " " + calendarResults.calendar.events[calendarResults.eventIt].eventType.ToString() + "\n" + "Round #" + (calendarResults.roundIt + 1);
        resultsObject.SetActive(true);

        float width = resultPrefab.GetComponent<RectTransform>().rect.width + Mathf.Min(calendarResults.roundIt + 1, 4) * resultMetersPrefab.GetComponent<RectTransform>().rect.width + resultPointsPrefab.GetComponent<RectTransform>().rect.width;
        scrollViewObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, scrollViewObject.GetComponent<RectTransform>().sizeDelta.y);

        // foreach (var x in calendarResults.CurrentEventResults.finalResults.Values)
        // {
        //     int i = calendarResults.CurrentEventResults.participants[x];
        //     GameObject wrapper = Instantiate(resultsWrapperPrefab);
        //     wrapper.GetComponent<RectTransform>().sizeDelta = new Vector2(width, wrapper.GetComponent<RectTransform>().sizeDelta.y);
        //     GameObject tmp = Instantiate(resultPrefab);
        //     wrapper.transform.SetParent(contentObject.transform, false);
        //     tmp.transform.SetParent(wrapper.transform, false);
        //     tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.calendar.competitors[i].firstName + " " + calendarResults.calendar.competitors[i].lastName.ToUpper();
        //     tmp.GetComponentsInChildren<Image>()[1].sprite = flagsData.GetFlag(calendarResults.calendar.competitors[i].countryCode);
        //     tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = calendarResults.calendar.competitors[i].countryCode;
        //     tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = calendarResults.CurrentEventResults.rank[x].ToString();
        //     // tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = x.Item1.ToString("#0.0");

        //     for (int j = 0; j <= calendarResults.roundIt; j++)
        //     {
        //         tmp = Instantiate(resultMetersPrefab);
        //         tmp.transform.SetParent(wrapper.transform, false);
        //         tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.CurrentEventResults.roundResults[x][j].distance.ToString("#0.0");
        //     }

        //     tmp = Instantiate(resultPointsPrefab);
        //     tmp.transform.SetParent(wrapper.transform, false);
        //     tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.CurrentEventResults.totalResults[x].ToString("#0.0");
        //     resultsList.Add(wrapper);
        // }
    }

    public void HideResults()
    {
        if (resultsList != null)
        {
            foreach (var x in resultsList)
            {
                Destroy(x);
            }
        }

        resultsList = new List<GameObject>();
        resultsObject.SetActive(false);
    }
}
