using System;
using System.Collections.Generic;
using Competition.Persistent;

namespace UI.SavesMenu
{
    public interface ISavesMenuView
    {
        GameSave SelectedSave { get; }
        IEnumerable<GameSave> Saves { set; }
        Calendar SelectedCalendar { get; }
        IEnumerable<Calendar> Calendars { set; }
        string CurrentSaveName { set; }
        string CurrentCalendarName { set; }
        string NewSaveName { get; }

        event Action OnSelectionChanged;
        event Action OnAdd;
        event Action OnRemove;
        event Action OnSubmit;

        void SelectSave(GameSave save);
        void HidePopUp();
        void HideSaveInfo();
        void ShowPopUp();
        void ShowPrompt();
        void ShowSaveInfo();
    }
}