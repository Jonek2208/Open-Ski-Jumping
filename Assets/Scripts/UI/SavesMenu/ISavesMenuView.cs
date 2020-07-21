using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.UI.SavesMenu
{
    public interface ISavesMenuView
    {
        GameSave SelectedSave { get; set; }
        IEnumerable<GameSave> Saves { set; }
        Calendar SelectedCalendar { get; }
        IEnumerable<Calendar> Calendars { set; }
        string CurrentSaveName { set; }
        string CurrentCalendarName { set; }
        string NewSaveName { get; }

        event Action OnDataReload;
        event Action OnSelectionChanged;
        event Action OnAdd;
        event Action OnRemove;
        event Action OnSubmit;

        bool SaveInfoEnabled { set; }
        void HidePopUp();
        void ShowPopUp();
        void ShowPrompt();
    }
}