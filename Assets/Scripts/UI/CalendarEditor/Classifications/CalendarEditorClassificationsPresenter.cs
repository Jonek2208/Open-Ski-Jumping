using Competition;
using Competition.Persistent;

namespace UI.CalendarEditor.Classifications
{
    public class CalendarEditorClassificationsPresenter
    {
        private readonly CalendarFactory calendarFactory;
        private readonly ICalendarEditorClassificationsView view;

        public CalendarEditorClassificationsPresenter(ICalendarEditorClassificationsView view,
            CalendarFactory calendarFactory)
        {
            this.view = view;
            this.calendarFactory = calendarFactory;

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
            calendarFactory.Classifications.Add(classification);
            PresentList();
            view.SelectClassification(classification);
            PresentClassificationInfo();
        }

        private void RemoveClassification()
        {
            var jumper = view.SelectedClassification;
            if (jumper == null) return;

            var val = calendarFactory.Classifications.Remove(jumper);

            PresentList();
            view.SelectClassification(null);
            PresentClassificationInfo();
        }

        private void PresentClassificationInfo()
        {
            var classification = view.SelectedClassification;
            if (classification == null)
            {
                view.HideClassificationInfo();
                return;
            }

            view.ShowClassificationInfo();
            view.BlockJumperInfoCallbacks = true;

            view.Name = classification.name;
            view.EventType = (int) classification.eventType;
            view.ClassificationType = (int) classification.classificationType;
            view.TeamClassificationLimit = classification.teamCompetitorsLimit;
            view.TeamClassificationLimitType = (int) classification.teamClassificationLimitType;
            view.MedalPlaces = classification.medalPlaces;
            if (classification.pointsTables.Count >= 1)
                view.SelectedPointsTableIndividual = classification.pointsTables[0];
            if (classification.pointsTables.Count >= 2)
                view.SelectedPointsTableTeam = classification.pointsTables[1];
            view.BlockJumperInfoCallbacks = false;
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
            PresentList();
            view.SelectClassification(classification);
        }

        private void InitEvents()
        {
            view.OnSelectionChanged += PresentClassificationInfo;
            view.OnAdd += CreateNewClassification;
            view.OnRemove += RemoveClassification;
            view.OnCurrentClassificationChanged += SaveClassificationInfo;
        }

        private void SetInitValues()
        {
            PresentList();
            PresentClassificationInfo();
        }
    }
}