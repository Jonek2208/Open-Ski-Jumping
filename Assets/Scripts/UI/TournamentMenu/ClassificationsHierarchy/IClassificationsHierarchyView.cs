using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;

namespace OpenSkiJumping.UI.TournamentMenu.ClassificationsHierarchy
{
    public interface IClassificationsHierarchyView 
    {
        ClassificationData SelectedClassification { get; set; }
        IEnumerable<ClassificationData> Classifications { set; }

        event Action OnSelectionChanged;
        event Action<ClassificationData, bool> OnChangeBibState;
        event Action OnMoveUp;
        event Action OnMoveDown;
        event Action OnDataReload;
    }
}