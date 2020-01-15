using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using CompCal;

public class SavesListUI : ListDisplay
{
    public DatabaseManager databaseManager;

    public TMPro.TMP_Dropdown calendarsDropdown;
    public List<GameSave> savesList;
    private List<CompCal.Calendar> calendarsList;

    public GameObject popUpObject;
    public TMPro.TMP_InputField calendarNameInput;
    public TMPro.TMP_Text promptText;


    public override void ListInit()
    {
        savesList = databaseManager.dbSaveData.savesList;
        foreach (var item in savesList)
        {
            AddListElement(NewListElement(item));
        }

        calendarsList = new List<CompCal.Calendar>();
        if (databaseManager.dbCalendars.Loaded) { calendarsList = databaseManager.dbCalendars.Data; }
        LoadCalendarsList();
    }

    public void LoadCalendarsList()
    {
        var optionsList = calendarsList.Select(it => new TMPro.TMP_Dropdown.OptionData(it.name));
        calendarsDropdown.options = new List<TMPro.TMP_Dropdown.OptionData>(optionsList);
    }

    public override void ShowElementInfo(int index)
    {

    }

    public GameObject NewListElement(GameSave item)
    {
        GameObject tmp = Instantiate(elementPrefab);
        SetValue(tmp, item);
        return tmp;
    }

    public void SetValue(GameObject tmp, GameSave item)
    {
        tmp.GetComponentInChildren<TMPro.TMP_Text>().text = item.name;
    }

    public GameSave CreateGameSaveFromCalendar(Calendar calendar)
    {
        GameSave gameSave = new GameSave();
        gameSave.resultsContainer = new ResultsContainer();
        gameSave.calendar = calendar;

        gameSave.resultsContainer.eventResults = new EventResults[gameSave.calendar.events.Count];
        gameSave.resultsContainer.classificationResults = new ClassificationResults[gameSave.calendar.classifications.Count];

        for (int i = 0; i < gameSave.resultsContainer.classificationResults.Length; i++)
        {
            gameSave.resultsContainer.classificationResults[i] = new ClassificationResults();
        }

        if (databaseManager.dbHills.Loaded)
        {
            gameSave.resultsContainer.hillProfiles = databaseManager.dbHills.Data.profileData;
        }

        gameSave.resultsContainer.hillInfos = new List<HillInfo>();
        foreach (var item in gameSave.resultsContainer.hillProfiles)
        {
            gameSave.resultsContainer.hillInfos.Add(new HillInfo((decimal)item.w, (decimal)(item.w + item.l2)));
        }
        return gameSave;
    }


    public void ShowPopUp()
    {

        promptText.text = "";
        popUpObject.SetActive(true);
        LoadCalendarsList();
    }

    public void ClosePopUp()
    {
        popUpObject.SetActive(false);
    }

    public void OnSaveButton()
    {
        if (calendarNameInput.text.Equals(""))
        {
            promptText.text = "Save's name must not be empty!";
        }
        else
        {
            Save();
            ClosePopUp();
        }
    }

    public void Save()
    {
        GameSave gameSave = CreateGameSaveFromCalendar(calendarsList[calendarsDropdown.value]);
        gameSave.name = calendarNameInput.text;
        savesList.Add(gameSave);
        AddListElement(NewListElement(gameSave));
        SetValue(elementsList[currentIndex], savesList[currentIndex]);
    }

    public void Play()
    {
        if (0 <= currentIndex && currentIndex < savesList.Count)
        {
            databaseManager.dbSaveData.currentSaveId = currentIndex;
            databaseManager.Save();
        }

    }

    public void Delete()
    {
        savesList.RemoveAt(currentIndex);
        DeleteListElement();
    }

}