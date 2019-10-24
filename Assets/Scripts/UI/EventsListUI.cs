using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using CompetitionClasses;

public class EventsListUI : ListDisplay
{
    public TMPro.TMP_Dropdown hillsDropdown;
    public TMPro.TMP_Dropdown eventTypeDropdown;
    public TMPro.TMP_Dropdown inLimitTypeDropdown;
    public TMPro.TMP_Dropdown qualRankTypeDropdown;
    public TMPro.TMP_Dropdown ordRankTypeDropdown;

    public TMPro.TMP_Dropdown qualRankEventDropdown;
    public TMPro.TMP_Dropdown ordRankEventDropdown;

    public TMPro.TMP_InputField inLimitInput;

    public GameObject classificationsToggleGroup;
    public GameObject classificationToggle;

    public List<CompetitionClasses.Event> eventsList;
    bool[] classificationsMask;
    List<GameObject> classificationToggles;

    public override void ListInit()
    {
        eventsList = new List<CompetitionClasses.Event>();

        List<TMPro.TMP_Dropdown.OptionData> hillsList = new List<TMPro.TMP_Dropdown.OptionData>();
        string filePath = Path.Combine(Application.streamingAssetsPath, "data.json");
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            List<HillProfile.ProfileData> hills = JsonConvert.DeserializeObject<HillProfile.AllData>(dataAsJson).profileData;
            foreach (var item in hills)
            {
                hillsList.Add(new TMPro.TMP_Dropdown.OptionData(item.name));
            }
        }

        hillsDropdown.options = hillsList;
    }
    public GameObject NewListElement(CompetitionClasses.Event e)
    {
        GameObject tmp = Instantiate(elementPrefab);
        SetValue(tmp, e);
        return tmp;
    }

    public void SetValue(GameObject tmp, CompetitionClasses.Event e)
    {
        tmp.GetComponentInChildren<TMPro.TMP_Text>().text = hillsDropdown.options[e.hillId].text;
    }

    public void CreateClassificationsList()
    {
        List<Classification> classificationsList = GetComponent<ClassificationsListUI>().classificationsList;
        classificationsMask = new bool[classificationsList.Count];

        if (classificationToggles == null) { classificationToggles = new List<GameObject>(); }

        foreach (var item in classificationToggles)
        {
            Destroy(item);
        }


        classificationToggles = new List<GameObject>();

        foreach (var c in classificationsList)
        {
            GameObject tmp = Instantiate(classificationToggle);
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = c.name;
            tmp.transform.SetParent(classificationsToggleGroup.transform);

            classificationToggles.Add(tmp);
        }
    }

    public void CreateOrdRankDropdownList()
    {
        ordRankEventDropdown.ClearOptions();
        List<string> tmp = new List<string>();
        Debug.Log(ordRankTypeDropdown.value);
        if (ordRankTypeDropdown.value == (int)CompetitionClasses.RankType.Event)
        {
            for (int i = 0; i < eventsList.Count; i++)
            {
                tmp.Add(hillsDropdown.options[eventsList[i].hillId].text);
            }
        }
        else if (ordRankTypeDropdown.value == (int)CompetitionClasses.RankType.Classification)
        {
            for (int i = 0; i < GetComponent<ClassificationsListUI>().classificationsList.Count; i++)
            {
                tmp.Add(GetComponent<ClassificationsListUI>().classificationsList[i].name);
            }
        }
        ordRankEventDropdown.AddOptions(tmp);
    }

    public void CreateQualRankDropdownList()
    {
        qualRankEventDropdown.ClearOptions();
        List<string> tmp = new List<string>();
        Debug.Log(qualRankTypeDropdown.value);
        if (qualRankTypeDropdown.value == (int)CompetitionClasses.RankType.Event)
        {
            for (int i = 0; i < eventsList.Count; i++)
            {
                Debug.Log(hillsDropdown.options[eventsList[i].hillId].text);
                tmp.Add(hillsDropdown.options[eventsList[i].hillId].text);
            }
        }
        else if (qualRankTypeDropdown.value == (int)CompetitionClasses.RankType.Classification)
        {
            for (int i = 0; i < GetComponent<ClassificationsListUI>().classificationsList.Count; i++)
            {
                Debug.Log(GetComponent<ClassificationsListUI>().classificationsList[i].name);
                tmp.Add(GetComponent<ClassificationsListUI>().classificationsList[i].name);
            }
        }
        qualRankEventDropdown.AddOptions(tmp);
    }

    public override void ShowElementInfo(int index)
    {
        hillsDropdown.value = eventsList[index].hillId;
        eventTypeDropdown.value = (int)eventsList[index].eventType;
        inLimitTypeDropdown.value = (int)eventsList[index].inLimitType;
        inLimitInput.text = eventsList[index].inLimit.ToString();

        if (!GetComponent<ClassificationsListUI>().updated)
        {
            CreateClassificationsList();
            GetComponent<ClassificationsListUI>().updated = true;
        }

        for (int i = 0; i < classificationsMask.Length; i++) { classificationsMask[i] = false; }
        foreach (var item in eventsList[index].classifications) { classificationsMask[item] = true; }
        Debug.Log(classificationsMask);
        for (int i = 0; i < classificationsMask.Length; i++)
        {
            classificationToggles[i].GetComponent<Toggle>().isOn = classificationsMask[i];
        }

        ordRankTypeDropdown.value = (int)eventsList[currentIndex].ordRankType;
        ordRankEventDropdown.value = eventsList[currentIndex].ordRankId;

        qualRankTypeDropdown.value = (int)eventsList[currentIndex].qualRankType;
        qualRankEventDropdown.value = eventsList[currentIndex].qualRankId;
    }

    public void Add()
    {
        CompetitionClasses.Event e = new CompetitionClasses.Event("New Event", 0, CompetitionClasses.EventType.Individual, new List<RoundInfo>(), new List<int>(), RankType.None, 0, RankType.None, 0);
        eventsList.Add(e);
        AddListElement(NewListElement(e));
    }

    public void Save()
    {
        eventsList[currentIndex].hillId = hillsDropdown.value;
        eventsList[currentIndex].eventType = (CompetitionClasses.EventType)eventTypeDropdown.value;
        eventsList[currentIndex].inLimitType = (LimitType)inLimitTypeDropdown.value;
        eventsList[currentIndex].inLimit = int.Parse(inLimitInput.text);

        eventsList[currentIndex].classifications = new List<int>();
        for (int i = 0; i < classificationsMask.Length; i++)
        {
            if (classificationToggles[i].GetComponent<Toggle>().isOn)
            {
                eventsList[currentIndex].classifications.Add(i); //Debug.Log(i);
            }
        }

        eventsList[currentIndex].ordRankType = (CompetitionClasses.RankType)ordRankTypeDropdown.value;
        eventsList[currentIndex].ordRankId = ordRankEventDropdown.value;

        eventsList[currentIndex].qualRankType = (CompetitionClasses.RankType)qualRankTypeDropdown.value;
        eventsList[currentIndex].qualRankId = qualRankEventDropdown.value;

        SetValue(elementsList[currentIndex], eventsList[currentIndex]);
    }

    public void Delete()
    {
        eventsList.RemoveAt(currentIndex);
        DeleteListElement();
    }

}
