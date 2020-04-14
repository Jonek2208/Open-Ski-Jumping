using System;
using System.Collections.Generic;
using Competition.Persistent;

namespace UI.CalendarEditor.Competitors
{
    public interface ICalendarEditorJumpersView
    {
        IEnumerable<Competitor> SelectedJumpers { get; set; }
        IEnumerable<Competitor> Jumpers { set; }

        event Action OnSelectionSave;
    }
}