using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.UI.CalendarsMenu
{
    public interface ICalendarsMenuView
    {
        Calendar SelectedCalendar { get; set; }
        IEnumerable<Calendar> Calendars { set; }
        string CurrentCalendarName { set; }
        string NewCalendarName { get; }

        event Action OnSelectionChanged;
        event Action OnAdd;
        event Action OnRemove;
        event Action OnSubmit;

        bool CalendarInfoEnabled { set; }
        void HidePopUp();
        void ShowPopUp();
        void ShowPrompt();
    }
}