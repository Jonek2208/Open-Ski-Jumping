using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEvents
{
    public interface ICalendarEventsView
    {
        EventInfo SelectedEvent { get; set; }
        IEnumerable<EventInfo> Events { set; }

        event Action OnSelectionChanged;
        event Action OnDataReload;
    }
}