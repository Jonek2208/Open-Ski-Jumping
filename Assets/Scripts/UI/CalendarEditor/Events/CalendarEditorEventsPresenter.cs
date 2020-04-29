using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;

namespace OpenSkiJumping.UI.CalendarEditor.Events
{
    public class CalendarEditorEventsPresenter
    {
        private readonly CalendarFactory calendarFactory;
        private readonly HillsRuntime hills;
        private readonly PresetsRuntime presets;
        private readonly ICalendarEditorEventsView view;

        public CalendarEditorEventsPresenter(ICalendarEditorEventsView view, CalendarFactory calendarFactory,
            HillsRuntime hills, PresetsRuntime presets)
        {
            this.view = view;
            this.calendarFactory = calendarFactory;
            this.hills = hills;
            this.presets = presets;

            InitEvents();
            SetInitValues();
        }


        private void PresentList()
        {
            view.Events = calendarFactory.Events;
        }

        private void CreateNewEvent()
        {
            var item = new EventInfo {hillId = hills.GetData().First().name};
            calendarFactory.Events.Add(item);
            PresentList();
            view.SelectedEvent = item;
            PresentEventInfo();
        }

        private void DuplicateEvent()
        {
            var item = view.SelectedEvent;
            if (item == null) return;
            var duplicated = new EventInfo
            {
                hillId = item.hillId, classifications = item.classifications, eventType = item.eventType,
                inLimit = item.inLimit, roundInfos = item.roundInfos, inLimitType = item.inLimitType,
                ordRankId = item.inLimit, ordRankType = item.ordRankType, qualRankId = item.inLimit,
                qualRankType = item.ordRankType
            };
            calendarFactory.Events.Add(duplicated);
            PresentList();
            view.SelectedEvent = duplicated;
            PresentEventInfo();
        }

        private void RemoveEvent()
        {
            var item = view.SelectedEvent;
            if (item == null) return;

            var val = calendarFactory.Events.Remove(item);

            PresentList();
            view.SelectedEvent = null;
            PresentEventInfo();
        }

        private void PresentEventInfo()
        {
            var item = view.SelectedEvent;
            if (item == null)
            {
                view.EventInfoEnabled = false;
                return;
            }

            view.EventInfoEnabled = true;

            view.EventType = (int) item.eventType;
            view.QualRankType = (int) item.qualRankType;
            view.QualRankId = item.qualRankId;
            view.OrdRankType = (int) item.ordRankType;
            view.OrdRankId = item.ordRankId;
            view.InLimitType = (int) item.inLimitType;
            view.InLimit = item.inLimit;
            view.SelectedRoundsInfo = item.roundInfos;
            view.SelectedHill = hills.GetProfileData(item.hillId);
            view.SelectedClassifications = calendarFactory.GetClassificationDataFromIds(item.classifications);
        }

        private void SaveEventInfo()
        {
            var item = view.SelectedEvent;
            if (item == null) return;

            item.eventType = (EventType) view.EventType;
            item.qualRankType = (RankType) view.QualRankType;
            item.qualRankId = view.QualRankId;
            item.ordRankType = (RankType) view.OrdRankType;
            item.ordRankId = view.OrdRankId;
            item.inLimitType = (LimitType) view.InLimitType;
            item.inLimit = view.InLimit;
            item.roundInfos = view.SelectedRoundsInfo;
            item.hillId = view.SelectedHill.name;
            item.classifications = view.SelectedClassifications.Select(it => it.Id).ToList();
            PresentList();
            view.SelectedEvent = item;
        }

        private void InitEvents()
        {
            view.OnSelectionChanged += PresentEventInfo;
            view.OnDuplicate += DuplicateEvent;
            view.OnAdd += CreateNewEvent;
            view.OnRemove += RemoveEvent;
            view.OnMoveUp += () => MoveEvent(-1);
            view.OnMoveDown += () => MoveEvent(1);
            view.OnCurrentEventChanged += SaveEventInfo;
            view.OnDataReload += SetInitValues;
        }

        private void MoveEvent(int val)
        {
            var item = view.SelectedEvent;
            if (item == null) return;
            calendarFactory.MoveEvent(view.SelectedEvent, val);
            PresentList();
            view.SelectedEvent = item;
            PresentEventInfo();
        }


        private void SetInitValues()
        {
            PresentList();
            view.SelectedEvent = calendarFactory.Events.FirstOrDefault();
            view.RoundsInfos = presets.GetData();
            view.Hills = hills.GetData();
            view.Classifications = calendarFactory.GetClassificationData();
            PresentEventInfo();
        }
    }
}