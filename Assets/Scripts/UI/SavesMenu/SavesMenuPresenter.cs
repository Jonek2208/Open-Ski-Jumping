using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;

namespace OpenSkiJumping.UI.SavesMenu
{
    public class SavesMenuPresenter
    {
        private readonly ISavesMenuView view;
        private readonly SavesRuntime saves;
        private readonly CalendarsRuntime calendars;
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
            GameSave gameSave = new GameSave
            {
                name = name, resultsContainer = new ResultsDatabase(), calendar = calendar
            };

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
            view.SelectedSave = save;
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
                view.SaveInfoEnabled = false;
                return;
            }

            view.SaveInfoEnabled = true;
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
            view.OnSelectionChanged += PresentSaveInfo;
            view.OnAdd += OnAdd;
            view.OnRemove += RemoveSave;
            view.OnSubmit += OnSubmit;
        }

        private void SetInitValues()
        {
            PresentList();
            view.SelectedSave = saves.GetData().First();
            PresentSaveInfo();
            view.Calendars = calendars.GetData();
        }

    }
}
