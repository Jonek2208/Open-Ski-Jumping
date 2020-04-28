using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.UI.CalendarEditor.Competitors
{
    public interface ICalendarEditorJumpersView
    {
        IEnumerable<Competitor> SelectedJumpers { get; set; }
        IEnumerable<Competitor> Jumpers { set; }

        event Action OnDataSave;
        event Action OnDataReload;
    }
}