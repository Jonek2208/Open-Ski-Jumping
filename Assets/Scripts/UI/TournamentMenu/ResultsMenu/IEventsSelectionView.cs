using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.UI.TournamentMenu.ResultsMenu
{
    public interface IEventsSelectionView
    {
        EventInfo SelectedEvent { get; set; }
        int CurrentEventIndex { get; }
        IEnumerable<EventInfo> Events { set; }
        IResultsListController ResultsListController { get; }

        event Action OnSelectionChanged;
        event Action OnDataReload;
    }
}