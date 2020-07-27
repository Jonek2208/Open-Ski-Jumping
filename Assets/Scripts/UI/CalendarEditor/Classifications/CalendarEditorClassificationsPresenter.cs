using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;

namespace OpenSkiJumping.UI.CalendarEditor.Classifications
{
    public class CalendarEditorClassificationsPresenter
    {
        private readonly CalendarFactory calendarFactory;
        private readonly PointsTablesRuntime pointsTables;
        private readonly ICalendarEditorClassificationsView view;

        public CalendarEditorClassificationsPresenter(ICalendarEditorClassificationsView view,
            CalendarFactory calendarFactory, PointsTablesRuntime pointsTables)
        {
            this.view = view;
            this.calendarFactory = calendarFactory;
            this.pointsTables = pointsTables;

            InitEvents();
            SetInitValues();
        }

        private void PresentList()
        {
            view.Classifications = calendarFactory.Classifications;
        }

        private void CreateNewClassification()
        {
            var classification = new ClassificationInfo();
            calendarFactory.AddClassification(classification);
            PresentList();
            view.SelectedClassification = classification;
            PresentClassificationInfo();
        }

        private void RemoveClassification()
        {
            var item = view.SelectedClassification;
            if (item == null) return;

            var val = calendarFactory.RemoveClassification(item);

            PresentList();
            view.SelectedClassification = null;
            PresentClassificationInfo();
        }

        private void PresentClassificationInfo()
        {
            var classification = view.SelectedClassification;
            if (classification == null)
            {
                view.ClassificationInfoEnabled = false;
                return;
            }

            view.ClassificationInfoEnabled = true;

            view.Name = classification.name;
            view.EventType = (int) classification.eventType;
            view.ClassificationType = (int) classification.classificationType;
            view.TeamClassificationLimit = classification.teamCompetitorsLimit;
            view.TeamClassificationLimitType = (int) classification.teamClassificationLimitType;
            view.MedalPlaces = classification.medalPlaces;
            view.BibColor = classification.leaderBibColor;
            if (classification.pointsTables.Count >= 1)
                view.SelectedPointsTableIndividual = classification.pointsTables[0];
            if (classification.pointsTables.Count >= 2)
                view.SelectedPointsTableTeam = classification.pointsTables[1];
        }

        private void SaveClassificationInfo()
        {
            var classification = view.SelectedClassification;
            if (classification == null) return;

            classification.name = view.Name;
            classification.eventType = (EventType) view.EventType;
            classification.classificationType = (ClassificationType) view.ClassificationType;
            classification.teamCompetitorsLimit = view.TeamClassificationLimit;
            classification.teamClassificationLimitType = (TeamClassificationLimitType) view.TeamClassificationLimitType;
            classification.medalPlaces = view.MedalPlaces;
            classification.leaderBibColor = view.BibColor;
            if (classification.classificationType == ClassificationType.Place)
            {
                classification.pointsTables = new List<PointsTable> {view.SelectedPointsTableIndividual};
                if (classification.eventType == EventType.Team)
                    classification.pointsTables.Add(view.SelectedPointsTableTeam);
            }

            PresentList();
            view.SelectedClassification = classification;
        }

        private void InitEvents()
        {
            view.OnSelectionChanged += PresentClassificationInfo;
            view.OnAdd += CreateNewClassification;
            view.OnRemove += RemoveClassification;
            view.OnCurrentClassificationChanged += SaveClassificationInfo;
            view.OnDataReload += SetInitValues;
        }

        private void SetInitValues()
        {
            view.PointsTablesIndividual = pointsTables.GetData(0);
            view.PointsTablesTeam = pointsTables.GetData(1);
            PresentList();
            view.SelectedClassification = calendarFactory.Classifications.FirstOrDefault();
            PresentClassificationInfo();
        }
    }
}