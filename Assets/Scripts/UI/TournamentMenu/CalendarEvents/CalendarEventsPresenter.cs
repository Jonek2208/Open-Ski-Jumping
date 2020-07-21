using OpenSkiJumping.UI.TournamentMenu.CalendarEventInfo;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEvents
{
    public class CalendarEventsPresenter
    {
        private readonly ICalendarEventsView view;
        private readonly ICalendarEventInfoView eventInfoView;
        private readonly TournamentMenuData model;

        public CalendarEventsPresenter(ICalendarEventsView view, ICalendarEventInfoView eventInfoView,
            TournamentMenuData model)
        {
            this.view = view;
            this.eventInfoView = eventInfoView;
            this.model = model;

            InitEvents();
            SetInitValues();
        }

        private void PresentList()
        {
            view.Events = model.GetEvents();
        }

        // private void PresentEventInfo()
        // {
        //     var item = view.SelectedEvent;
        //     if (item == null)
        //     {
        //         view.EventInfoEnabled = false;
        //         return;
        //     }
        //
        //     view.EventInfoEnabled = true;
        //
        //     view.EventType = (int) item.eventType;
        //     view.OrdRankType = (int) item.ordRankType;
        //     view.OrdRankId = item.ordRankId;
        //     view.QualRankType = (int) item.qualRankType;
        //     view.QualRankId = item.qualRankId;
        //     view.InLimitType = (int) item.inLimitType;
        //     view.InLimit = item.inLimit;
        //     view.PreQualRankType = (int) item.preQualRankType;
        //     view.PreQualRankId = item.preQualRankId;
        //     view.PreQualLimitType = (int) item.preQualLimitType;
        //     view.PreQualLimit = item.preQualLimit;
        //     view.SelectedRoundsInfo = item.roundInfos;
        //     view.SelectedHill = hills.GetProfileData(item.hillId);
        //     view.SelectedClassifications = calendarFactory.GetClassificationDataFromIds(item.classifications);
        // }
        //
        // private void SaveEventInfo()
        // {
        //     var item = view.SelectedEvent;
        //     if (item == null) return;
        //
        //     item.eventType = (EventType) view.EventType;
        //     item.ordRankType = (RankType) view.OrdRankType;
        //     item.ordRankId = view.OrdRankId;
        //     item.qualRankType = (RankType) view.QualRankType;
        //     item.qualRankId = view.QualRankId;
        //     item.inLimitType = (LimitType) view.InLimitType;
        //     item.inLimit = view.InLimit;
        //     item.preQualRankType = (RankType) view.PreQualRankType;
        //     item.preQualRankId = view.PreQualRankId;
        //     item.preQualLimitType = (LimitType) view.PreQualLimitType;
        //     item.preQualLimit = view.PreQualLimit;
        //     item.roundInfos = view.SelectedRoundsInfo;
        //     item.hillId = view.SelectedHill.name;
        //     item.classifications = view.SelectedClassifications.Select(it => it.id).ToList();
        //     PresentList();
        //     view.SelectedEvent = item;
        // }

        private void InitEvents()
        {
            // view.OnSelectionChanged += () => eventInfoView.OnDataReload?.Invoke();
            // view.OnDuplicate += DuplicateEvent;
            // view.OnAdd += CreateNewEvent;
            // view.OnRemove += RemoveEvent;
            // view.OnMoveUp += () => MoveEvent(-1);
            // view.OnMoveDown += () => MoveEvent(1);
            // view.OnCurrentEventChanged += SaveEventInfo;
            view.OnDataReload += SetInitValues;
        }

        private void MoveEvent(int val)
        {
            var item = view.SelectedEvent;
            if (item == null) return;
            // calendarFactory.MoveEvent(view.SelectedEvent, val);
            PresentList();
            view.SelectedEvent = item;
            // PresentEventInfo();
        }


        private void SetInitValues()
        {
            PresentList();
            // view.SelectedEvent = calendarFactory.Events.FirstOrDefault();
            // view.RoundsInfos = presets.GetData();
            // view.Hills = hills.GetData();
            // view.Classifications = calendarFactory.GetClassificationData();
            // PresentEventInfo();
        }
    }
}