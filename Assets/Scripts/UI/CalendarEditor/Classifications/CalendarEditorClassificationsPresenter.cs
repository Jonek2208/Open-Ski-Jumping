namespace UI.CalendarEditor.Classifications
{
    public class CalendarEditorClassificationsPresenter
    {
        private readonly ICalendarEditorClassificationsView view;
        private readonly CalendarFactory calendarFactory;

        public CalendarEditorClassificationsPresenter(ICalendarEditorClassificationsView view,
            CalendarFactory calendarFactory)
        {
            this.view = view;
            this.calendarFactory = calendarFactory;
        }
    }
}