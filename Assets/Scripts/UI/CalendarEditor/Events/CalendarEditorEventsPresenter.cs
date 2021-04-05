using System.Collections.Generic;
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
            var item = new EventInfo {hillId = hills.GetSortedData().First().name};
            calendarFactory.AddEvent(item);
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
                hillId = item.hillId, classifications = new List<int>(item.classifications), eventType = item.eventType,
                inLimit = item.inLimit, roundInfos = item.roundInfos, inLimitType = item.inLimitType,
                ordRankId = item.ordRankId, ordRankType = item.ordRankType, qualRankId = item.qualRankId,
                qualRankType = item.qualRankType, preQualRankType = item.preQualRankType,
                preQualRankId = item.preQualRankId, preQualLimitType = item.preQualLimitType,
                preQualLimit = item.preQualLimit,
                hillSurface = item.hillSurface
            };
            calendarFactory.AddEvent(duplicated);
            PresentList();
            view.SelectedEvent = duplicated;
            PresentEventInfo();
        }

        private void RemoveEvent()
        {
            var item = view.SelectedEvent;
            if (item == null) return;

            var val = calendarFactory.RemoveEvent(item);

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
            view.OrdRankType = (int) item.ordRankType;
            view.OrdRankId = item.ordRankId;
            view.QualRankType = (int) item.qualRankType;
            view.QualRankId = item.qualRankId;
            view.InLimitType = (int) item.inLimitType;
            view.InLimit = item.inLimit;
            view.PreQualRankType = (int) item.preQualRankType;
            view.PreQualRankId = item.preQualRankId;
            view.PreQualLimitType = (int) item.preQualLimitType;
            view.PreQualLimit = item.preQualLimit;
            view.SelectedRoundsInfo = item.roundInfos;
            view.HillSurface = (int) item.hillSurface;
            view.SelectedHill = hills.GetProfileData(item.hillId);
            view.SelectedClassifications = calendarFactory.GetClassificationDataFromIds(item.classifications);
        }

        private void SaveEventInfo()
        {
            var item = view.SelectedEvent;
            if (item == null) return;

            item.eventType = (EventType) view.EventType;
            item.ordRankType = (RankType) view.OrdRankType;
            item.ordRankId = view.OrdRankId;
            item.qualRankType = (RankType) view.QualRankType;
            item.qualRankId = view.QualRankId;
            item.inLimitType = (LimitType) view.InLimitType;
            item.inLimit = view.InLimit;
            item.preQualRankType = (RankType) view.PreQualRankType;
            item.preQualRankId = view.PreQualRankId;
            item.preQualLimitType = (LimitType) view.PreQualLimitType;
            item.preQualLimit = view.PreQualLimit;
            item.roundInfos = view.SelectedRoundsInfo;
            item.hillSurface = (HillSurface) view.HillSurface;
            item.hillId = view.SelectedHill.name;
            item.classifications = view.SelectedClassifications.Select(it => it.id).ToList();
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
            calendarFactory.RecalculateEvents();
            PresentList();
            view.SelectedEvent = calendarFactory.Events.FirstOrDefault();
            view.RoundsInfos = presets.GetData();
            view.Hills = hills.GetSortedData();
            view.Classifications = calendarFactory.GetClassificationData();
            PresentEventInfo();
        }
    }
}