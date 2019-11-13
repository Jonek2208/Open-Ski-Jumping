using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

using Calendar;

public class CalendarCreatorScript : MonoBehaviour
{
    const string competitorsFileName = "competitors.json";
    const string countryDataFileName = "country_data.json";
    const string savesPath = "competition_saves.json";
    public GameObject jumpersContentObject;
    public GameObject jumperPrefab;
    public SpriteAtlas flagsSprite;

    List<Competitor> allCompetitors;
    List<GameObject> competitorsObjList;
    ClassificationsListUI classificationsListUI;
    EventsListUI eventsListUI;
    public void CreateCompetitorsList()
    {
        competitorsObjList = new List<GameObject>();
        Dictionary<string, int> countriesDict = new Dictionary<string, int>();

        string filePath = Path.Combine(Application.streamingAssetsPath, countryDataFileName);
        CountryData countryData = new CountryData();
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            countryData = JsonConvert.DeserializeObject<CountryData>(dataAsJson);
            for (int i = 0; i < countryData.spritesList.Count; i++)
            {
                countriesDict.Add(countryData.spritesList[i], i);
            }

            foreach (var c in countryData.countryList)
            {
                countriesDict.Add(c.ioc, countriesDict[c.alpha2]);
            }
        }

        foreach (var c in countryData.countryList)
        {
            Debug.Log(c.en + " " + countriesDict[c.ioc] + " " + countriesDict[c.alpha2]);
        }

        filePath = Path.Combine(Application.streamingAssetsPath, competitorsFileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            allCompetitors = JsonConvert.DeserializeObject<List<Competitor>>(dataAsJson);
            foreach (var c in allCompetitors)
            {
                GameObject tmp = Instantiate(jumperPrefab);
                tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = c.lastName.ToUpper() + " " + c.firstName;
                tmp.GetComponentsInChildren<Image>()[3].sprite = flagsSprite.GetSprite("flags_responsive_uncompressed_" + countriesDict[c.countryCode].ToString());
                tmp.transform.SetParent(jumpersContentObject.transform);
                competitorsObjList.Add(tmp);
            }
        }
    }
    public void OnFinish()
    {
        CalendarResults calendarResults = new CalendarResults();
        calendarResults.calendar = new Calendar.Calendar();
        for (int i = 0; i < allCompetitors.Count; i++)
        {
            if (competitorsObjList[i].GetComponentInChildren<Toggle>().isOn)
            {
                calendarResults.calendar.competitors.Add(allCompetitors[i]);
            }
        }

        calendarResults.calendar.classifications = classificationsListUI.classificationsList;
        calendarResults.calendar.events = eventsListUI.eventsList;
        for (int i = 0; i < calendarResults.calendar.events.Count; i++)
        {
            foreach (var jt in calendarResults.calendar.events[i].classifications)
            {
                calendarResults.calendar.classifications[jt].events.Add(i);
            }
        }

        calendarResults.eventResults = new EventResults[calendarResults.calendar.events.Count];
        calendarResults.classificationResults = new ClassificationResults[calendarResults.calendar.classifications.Count];
        for (int i = 0; i < calendarResults.classificationResults.Length; i++)
        {
            calendarResults.classificationResults[i] = new ClassificationResults();
            calendarResults.classificationResults[i].totalResults = new decimal[calendarResults.calendar.competitors.Count];
            calendarResults.classificationResults[i].eventResults = new List<decimal>[calendarResults.calendar.competitors.Count];
            for (int j = 0; j < calendarResults.classificationResults[i].eventResults.Length; j++)
            {
                calendarResults.classificationResults[i].eventResults[j] = new List<decimal>();
            }
        }
        calendarResults.eventIt = 0;

        if (File.Exists(Path.Combine(Application.streamingAssetsPath, "data.json")))
        {
            calendarResults.hillProfiles = JsonConvert.DeserializeObject<HillProfile.AllData>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "data.json"))).profileData;
        }

        calendarResults.calendar.hillInfos = new List<HillInfo>();
        foreach (var item in calendarResults.hillProfiles)
        {
            calendarResults.calendar.hillInfos.Add(new HillInfo((decimal)item.w, (decimal)(item.w + item.l2)));
        }

        string filePath = Path.Combine(Application.streamingAssetsPath, savesPath);
        string dataAsJson = JsonConvert.SerializeObject(calendarResults);
        File.WriteAllText(filePath, dataAsJson);
        SceneManager.LoadScene("Scenes/Hills/Templates/HillTemplateEditor");
    }

    void Start()
    {
        CreateCompetitorsList();
        classificationsListUI = GetComponent<ClassificationsListUI>();
        eventsListUI = GetComponent<EventsListUI>();
    }
}