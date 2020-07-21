namespace OpenSkiJumping.UI.TournamentMenu.JumpersSelection
{
    public class JumpersSelectionPresenter
    {
        private readonly IJumpersSelectionView view;
        private readonly TournamentMenuData model;

        public JumpersSelectionPresenter(IJumpersSelectionView view, TournamentMenuData model)
        {
            this.model = model;
            this.view = view;

            InitEvents();
            SetInitValues();
        }

        private void PresentList()
        {
            view.Jumpers = model.Competitors;
        }

        private void ChangeJumperState(CompetitorData item, bool value)
        {
            item.registered = value;
        }

        private void InitEvents()
        {
            view.OnChangeJumperState += ChangeJumperState;
            view.OnDataReload += SetInitValues;
        }

        private void SetInitValues()
        {
            PresentList();
        }
    }
}