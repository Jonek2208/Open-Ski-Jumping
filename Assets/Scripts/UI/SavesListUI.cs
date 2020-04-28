using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.UI
{
    // public class SavesListUI : ListDisplay
    // {
    //     public DatabaseManager databaseManager;
    //
    //     public TMP_Dropdown calendarsDropdown;
    //     public List<GameSave> savesList;
    //     private List<Calendar> calendarsList;
    //
    //     public GameObject popUpObject;
    //     public TMP_InputField calendarNameInput;
    //     public TMP_Text promptText;
    //
    //
    //     public override void ListInit()
    //     {
    //         savesList = databaseManager.dbSaveData.savesList;
    //         foreach (var item in savesList)
    //         {
    //             AddListElement(NewListElement(item));
    //         }
    //
    //         calendarsList = new List<Calendar>();
    //         if (databaseManager.dbCalendars.Loaded) { calendarsList = databaseManager.dbCalendars.Data; }
    //         LoadCalendarsList();
    //     }
    //
    //     public void LoadCalendarsList()
    //     {
    //         var optionsList = calendarsList.Select(it => new TMP_Dropdown.OptionData(it.name));
    //         calendarsDropdown.options = new List<TMP_Dropdown.OptionData>(optionsList);
    //     }
    //
    //     public override void ShowElementInfo(int index)
    //     {
    //
    //     }
    //
    //     public GameObject NewListElement(GameSave item)
    //     {
    //         GameObject tmp = Instantiate(elementPrefab);
    //         SetValue(tmp, item);
    //         return tmp;
    //     }
    //
    //     public void SetValue(GameObject tmp, GameSave item)
    //     {
    //         tmp.GetComponentInChildren<TMP_Text>().text = item.name;
    //     }
    //
    //     public GameSave CreateGameSaveFromCalendar(Calendar calendar)
    //     {
    //         GameSave gameSave = new GameSave();
    //         gameSave.resultsContainer = new ResultsDatabase();
    //         gameSave.calendar = calendar;
    //
    //         gameSave.resultsContainer.eventResults = new EventResults[gameSave.calendar.events.Count];
    //         gameSave.resultsContainer.classificationResults = new ClassificationResults[gameSave.calendar.classifications.Count];
    //
    //         for (int i = 0; i < gameSave.resultsContainer.classificationResults.Length; i++)
    //         {
    //             gameSave.resultsContainer.classificationResults[i] = new ClassificationResults();
    //         }
    //
    //         return gameSave;
    //     }
    //
    //
    //     public void ShowPopUp()
    //     {
    //
    //         promptText.text = "";
    //         popUpObject.SetActive(true);
    //         LoadCalendarsList();
    //     }
    //
    //     public void ClosePopUp()
    //     {
    //         popUpObject.SetActive(false);
    //     }
    //
    //     public void OnSaveButton()
    //     {
    //         if (calendarNameInput.text.Equals(""))
    //         {
    //             promptText.text = "Save's name must not be empty!";
    //         }
    //         else
    //         {
    //             Save();
    //             ClosePopUp();
    //         }
    //     }
    //
    //     public void Save()
    //     {
    //         GameSave gameSave = CreateGameSaveFromCalendar(calendarsList[calendarsDropdown.value]);
    //         gameSave.name = calendarNameInput.text;
    //         savesList.Add(gameSave);
    //         AddListElement(NewListElement(gameSave));
    //         SetValue(elementsList[currentIndex], savesList[currentIndex]);
    //     }
    //
    //     public void Play()
    //     {
    //         if (0 <= currentIndex && currentIndex < savesList.Count)
    //         {
    //             databaseManager.dbSaveData.currentSaveId = currentIndex;
    //             databaseManager.Save();
    //         }
    //
    //     }
    //
    //     public void Delete()
    //     {
    //         savesList.RemoveAt(currentIndex);
    //         DeleteListElement();
    //     }
    //
    // }
}