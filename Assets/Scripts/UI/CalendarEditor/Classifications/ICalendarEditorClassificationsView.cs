using System;
using System.Collections.Generic;
using Competition.Persistent;

namespace UI.CalendarEditor.Classifications
{
    public interface ICalendarEditorClassificationsView
    {
        ClassificationInfo SelectedClassification { get; }
        IEnumerable<ClassificationInfo> Classifications { set; }

        PointsTable SelectedPointsTableIndividual { get; set; }
        IEnumerable<PointsTable> PointsTablesIndividual { set; }

        PointsTable SelectedPointsTableTeam { get; set; }
        IEnumerable<PointsTable> PointsTablesTeam { set; }

        string Name { get; set; }
        int EventType { get; set; }
        int ClassificationType { get; set; }

        bool BlockJumperInfoCallbacks { get; set; }
        bool BlockSelectionCallbacks { get; set; }

        event Action OnSelectionChanged;
        event Action OnCurrentClassificationChanged;
        event Action OnAdd;
        event Action OnRemove;

        void SelectClassification(ClassificationInfo classification);
    }
}