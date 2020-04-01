using System;
using System.Collections.Generic;
using System.Linq;
using Competition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICalendarsMenuView
{
    Calendar SelectedCalendar { get; }
    IEnumerable<Calendar> Calendars { set; }
    string CurrentCalendarName { set; }
    string NewCalendarName { get; }

    event Action OnSelectionChanged;
    event Action OnAdd;
    event Action OnRemove;
    event Action OnSubmit;

    void SelectCalendar(Calendar calendar);
    void HidePopUp();
    void HideSaveInfo();
    void ShowPopUp();
    void ShowPrompt();
    void ShowSaveInfo();
}