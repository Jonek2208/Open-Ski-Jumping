using System;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;

namespace OpenSkiJumping.UI.HillsMenu
{
    public class HillsMenuPresenter
    {
        private readonly IHillsMenuView view;
        private readonly HillsRuntime model;

        public HillsMenuPresenter(IHillsMenuView view, HillsRuntime model)
        {
            this.view = view;
            this.model = model;

            InitEvents();
            SetInitValues();
        }

        private void Duplicate()
        {
            var item = view.SelectedHill;
            if (item == null) return;
            
            var duplicated = item.Clone();

            model.Add(duplicated);
            PresentList();
            view.SelectedHill = duplicated;
            PresentJumperInfo();
        }

        private void CreateNewHill()
        {
            var item = new ProfileData();
            model.Add(item);
            PresentList();
            view.SelectedHill = item;
            PresentJumperInfo();
        }

        private void RemoveHill()
        {
            var item = view.SelectedHill;
            if (item == null) return;

            var val = model.Remove(item);

            PresentList();
            view.SelectedHill = null;
            PresentJumperInfo();
        }

        private void PresentList()
        {
            view.Hills = model.GetSortedData();
        }

        private void PresentJumperInfo()
        {
            view.HillInfoView.SelectedHill = view.SelectedHill;
        }

        private void SaveItemInfo()
        {
            var item = view.SelectedHill;
            if (item == null) return;

            PresentList();
            view.SelectedHill = item;
        }

        private void InitEvents()
        {
            view.OnSelectionChanged += PresentJumperInfo;
            view.OnAdd += CreateNewHill;
            view.OnRemove += RemoveHill;
            view.OnDuplicate += Duplicate;
            view.HillInfoView.OnCurrentItemChanged += SaveItemInfo;
        }

        private void SetInitValues()
        {
            PresentList();
            view.SelectedHill = model.GetSortedData().FirstOrDefault();
            PresentJumperInfo();
        }
    }
}