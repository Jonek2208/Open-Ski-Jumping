using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using CompCal;

public class SavesListUI : ListDisplay
{
    public DatabaseManager databaseManager;

    public TMPro.TMP_Dropdown calendarsDropdown;
    public List<CompCal.CalendarResults> savesList;
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

    public GameObject NewListElement(CompCal.CalendarResults item)
    {
        GameObject tmp = Instantiate(elementPrefab);
        SetValue(tmp, item);
        return tmp;
    }

    public void SetValue(GameObject tmp, CompCal.CalendarResults item)
    {
        tmp.GetComponentInChildren<TMPro.TMP_Text>().text = item.name;
    }

    public CalendarResults CreateCalendarResultsFromCalendar(Calendar calendar)
    {
        CalendarResults calendarResults = new CalendarResults();
        calendarResults.calendar = calendar;

        calendarResults.eventResults = new EventResults[calendarResults.calendar.events.Count];
        calendarResults.classificationResults = new ClassificationResults[calendarResults.calendar.classifications.Count];

        for (int i = 0; i < calendarResults.classificationResults.Length; i++)
        {
            calendarResults.classificationResults[i] = new ClassificationResults();
            calendarResults.classificationResults[i].totalResults = new decimal[calendarResults.calendar.competitors.Count];
            calendarResults.classificationResults[i].rank = new int[calendarResults.calendar.competitors.Count];
            calendarResults.classificationResults[i].eventResults = new List<decimal>[calendarResults.calendar.competitors.Count];
            for (int j = 0; j < calendarResults.classificationResults[i].eventResults.Length; j++)
            {
                calendarResults.classificationResults[i].eventResults[j] = new List<decimal>();
            }
        }

        if (databaseManager.dbHills.Loaded)
        {
            calendarResults.hillProfiles = databaseManager.dbHills.Data.profileData;
        }

        calendarResults.hillInfos = new List<HillInfo>();
        foreach (var item in calendarResults.hillProfiles)
        {
            calendarResults.hillInfos.Add(new HillInfo((decimal)item.w, (decimal)(item.w + item.l2)));
        }
        return calendarResults;
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
        CalendarResults calendarResults = CreateCalendarResultsFromCalendar(calendarsList[calendarsDropdown.value]);
        calendarResults.name = calendarNameInput.text;
        savesList.Add(calendarResults);
        AddListElement(NewListElement(calendarResults));
        SetValue(elementsList[currentIndex], savesList[currentIndex]);
    }

    public void Play()
    {
        if (0 <= currentIndex && currentIndex < savesList.Count)
        {
            databaseManager.dbSaveData.currentSaveId = currentIndex;
            databaseManager.Save();
            MainMenuController.LoadTournamentMenu();
        }

    }

    public void Delete()
    {
        savesList.RemoveAt(currentIndex);
        DeleteListElement();
    }

}