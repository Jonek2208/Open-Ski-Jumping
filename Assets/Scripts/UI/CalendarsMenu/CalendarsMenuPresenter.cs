using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;

namespace OpenSkiJumping.UI.CalendarsMenu
{
    public class CalendarsMenuPresenter
    {
        private readonly ICalendarsMenuView view;
        private readonly CalendarsRuntime calendars;
        public CalendarsMenuPresenter(ICalendarsMenuView view, CalendarsRuntime calendars)
        {
            this.view = view;
            this.calendars = calendars;

            InitEvents();
            SetInitValues();
        }

        private void CreateNewCalendar()
        {
            var calendar = new Calendar {name = view.NewCalendarName};
            calendars.Add(calendar);
            PresentList();
            view.SelectedCalendar = calendar;
        }

        private void RemoveSave()
        {
            var save = view.SelectedCalendar;
            if (save == null) { return; }
            bool val = calendars.Remove(save);

            PresentList();
            PresentCalendarInfo();
        }

        private void PresentList()
        {
            view.Calendars = calendars.GetData();
        }

        private void PresentCalendarInfo()
        {
            var calendar = view.SelectedCalendar;
            if (calendar == null)
            {
                view.CalendarInfoEnabled = false;
                return;
            }

            view.CalendarInfoEnabled = true;
            view.CurrentCalendarName = calendar.name;
        }

        private void OnAdd()
        {
            view.ShowPopUp();
        }

        private void OnSubmit()
        {
            if (view.NewCalendarName.Length > 0)
            {
                CreateNewCalendar(); 
                view.HidePopUp();
            }
            else
            {
                view.ShowPrompt();
            }
        }

        private void InitEvents()
        {
            view.OnSelectionChanged += PresentCalendarInfo;
            view.OnAdd += OnAdd;
            view.OnRemove += RemoveSave;
            view.OnSubmit += OnSubmit;
        }

        private void SetInitValues()
        {
            PresentList();
            view.SelectedCalendar = calendars.GetData().First();
            PresentCalendarInfo();
        }

    }
}
