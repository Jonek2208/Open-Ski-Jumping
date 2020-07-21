using System.Linq;
using OpenSkiJumping.Competition;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEventInfo
{
    public class CalendarEventInfoPresenter
    {
        private readonly ICalendarEventInfoView view;
        private readonly TournamentMenuData model;

        public CalendarEventInfoPresenter(ICalendarEventInfoView view, TournamentMenuData model)
        {
            this.view = view;
            this.model = model;

            InitEvents();
        }

        private string GetInLimit(LimitType limitType, int value)
        {
            if (limitType == LimitType.Normal) return $"Normal: {value}";
            if (limitType == LimitType.Exact) return $"Exact: {value}";
            return "-";
        }

        private void ShowInfo()
        {
            var item = model.SelectedEventCalendar;
            if (item == null) return;
            view.RoundsInfo = item.roundInfos;
            view.Classifications = item.classifications.Select(it => model.Calendar.classifications[it]);
            view.Hill = item.hillId;

            view.EventType = item.eventType;
            view.OrdRank = model.GetRank(item.ordRankType, item.ordRankId);
            view.QualRank = model.GetRank(item.qualRankType, item.qualRankId);
            view.QualLimit = GetInLimit(item.inLimitType, item.inLimit);
            view.QualLimit = GetInLimit(item.inLimitType, item.inLimit);
            view.PreQualRank = model.GetRank(item.preQualRankType, item.preQualRankId);
            view.PreQualLimit = GetInLimit(item.preQualLimitType, item.preQualLimit);
        }

        private void InitEvents()
        {
            view.OnDataReload += ShowInfo;
        }
    }
}