using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class JumpUIManager : MonoBehaviour
{
    const string countryDataFileName = "country_data.json";
    public JudgesController judgesController;
    public Slider gateSlider;
    public TMPro.TMP_Text gateText;
    public Slider windSlider;
    public TMPro.TMP_Text windText;
    public TMPro.TMP_Text distText;
    public TMPro.TMP_Text speedText;

    public TMPro.TMP_Text[] stylePtsText;

    public TMPro.TMP_Text totalPtsText;
    public GameObject pointsPanel;

    public TMPro.TMP_Text hillNameText;
    public SpriteAtlas flagsSprite;

    private List<GameObject> resultsList;
    public GameObject resultsWrapperPrefab;
    public GameObject resultPrefab;
    public GameObject resultMetersPrefab;
    public GameObject resultPointsPrefab;
    public GameObject scrollViewObject;
    public GameObject resultsObject;
    public GameObject resultsDropdown;
    public TMPro.TMP_Text resultsInfoText;
    public GameObject contentObject;
    private Dictionary<string, int> countriesDict;
    public GameObject jumperResult;
    public GameObject jumperResultBlue;
    public GameObject jumperResultRed;
    public RectTransform jumperResultTransformTop;
    public RectTransform jumperResultTransformBottom;
    private GameObject currentJumperInfo;


    public void SetSpeed(float val)
    {
        speedText.text = val.ToString("#0.00") + " km/h";
        Debug.Log(val);
    }

    public void SetDistance(decimal val)
    {
        distText.text = (int)val + "." + (int)(val * 10 % 10) + " m";
    }

    public void SetPoints(decimal[] points, int lo, int hi, decimal total, int rank)
    {
        pointsPanel.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            stylePtsText[i].text = "" + points[i].ToString("#.0");
        }
        stylePtsText[lo].fontStyle = TMPro.FontStyles.Strikethrough;
        stylePtsText[hi].fontStyle = TMPro.FontStyles.Strikethrough;
        currentJumperInfo.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = total.ToString("#0.0") + "\t\t" + rank;
    }

    public void UIReset()
    {
        speedText.text = "";
        distText.text = "";
        pointsPanel.SetActive(false);

        for (int i = 0; i < 5; i++)
        {
            stylePtsText[i].fontStyle = TMPro.FontStyles.Normal;
        }
    }

    public void DisableJumperResults()
    {
        jumperResult.SetActive(false);
        jumperResultBlue.SetActive(false);
        jumperResultRed.SetActive(false);
    }

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

    public void ShowJumperInfo(GameObject gameObject, Calendar.Competitor competitor, int bib, string resultsString)
    {
        gameObject.SetActive(true);
        gameObject.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = bib + "\t\t" + competitor.firstName + " " + competitor.lastName.ToUpper(); ;
        gameObject.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = competitor.countryCode;
        gameObject.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = resultsString;
        string flagSprite = countriesDict.ContainsKey(competitor.countryCode) ? countriesDict[competitor.countryCode].ToString() : "0";
        gameObject.GetComponentsInChildren<Image>()[1].sprite = flagsSprite.GetSprite("flags_responsive_uncompressed_" + flagSprite);
    }

    public void ShowJumperInfoNormal(Calendar.Competitor competitor, int bib)
    {
        DisableJumperResults();
        ShowJumperInfo(jumperResult, competitor, bib, "");
        currentJumperInfo = jumperResult;
    }

    public void ShowJumperInfoNormal(Calendar.Competitor competitor, int bib, decimal lastPoints, int lastRank)
    {
        DisableJumperResults();
        ShowJumperInfo(jumperResult, competitor, bib, lastPoints.ToString("#0.0") + "\t\t" + lastRank);
        currentJumperInfo = jumperResult;
    }

    public void ShowJumperInfoKO(Calendar.Competitor comp1, int bib1)
    {
        DisableJumperResults();
        ShowJumperInfo(jumperResultRed, comp1, bib1, "");
        jumperResultRed.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition;
        currentJumperInfo = jumperResultRed;
    }
    public void ShowJumperInfoKO(Calendar.Competitor comp1, int bib1, Calendar.Competitor comp2, int bib2)
    {
        DisableJumperResults();
        ShowJumperInfo(jumperResultBlue, comp1, bib1, "");
        jumperResultBlue.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 50);
        ShowJumperInfo(jumperResultRed, comp2, bib2, "");
        jumperResultRed.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition;
        currentJumperInfo = jumperResultBlue;
    }

    public void ShowJumperInfoKO(Calendar.Competitor comp1, int bib1, Calendar.Competitor comp2, int bib2, float points1)
    {
        DisableJumperResults();
        ShowJumperInfo(jumperResultBlue, comp1, bib1, points1.ToString("#0.0"));
        jumperResultBlue.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition;
        ShowJumperInfo(jumperResultRed, comp2, bib2, "");
        jumperResultRed.GetComponent<RectTransform>().anchoredPosition = jumperResult.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 50);
        currentJumperInfo = jumperResultRed;
    }


    public void SetHillNameText(string s = "")
    {
        hillNameText.text = s;
    }

    public void ShowEventResults(Calendar.CalendarResults calendarResults)
    {
        resultsList = new List<GameObject>();
        int hillId = calendarResults.calendar.events[calendarResults.eventIt].hillId;
        resultsInfoText.text = calendarResults.hillProfiles[hillId].name + " " + calendarResults.calendar.events[calendarResults.eventIt].eventType.ToString() + "\n" + "Round #" + (calendarResults.roundIt + 1);
        resultsObject.SetActive(true);
        jumperResult.SetActive(false);

        float width = resultPrefab.GetComponent<RectTransform>().rect.width + Mathf.Min(calendarResults.roundIt + 1, 4) * resultMetersPrefab.GetComponent<RectTransform>().rect.width + resultPointsPrefab.GetComponent<RectTransform>().rect.width;
        scrollViewObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, scrollViewObject.GetComponent<RectTransform>().sizeDelta.y);

        foreach (var x in calendarResults.CurrentEventResults.finalResults.Values)
        {
            int i = calendarResults.CurrentEventResults.competitorsList[x];
            GameObject wrapper = Instantiate(resultsWrapperPrefab);
            wrapper.GetComponent<RectTransform>().sizeDelta = new Vector2(width, wrapper.GetComponent<RectTransform>().sizeDelta.y);
            GameObject tmp = Instantiate(resultPrefab);
            wrapper.transform.SetParent(contentObject.transform);
            tmp.transform.SetParent(wrapper.transform);
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.calendar.competitors[i].firstName + " " + calendarResults.calendar.competitors[i].lastName.ToUpper();
            tmp.GetComponentsInChildren<Image>()[1].sprite = flagsSprite.GetSprite("flags_responsive_uncompressed_" + countriesDict[calendarResults.calendar.competitors[i].countryCode].ToString());
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = calendarResults.calendar.competitors[i].countryCode;
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = calendarResults.CurrentEventResults.rank[x].ToString();
            // tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = x.Item1.ToString("#0.0");

            for (int j = 0; j <= calendarResults.roundIt; j++)
            {
                tmp = Instantiate(resultMetersPrefab);
                tmp.transform.SetParent(wrapper.transform);
                tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.CurrentEventResults.roundResults[x][j].distance.ToString("#0.0");
            }

            tmp = Instantiate(resultPointsPrefab);
            tmp.transform.SetParent(wrapper.transform);
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.CurrentEventResults.totalResults[x].ToString("#0.0");
            resultsList.Add(wrapper);
        }
    }

    // public void ShowClassificationsResults(CompetitionClasses.CalendarResults calendarResults)
    // {
    //     resultsList = new List<GameObject>();
    //     int hillId = calendarResults.calendar.events[calendarResults.eventIt].hillId;
    //     resultsInfoText.text = calendarResults.hillProfiles[hillId].name + " " + calendarResults.calendar.events[calendarResults.eventIt].eventType.ToString() + "\n" + "Round #" + (calendarResults.roundIt + 1);
    //     resultsObject.SetActive(true);
    //     jumperInfo.SetActive(false);
    //     resultsDropdown.SetActive(true);

    //     float width = resultPrefab.GetComponent<RectTransform>().rect.width + Mathf.Max(calendarResults.roundIt + 1, 4) * resultMetersPrefab.GetComponent<RectTransform>().rect.width + resultPointsPrefab.GetComponent<RectTransform>().rect.width;
    //     scrollViewObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, scrollViewObject.GetComponent<RectTransform>().sizeDelta.y);

    //     foreach (var x in calendarResults.CurrentEventResults.finalResults)
    //     {
    //         int i = calendarResults.CurrentEventResults.competitorsList[x.Item2];
    //         GameObject wrapper = Instantiate(resultsWrapperPrefab);
    //         wrapper.GetComponent<RectTransform>().sizeDelta = new Vector2(width, wrapper.GetComponent<RectTransform>().sizeDelta.y);
    //         GameObject tmp = Instantiate(resultPrefab);
    //         wrapper.transform.SetParent(contentObject.transform);
    //         tmp.transform.SetParent(wrapper.transform);
    //         tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.calendar.competitors[i].firstName + " " + calendarResults.calendar.competitors[i].lastName.ToUpper();
    //         tmp.GetComponentsInChildren<Image>()[1].sprite = flagsSprite.GetSprite("flags_responsive_uncompressed_" + countriesDict[calendarResults.calendar.competitors[i].countryCode].ToString());
    //         tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = calendarResults.calendar.competitors[i].countryCode;
    //         tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = calendarResults.CurrentEventResults.rank[x.Item2].ToString();
    //         // tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = x.Item1.ToString("#0.0");

    //         for (int j = 0; j <= calendarResults.roundIt; j++)
    //         {
    //             tmp = Instantiate(resultMetersPrefab);
    //             tmp.transform.SetParent(wrapper.transform);
    //             tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.CurrentEventResults.roundResults[x.Item2][j].distance.ToString("#0.0");
    //         }

    //         tmp = Instantiate(resultPointsPrefab);
    //         tmp.transform.SetParent(wrapper.transform);
    //         tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = x.Item1.ToString("#0.0");
    //         resultsList.Add(wrapper);
    //     }
    // }

    // public void OnResultsDropdown()
    // {

    // }

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
        jumperResult.SetActive(true);
        resultsObject.SetActive(false);
        resultsDropdown.SetActive(false);

    }
    void Awake()
    {
        countriesDict = new Dictionary<string, int>();

        string filePath = Path.Combine(Application.streamingAssetsPath, countryDataFileName);
        Calendar.CountryData countryData = new Calendar.CountryData();
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            countryData = JsonConvert.DeserializeObject<Calendar.CountryData>(dataAsJson);
            for (int i = 0; i < countryData.spritesList.Count; i++)
            {
                countriesDict.Add(countryData.spritesList[i], i);
            }

            foreach (var c in countryData.countryList)
            {
                if (countriesDict.ContainsKey(c.alpha2))
                {
                    countriesDict.Add(c.ioc, countriesDict[c.alpha2]);
                }
            }
        }
    }
}
