using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using CompCal;

public class EventsListUI : ListDisplay
{
    public DatabaseManager databaseManager;
    public TMPro.TMP_Dropdown hillsDropdown;

    public TMPro.TMP_Dropdown eventPresetDropdown;
    public TMPro.TMP_Dropdown eventTypeDropdown;
    public TMPro.TMP_Dropdown inLimitTypeDropdown;
    public TMPro.TMP_Dropdown qualRankTypeDropdown;
    public TMPro.TMP_Dropdown ordRankTypeDropdown;

    public TMPro.TMP_Dropdown qualRankEventDropdown;
    public TMPro.TMP_Dropdown ordRankEventDropdown;

    public TMPro.TMP_InputField inLimitInput;

    public GameObject classificationsToggleGroup;
    public GameObject classificationToggle;

    public List<CompCal.Event> eventsList;
    private bool[] classificationsMask;
    private List<HillProfile.ProfileData> hillsList;
    private List<GameObject> classificationToggles;
    private List<RoundInfoPreset> presetsList;

    public override void ListInit()
    {
        eventsList = new List<CompCal.Event>();
        hillsList = new List<HillProfile.ProfileData>();
        if (databaseManager.dbHills.Loaded) { hillsList = databaseManager.dbHills.Data.profileData; }

        var optionsList = hillsList.Select(it => new TMPro.TMP_Dropdown.OptionData(it.name));
        hillsDropdown.options = new List<TMPro.TMP_Dropdown.OptionData>(optionsList);

        presetsList = new List<RoundInfoPreset>();
        if (databaseManager.dbRoundInfoPresets.Loaded) { presetsList = databaseManager.dbRoundInfoPresets.Data; }

        var optionsList1 = presetsList.Select(it => new TMPro.TMP_Dropdown.OptionData(it.name));
        eventPresetDropdown.options = new List<TMPro.TMP_Dropdown.OptionData>(optionsList1);
    }

    public override void ShowElementInfo(int index)
    {
        hillsDropdown.value = eventsList[index].hillId;
        eventTypeDropdown.value = (int)eventsList[index].eventType;
        inLimitTypeDropdown.value = (int)eventsList[index].inLimitType;
        inLimitInput.text = eventsList[index].inLimit.ToString();
        eventPresetDropdown.value = (int)eventsList[index].eventPreset;

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

    public void LoadList(List<CompCal.Event> tmpList)
    {
        ClearListElement();

        if (tmpList == null) { tmpList = new List<CompCal.Event>(); }
        ListInit();
        foreach (var item in tmpList)
        {
            eventsList.Add(item);
            AddListElement(NewListElement(item));
        }
    }

    public GameObject NewListElement(CompCal.Event e)
    {
        GameObject tmp = Instantiate(elementPrefab);
        SetValue(tmp, e);
        return tmp;
    }

    public void SetValue(GameObject tmp, CompCal.Event e)
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
        if (ordRankTypeDropdown.value == (int)CompCal.RankType.Event)
        {
            for (int i = 0; i < currentIndex; i++)
            {
                tmp.Add(hillsDropdown.options[eventsList[i].hillId].text);
            }
        }
        else if (ordRankTypeDropdown.value == (int)CompCal.RankType.Classification)
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
        if (qualRankTypeDropdown.value == (int)CompCal.RankType.Event)
        {
            for (int i = 0; i < currentIndex; i++)
            {
                Debug.Log(hillsDropdown.options[eventsList[i].hillId].text);
                tmp.Add(hillsDropdown.options[eventsList[i].hillId].text);
            }
        }
        else if (qualRankTypeDropdown.value == (int)CompCal.RankType.Classification)
        {
            for (int i = 0; i < GetComponent<ClassificationsListUI>().classificationsList.Count; i++)
            {
                Debug.Log(GetComponent<ClassificationsListUI>().classificationsList[i].name);
                tmp.Add(GetComponent<ClassificationsListUI>().classificationsList[i].name);
            }
        }
        qualRankEventDropdown.AddOptions(tmp);
    }



    public void Add()
    {
        CompCal.Event e = new CompCal.Event("New Event", 0, CompCal.EventType.Individual, new List<RoundInfo>(), new List<int>(), RankType.None, 0, RankType.None, 0);
        eventsList.Add(e);
        AddListElement(NewListElement(e));
        Save();
    }

    public void Save()
    {
        eventsList[currentIndex].hillId = hillsDropdown.value;
        eventsList[currentIndex].eventType = (CompCal.EventType)eventTypeDropdown.value;
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

        eventsList[currentIndex].ordRankType = (CompCal.RankType)ordRankTypeDropdown.value;
        eventsList[currentIndex].ordRankId = ordRankEventDropdown.value;

        eventsList[currentIndex].qualRankType = (CompCal.RankType)qualRankTypeDropdown.value;
        eventsList[currentIndex].qualRankId = qualRankEventDropdown.value;
        eventsList[currentIndex].eventPreset = eventPresetDropdown.value;

        eventsList[currentIndex].roundInfos = presetsList[eventPresetDropdown.value].roundInfos;

        SetValue(elementsList[currentIndex], eventsList[currentIndex]);
    }

    public void Delete()
    {
        eventsList.RemoveAt(currentIndex);
        DeleteListElement();
    }

}
