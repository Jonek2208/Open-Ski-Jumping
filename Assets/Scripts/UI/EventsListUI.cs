using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = OpenSkiJumping.Competition.EventType;

namespace OpenSkiJumping.UI
{
    // public class EventsListUI : ListDisplay
    // {
    //     public DatabaseManager databaseManager;
    //     public TMP_Dropdown hillsDropdown;
    //
    //     public TMP_Dropdown eventPresetDropdown;
    //     public TMP_Dropdown eventTypeDropdown;
    //     public TMP_Dropdown inLimitTypeDropdown;
    //     public TMP_Dropdown qualRankTypeDropdown;
    //     public TMP_Dropdown ordRankTypeDropdown;
    //
    //     public TMP_Dropdown qualRankEventDropdown;
    //     public TMP_Dropdown ordRankEventDropdown;
    //
    //     public TMP_InputField inLimitInput;
    //
    //     public GameObject classificationsToggleGroup;
    //     public GameObject classificationToggle;
    //
    //     public List<EventInfo> eventsList;
    //     private bool[] classificationsMask;
    //     private List<ProfileData> hillsList;
    //     private List<GameObject> classificationToggles;
    //     private List<EventRoundsInfo> presetsList;
    //
    //     public override void ListInit()
    //     {
    //         eventsList = new List<EventInfo>();
    //         hillsList = new List<ProfileData>();
    //         if (databaseManager.dbHills.Loaded)
    //         {
    //             hillsList = databaseManager.dbHills.Data.profileData;
    //         }
    //
    //         var optionsList = hillsList.Select(it => new TMP_Dropdown.OptionData(it.name));
    //         hillsDropdown.options = new List<TMP_Dropdown.OptionData>(optionsList);
    //
    //         presetsList = new List<EventRoundsInfo>();
    //         if (databaseManager.dbRoundInfoPresets.Loaded)
    //         {
    //             presetsList = databaseManager.dbRoundInfoPresets.Data;
    //         }
    //
    //         var optionsList1 = presetsList.Select(it => new TMP_Dropdown.OptionData(it.name));
    //         eventPresetDropdown.options = new List<TMP_Dropdown.OptionData>(optionsList1);
    //     }
    //
    //     public override void ShowElementInfo(int index)
    //     {
    //         changeEventListener.enabled = false;
    //         // hillsDropdown.value = eventsList[index].hillId;
    //         hillsDropdown.value = 0;
    //         eventTypeDropdown.value = (int) eventsList[index].eventType;
    //         inLimitTypeDropdown.value = (int) eventsList[index].inLimitType;
    //         inLimitInput.text = eventsList[index].inLimit.ToString();
    //         // eventPresetDropdown.value = (int)eventsList[index].eventPreset;
    //
    //         if (!GetComponent<ClassificationsListUI>().updated)
    //         {
    //             CreateClassificationsList();
    //             GetComponent<ClassificationsListUI>().updated = true;
    //         }
    //
    //         for (int i = 0; i < classificationsMask.Length; i++)
    //         {
    //             classificationsMask[i] = false;
    //         }
    //
    //         foreach (var item in eventsList[index].classifications)
    //         {
    //             classificationsMask[item] = true;
    //         }
    //
    //         Debug.Log(classificationsMask);
    //         for (int i = 0; i < classificationsMask.Length; i++)
    //         {
    //             classificationToggles[i].GetComponent<Toggle>().isOn = classificationsMask[i];
    //         }
    //
    //         ordRankTypeDropdown.value = (int) eventsList[currentIndex].ordRankType;
    //         ordRankEventDropdown.value = eventsList[currentIndex].ordRankId;
    //
    //         qualRankTypeDropdown.value = (int) eventsList[currentIndex].qualRankType;
    //         qualRankEventDropdown.value = eventsList[currentIndex].qualRankId;
    //         changeEventListener.enabled = true;
    //     }
    //
    //     public void LoadList(List<EventInfo> tmpList)
    //     {
    //         ClearListElement();
    //
    //         if (tmpList == null)
    //         {
    //             tmpList = new List<EventInfo>();
    //         }
    //
    //         ListInit();
    //         foreach (var item in tmpList)
    //         {
    //             eventsList.Add(item);
    //             AddListElement(NewListElement(item));
    //         }
    //     }
    //
    //     public GameObject NewListElement(EventInfo e)
    //     {
    //         GameObject tmp = Instantiate(elementPrefab);
    //         SetValue(tmp, e);
    //         return tmp;
    //     }
    //
    //     public void SetValue(GameObject tmp, EventInfo e)
    //     {
    //         tmp.GetComponentInChildren<TMP_Text>().text = e.hillId;
    //     }
    //
    //     public void CreateClassificationsList()
    //     {
    //         List<ClassificationInfo> classificationsList = GetComponent<ClassificationsListUI>().classificationsList;
    //         classificationsMask = new bool[classificationsList.Count];
    //
    //         if (classificationToggles == null)
    //         {
    //             classificationToggles = new List<GameObject>();
    //         }
    //
    //         foreach (var item in classificationToggles)
    //         {
    //             Destroy(item);
    //         }
    //
    //
    //         classificationToggles = new List<GameObject>();
    //
    //         foreach (var c in classificationsList)
    //         {
    //             GameObject tmp = Instantiate(classificationToggle);
    //             tmp.GetComponentsInChildren<TMP_Text>()[0].text = c.name;
    //             tmp.transform.SetParent(classificationsToggleGroup.transform, false);
    //
    //             classificationToggles.Add(tmp);
    //         }
    //     }
    //
    //     public void CreateOrdRankDropdownList()
    //     {
    //         ordRankEventDropdown.ClearOptions();
    //         List<string> tmp = new List<string>();
    //         Debug.Log(ordRankTypeDropdown.value);
    //         if (ordRankTypeDropdown.value == (int) RankType.Event)
    //         {
    //             for (int i = 0; i < currentIndex; i++)
    //             {
    //                 tmp.Add(eventsList[i].hillId);
    //             }
    //         }
    //         else if (ordRankTypeDropdown.value == (int) RankType.Classification)
    //         {
    //             for (int i = 0; i < GetComponent<ClassificationsListUI>().classificationsList.Count; i++)
    //             {
    //                 tmp.Add(GetComponent<ClassificationsListUI>().classificationsList[i].name);
    //             }
    //         }
    //
    //         ordRankEventDropdown.AddOptions(tmp);
    //     }
    //
    //     public void CreateQualRankDropdownList()
    //     {
    //         qualRankEventDropdown.ClearOptions();
    //         List<string> tmp = new List<string>();
    //         Debug.Log(qualRankTypeDropdown.value);
    //         if (qualRankTypeDropdown.value == (int) RankType.Event)
    //         {
    //             for (int i = 0; i < currentIndex; i++)
    //             {
    //                 // Debug.Log(hillsDropdown.options[eventsList[i].hillId].text);
    //                 tmp.Add(eventsList[i].hillId);
    //             }
    //         }
    //         else if (qualRankTypeDropdown.value == (int) RankType.Classification)
    //         {
    //             for (int i = 0; i < GetComponent<ClassificationsListUI>().classificationsList.Count; i++)
    //             {
    //                 Debug.Log(GetComponent<ClassificationsListUI>().classificationsList[i].name);
    //                 tmp.Add(GetComponent<ClassificationsListUI>().classificationsList[i].name);
    //             }
    //         }
    //
    //         qualRankEventDropdown.AddOptions(tmp);
    //     }
    //
    //
    //     public void Add()
    //     {
    //         EventInfo e = new EventInfo
    //         {
    //             name = "", eventType = EventType.Individual, roundInfos = new EventRoundsInfo(),
    //             classifications = new List<int>(), qualRankType = RankType.None, qualRankId = 0,
    //             ordRankType = RankType.None, ordRankId = 0
    //         };
    //         eventsList.Add(e);
    //         AddListElement(NewListElement(e));
    //         Save();
    //     }
    //
    //     public void Save()
    //     {
    //         changeEventListener.enabled = false;
    //         // eventsList[currentIndex].hillId = hillsDropdown.value;
    //         eventsList[currentIndex].hillId = "";
    //         eventsList[currentIndex].eventType = (EventType) eventTypeDropdown.value;
    //         eventsList[currentIndex].inLimitType = (LimitType) inLimitTypeDropdown.value;
    //         eventsList[currentIndex].inLimit = int.Parse(inLimitInput.text);
    //
    //         eventsList[currentIndex].classifications = new List<int>();
    //         for (int i = 0; i < classificationsMask.Length; i++)
    //         {
    //             if (classificationToggles[i].GetComponent<Toggle>().isOn)
    //             {
    //                 eventsList[currentIndex].classifications.Add(i); //Debug.Log(i);
    //             }
    //         }
    //
    //         eventsList[currentIndex].ordRankType = (RankType) ordRankTypeDropdown.value;
    //         eventsList[currentIndex].ordRankId = ordRankEventDropdown.value;
    //
    //         eventsList[currentIndex].qualRankType = (RankType) qualRankTypeDropdown.value;
    //         eventsList[currentIndex].qualRankId = qualRankEventDropdown.value;
    //         // eventsList[currentIndex].eventPreset = eventPresetDropdown.value;
    //
    //         eventsList[currentIndex].roundInfos = presetsList[eventPresetDropdown.value];
    //         changeEventListener.enabled = true;
    //         SetValue(elementsList[currentIndex], eventsList[currentIndex]);
    //     }
    //
    //     public void Delete()
    //     {
    //         eventsList.RemoveAt(currentIndex);
    //         DeleteListElement();
    //     }
    // }
}