using System.Linq;
using Data;
using ScriptableObjects;

namespace UI.CalendarEditor.Competitors
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
            PresentList();
        }

        private void SetInitValues()
        {
            throw new System.NotImplementedException();
        }

        private void PresentList()
        {
            view.Jumpers = jumpers.GetData().OrderBy(item => item.countryCode);
        }

        private void Save()
        {
            calendarFactory.Competitors = view.SelectedJumpers;
        }

        private void InitEvents()
        {
            //     view.OnSelectionChanged += PresentJumperInfo;
            //     view.OnAdd += CreateNewJumper;
            //     view.OnRemove += RemoveJumper;
            //     view.OnCurrentJumperChanged += SaveJumperInfo;
        }
    }
}