using System.Linq;
using OpenSkiJumping.UI.TournamentMenu.CalendarEventInfo;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEvents
{
    public class CalendarEventsPresenter
    {
        private readonly ICalendarEventsView view;
        private readonly TournamentMenuData model;

        public CalendarEventsPresenter(ICalendarEventsView view, TournamentMenuData model)
        {
            this.view = view;
            this.model = model;

            InitEvents();
            SetInitValues();
        }

        private void PresentList()
        {
            view.Events = model.GetEvents();
        }

        private void HandleSelectionChanged()
        {
            model.SelectedEventCalendar = view.SelectedEvent;
            view.EventInfoView.DataReload();
        }


        private void InitEvents()
        {
            view.OnSelectionChanged += HandleSelectionChanged;
            view.OnDataReload += SetInitValues;
        }


        private void SetInitValues()
        {
            PresentList();
            view.SelectedEvent = model.GetEvents().FirstOrDefault();
            HandleSelectionChanged();
        }
    }
}