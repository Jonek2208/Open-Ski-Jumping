namespace OpenSkiJumping.UI.TournamentMenu.TeamsSelection
{
    public class TeamsSelectionPresenter
    {
        private readonly ITeamsSelectionView view;
        private readonly TournamentMenuData model;
        private readonly ITournamentMenuController menuController;

        public TeamsSelectionPresenter(ITeamsSelectionView view, TournamentMenuData model, ITournamentMenuController menuController)
        {
            this.view = view;
            this.model = model;
            this.menuController = menuController;
            InitEvents();
            SetInitValues();
        }

        private void PresentList()
        {
            view.Teams = model.Teams;
        }

        private void EditTeam(TeamData item)
        {
            model.EditedTeam = item;
            menuController.ShowTeamSquad();
        }

        private void ChangeTeamState(TeamData item, bool value)
        {
            item.registered = value;
        }

        private void InitEvents()
        {
            view.OnChangeTeamState += ChangeTeamState;
            view.OnEditRequest += EditTeam;
            view.OnDataReload += SetInitValues;
            menuController.OnReloadTeamsList += view.RefreshTeams;
        }

        private void SetInitValues()
        {
            PresentList();
        }
    }
}