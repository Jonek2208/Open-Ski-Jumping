using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.UI
{
    // public class CalendarsListUI : ListDisplay
    // {
    //     public DatabaseManager databaseManager;
    //
    //     public List<Calendar> calendarsList;
    //
    //     private void LoadCalendarsData(out List<Calendar> tmpList)
    //     {
    //         tmpList = new List<Calendar>();
    //         if (databaseManager.dbCalendars.Loaded) { tmpList = databaseManager.dbCalendars.Data; }
    //     }
    //
    //     public override void ListInit()
    //     {
    //         LoadCalendarsData(out calendarsList);
    //         foreach (var calendar in calendarsList) { AddListElement(NewListElement(calendar)); }
    //     }
    //
    //     public GameObject NewListElement(Calendar calendar)
    //     {
    //         GameObject tmp = Instantiate(elementPrefab);
    //         SetValue(tmp, calendar);
    //         return tmp;
    //     }
    //
    //     public void SetValue(GameObject tmp, Calendar calendar)
    //     {
    //         tmp.GetComponentsInChildren<TMP_Text>()[0].text = calendar.name;
    //     }
    //     public override void ShowElementInfo(int index) { }
    //
    //     public void Add()
    //     {
    //         GetComponent<CalendarCreatorScript>().LoadCalendar();
    //     }
    //     public void AddCalendar(Calendar calendar)
    //     {
    //         calendarsList.Add(calendar);
    //         AddListElement(NewListElement(calendar));
    //     }
    //     public void Edit()
    //     {
    //         Debug.Log(currentIndex);
    //         SetValue(elementsList[currentIndex], calendarsList[currentIndex]);
    //         GetComponent<CalendarCreatorScript>().LoadCalendar(calendarsList[currentIndex]);
    //     }
    //
    //     public void ChangeCalendar(Calendar calendar)
    //     {
    //         calendarsList[currentIndex] = calendar;
    //     }
    //
    //     public void Delete()
    //     {
    //         calendarsList.RemoveAt(currentIndex);
    //         DeleteListElement();
    //     }
    // }
}
