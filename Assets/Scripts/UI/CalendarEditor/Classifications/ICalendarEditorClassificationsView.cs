using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.UI.CalendarEditor.Classifications
{
    public interface ICalendarEditorClassificationsView
    {
        ClassificationInfo SelectedClassification { get; set; }
        IEnumerable<ClassificationInfo> Classifications { set; }

        IEnumerable<PointsTable> PointsTables { set; }
        PointsTable SelectedPointsTableIndividual { get; set; }
        PointsTable SelectedPointsTableTeam { get; set; }

        string Name { get; set; }
        int EventType { get; set; }
        int ClassificationType { get; set; }
        int TeamClassificationLimitType { get; set; }
        int TeamClassificationLimit { get; set; }
        int MedalPlaces { get; set; }
        string BibColor { get; set; }

        event Action OnSelectionChanged;
        event Action OnCurrentClassificationChanged;
        event Action OnAdd;
        event Action OnRemove;
        event Action OnDataReload;

        bool ClassificationInfoEnabled { set; }
    }
}