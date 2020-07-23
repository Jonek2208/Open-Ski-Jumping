using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.UI.TournamentMenu.ResultsMenu
{
    public interface IClassificationsSelectionView
    {
        ClassificationInfo SelectedClassification { get; set; }
        int CurrentClassificationIndex { get; }
        IEnumerable<ClassificationInfo> Classifications { set; }
        IResultsListController ResultsListController { get; }

        event Action OnSelectionChanged;
        event Action OnDataReload;
    }
}