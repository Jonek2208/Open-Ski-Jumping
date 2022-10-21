using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ScriptableObjects;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public class JumpersMenuPresenter
    {
        private readonly IJumpersMenuView view;
        private readonly CompetitorsRuntime jumpers;
        private FlagsData flagsData;

        public JumpersMenuPresenter(IJumpersMenuView view, CompetitorsRuntime jumpers, FlagsData flagsData)
        {
            this.view = view;
            this.jumpers = jumpers;
            this.flagsData = flagsData;

            InitEvents();
            SetInitValues();
        }

        private void CreateNewJumper()
        {
            var jumper = new Competitor();
            jumpers.Add(jumper);
            PresentList();
        }

        private void RemoveJumper(Competitor jumper)
        {
            if (jumper == null)
            {
                return;
            }

            jumpers.Remove(jumper);

            PresentList();
        }

        private void PresentList()
        {
            view.Jumpers = jumpers.GetData().OrderBy(item => item.countryCode);
        }

        private void SaveJumperInfo(Competitor jumper)
        {
            if (jumper == null)
            {
                return;
            }
            
            jumpers.Recalculate(jumper);
            PresentList();
        }

        private void InitEvents()
        {
            view.OnAdd += CreateNewJumper;
            view.OnRemove += RemoveJumper;
            view.OnCurrentJumperChanged += SaveJumperInfo;
        }

        private void SetInitValues()
        {
            PresentList();
        }
    }
}