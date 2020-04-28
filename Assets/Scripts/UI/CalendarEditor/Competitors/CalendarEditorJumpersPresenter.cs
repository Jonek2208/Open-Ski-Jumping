using System.Linq;
using OpenSkiJumping.Data;
using OpenSkiJumping.ScriptableObjects;

namespace OpenSkiJumping.UI.CalendarEditor.Competitors
{
    public class CalendarEditorJumpersPresenter
    {
        private readonly ICalendarEditorJumpersView view;
        private readonly CompetitorsRuntime jumpers;
        private readonly FlagsData flagsData;
        private readonly CalendarFactory calendarFactory;

        public CalendarEditorJumpersPresenter(ICalendarEditorJumpersView view, CompetitorsRuntime jumpers,
            FlagsData flagsData, CalendarFactory calendarFactory)
        {
            this.view = view;
            this.jumpers = jumpers;
            this.flagsData = flagsData;
            this.calendarFactory = calendarFactory;

            InitEvents();
            SelectJumpers();
            PresentList();
        }

        private void SelectJumpers()
        {
            view.SelectedJumpers = calendarFactory.Competitors;
        }

        private void PresentList()
        {
            view.Jumpers = jumpers.GetData().OrderBy(item => item.countryCode);
        }

        private void Save()
        {
            calendarFactory.Competitors = view.SelectedJumpers.ToList();
        }

        private void InitEvents()
        {
            view.OnDataSave += Save;
            view.OnDataReload += () =>
            {
                SelectJumpers();
                PresentList();
            };

            // view.OnSelectionChanged += PresentJumperInfo;
            // view.OnAdd += CreateNewJumper;
            // view.OnRemove += RemoveJumper;
            // view.OnCurrentJumperChanged += SaveJumperInfo;
        }
    }
}