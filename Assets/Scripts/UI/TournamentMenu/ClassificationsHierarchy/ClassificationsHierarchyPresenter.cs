using System.Linq;

namespace OpenSkiJumping.UI.TournamentMenu.ClassificationsHierarchy
{
    public class ClassificationsHierarchyPresenter
    {
        private readonly IClassificationsHierarchyView view;
        private readonly TournamentMenuData model;

        public ClassificationsHierarchyPresenter(IClassificationsHierarchyView view, TournamentMenuData model)
        {
            this.view = view;
            this.model = model;

            InitEvents();
            SetInitValues();
        }

        private void PresentList()
        {
            view.Classifications = model.Classifications;
        }

        private void ChangeBibState(ClassificationData item, bool value)
        {
            item.useBib = value;
        }

        private void ChangePriority(int targetValue)
        {
            var item = view.SelectedClassification;
            if (item == null) return;
            model.ChangeClassificationPriority(item, item.priority + targetValue);
            PresentList();
            view.SelectedClassification = item;
        }

        private void InitEvents()
        {
            view.OnChangeBibState += ChangeBibState;
            view.OnMoveUp += () => ChangePriority(-1);
            view.OnMoveDown += () => ChangePriority(1);
            view.OnDataReload += SetInitValues;
        }

        private void SetInitValues()
        {
            PresentList();
            view.SelectedClassification = model.Classifications.FirstOrDefault();
        }
    }
}