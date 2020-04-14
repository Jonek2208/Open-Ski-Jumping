using System;
using System.Collections.Generic;
using Competition.Persistent;

namespace UI.CalendarsMenu
{
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
        void HideCalendarInfo();
        void ShowPopUp();
        void ShowPrompt();
        void ShowCalendarInfo();
    }
}