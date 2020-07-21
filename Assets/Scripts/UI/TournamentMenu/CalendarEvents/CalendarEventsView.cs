using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEvents
{
    public class CalendarEventsView : MonoBehaviour, ICalendarEventsView
    {
        public EventInfo SelectedEvent { get; set; }
        public IEnumerable<EventInfo> Events { get; set; }
        public event Action OnSelectionChanged;
        public event Action OnDataReload;
    }
}