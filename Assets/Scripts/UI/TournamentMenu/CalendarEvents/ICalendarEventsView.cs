using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;
using OpenSkiJumping.UI.TournamentMenu.CalendarEventInfo;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEvents
{
    public interface ICalendarEventsView
    {
        ICalendarEventInfoView EventInfoView { get; }
        EventInfo SelectedEvent { get; set; }
        IEnumerable<EventInfo> Events { set; }

        event Action OnSelectionChanged;
        event Action OnDataReload;
    }
}