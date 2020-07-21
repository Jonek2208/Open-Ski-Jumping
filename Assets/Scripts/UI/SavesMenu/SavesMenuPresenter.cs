using System.Linq;
using System.Linq.Expressions;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.UI.TournamentMenu;

namespace OpenSkiJumping.UI.SavesMenu
{
    public class SavesMenuPresenter
    {
        private readonly ISavesMenuView view;
        private readonly SavesRuntime saves;
        private readonly CalendarsRuntime calendars;
        private readonly CompetitorsRuntime competitorsRuntime;

        public SavesMenuPresenter(ISavesMenuView view, SavesRuntime saves, CalendarsRuntime calendars, CompetitorsRuntime competitorsRuntime)
        {
            this.view = view;
            this.saves = saves;
            this.calendars = calendars;
            this.competitorsRuntime = competitorsRuntime;

            InitEvents();
            SetInitValues();
        }



        private void CreateNewSave()
        {
            GameSave save = new GameSave(view.NewSaveName, view.SelectedCalendar, competitorsRuntime);
            saves.Add(save);
            PresentList();
            view.SelectedSave = save;
            PresentSaveInfo();
        }

        private void RemoveSave()
        {
            var save = view.SelectedSave;
            if (save == null)
            {
                return;
            }

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
            view.OnDataReload += SetInitValues;
        }

        private void SetInitValues()
        {
            PresentList();
            view.SelectedSave = saves.GetData().FirstOrDefault();
            PresentSaveInfo();
            view.Calendars = calendars.GetData();
        }
    }
}