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

    public void SetSpeed(float val)
    {
        speedText.text = (int)val + "." + (int)(val * 10 % 100) + " km/h";
        Debug.Log(val);
    }

    public void SetDistance(float val)
    {
        distText.text = (int)val + "." + (int)(val * 10 % 10) + " m";
    }

    public void SetPoints(float[] points, int lo, int hi, float total, int rank)
    {
        pointsPanel.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            stylePtsText[i].text = "" + points[i];
        }
        stylePtsText[lo].fontStyle = TMPro.FontStyles.Strikethrough;
        stylePtsText[hi].fontStyle = TMPro.FontStyles.Strikethrough;
        totalPtsText.text = "" + total + "\t\t" + rank;
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


    private List<GameObject> resultsList;
    public GameObject resultPrefab;
    public GameObject resultsObject;
    public TMPro.TMP_Text resultsInfoText;
    public GameObject contentObject;

    private Dictionary<string, int> countriesDict;
    public GameObject jumperInfo;
    public void ShowJumperInfo(string name, string surname, string country, bool showLastRound = false, float lastPoints = 0, int lastRank = 0)
    {
        jumperInfo.SetActive(true);
        jumperInfo.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = name + " " + surname;
        jumperInfo.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = country;
        string lastRound = (showLastRound ? lastPoints + "\t\t" + lastRank : "");
        jumperInfo.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = lastRound;
        jumperInfo.GetComponentsInChildren<Image>()[1].sprite = flagsSprite.GetSprite("flags_responsive_uncompressed_" + countriesDict[country].ToString());
    }

    public void SetHillNameText(string s = "")
    {
        hillNameText.text = s;
    }

    public void ShowResults(CompetitionClasses.CalendarResults calendarResults)
    {
        resultsList = new List<GameObject>();
        int hillId = calendarResults.calendar.events[calendarResults.eventIt].hillId;
        resultsInfoText.text = calendarResults.hillProfiles[hillId].name + " " + calendarResults.calendar.events[calendarResults.eventIt].eventType.ToString() + "\n" + "Round #" + (calendarResults.roundIt + 1);
        resultsObject.SetActive(true);
        jumperInfo.SetActive(false);

        foreach (var x in calendarResults.CurrentEventResults.finalResults)
        {
            int i = calendarResults.CurrentEventResults.competitorsList[x.Item2];
            GameObject tmp = Instantiate(resultPrefab);
            tmp.transform.SetParent(contentObject.transform);
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.calendar.competitors[i].lastName.ToUpper() + " " + calendarResults.calendar.competitors[i].firstName;
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = calendarResults.calendar.competitors[i].countryCode;
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = x.Item1.ToString();
            resultsList.Add(tmp);
        }
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
        jumperInfo.SetActive(true);
        resultsObject.SetActive(false);
    }
    void Awake()
    {
        countriesDict = new Dictionary<string, int>();

        string filePath = Path.Combine(Application.streamingAssetsPath, countryDataFileName);
        CompetitionClasses.CountryData countryData = new CompetitionClasses.CountryData();
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            countryData = JsonConvert.DeserializeObject<CompetitionClasses.CountryData>(dataAsJson);
            for (int i = 0; i < countryData.spritesList.Count; i++)
            {
                countriesDict.Add(countryData.spritesList[i], i);
            }

            foreach (var c in countryData.countryList)
            {
                countriesDict.Add(c.ioc, countriesDict[c.alpha2]);
            }
        }
    }
}
