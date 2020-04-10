using Competition;

public class CalendarsMenuPresenter
{
    private ICalendarsMenuView view;
    private CalendarsRuntime calendars;
    public CalendarsMenuPresenter(ICalendarsMenuView view, CalendarsRuntime calendars)
    {
        this.view = view;
        this.calendars = calendars;

        InitEvents();
        SetInitValues();
    }

    private void CreateNewCalendar()
    {
        Calendar calendar = new Calendar();
        calendar.name = view.NewCalendarName;
        calendars.Add(calendar);
        PresentList();
        view.SelectCalendar(calendar);
    }

    private void RemoveSave()
    {
        var save = view.SelectedCalendar;
        if (save == null) { return; }
        bool val = calendars.Remove(save);

        PresentList();
        PresentSaveInfo();
    }

    private void PresentList()
    {
        view.Calendars = calendars.GetData();
    }

    private void PresentSaveInfo()
    {
        var calendar = view.SelectedCalendar;
        if (calendar == null)
        {
            view.HideCalendarInfo();
            return;
        }

        view.ShowCalendarInfo();
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
        this.view.OnSelectionChanged += PresentSaveInfo;
        this.view.OnAdd += OnAdd;
        this.view.OnRemove += RemoveSave;
        this.view.OnSubmit += OnSubmit;
    }

    private void SetInitValues()
    {
        PresentList();
        PresentSaveInfo();
        view.Calendars = calendars.GetData();
    }

}
