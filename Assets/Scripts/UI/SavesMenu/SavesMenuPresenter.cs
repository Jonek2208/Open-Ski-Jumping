using Competition;

public class SavesMenuPresenter
{
    private ISavesMenuView view;
    private SavesRuntime saves;
    private CalendarsRuntime calendars;
    public SavesMenuPresenter(ISavesMenuView view, SavesRuntime saves, CalendarsRuntime calendars)
    {
        this.view = view;
        this.saves = saves;
        this.calendars = calendars;

        InitEvents();
        SetInitValues();
    }

    private GameSave CreateGameSaveFromCalendar(string name, Calendar calendar)
    {
        GameSave gameSave = new GameSave();
        gameSave.name = name;
        gameSave.resultsContainer = new ResultsDatabase();
        gameSave.calendar = calendar;

        gameSave.resultsContainer.eventResults = new EventResults[gameSave.calendar.events.Count];
        gameSave.resultsContainer.classificationResults = new ClassificationResults[gameSave.calendar.classifications.Count];

        for (int i = 0; i < gameSave.resultsContainer.classificationResults.Length; i++)
        {
            gameSave.resultsContainer.classificationResults[i] = new ClassificationResults();
        }

        return gameSave;
    }

    private void CreateNewSave()
    {
        GameSave save = CreateGameSaveFromCalendar(view.NewSaveName, view.SelectedCalendar);
        saves.Add(save);
        PresentList();
        view.SelectSave(save);
    }

    private void RemoveSave()
    {
        var save = view.SelectedSave;
        if (save == null) { return; }
        bool val = saves.Remove(save);

        PresentList();
        PresentSaveInfo();
    }

    private void PresentList()
    {
        view.Saves = saves.GetData();
    }

    private void PresentSaveInfo()
    {
        var save = view.SelectedSave;
        if (save == null)
        {
            view.HideSaveInfo();
            return;
        }
        
        view.ShowSaveInfo();
        view.CurrentSaveName = save.name;
        view.CurrentCalendarName = save.calendar.name;
    }

    private void OnAdd()
    {
        view.ShowPopUp();
    }

    private void OnSubmit()
    {
        if (view.NewSaveName.Length > 0)
        {
            CreateNewSave();
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
