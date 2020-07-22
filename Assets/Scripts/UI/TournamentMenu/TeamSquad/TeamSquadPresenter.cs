using System.Linq;

namespace OpenSkiJumping.UI.TournamentMenu.TeamSquad
{
    public class TeamSquadPresenter
    {
        private readonly ITeamSquadView view;
        private readonly TournamentMenuData model;
        private readonly ITournamentMenuController menuController;

        public TeamSquadPresenter(ITeamSquadView view, TournamentMenuData model,
            ITournamentMenuController menuController)
        {
            this.view = view;
            this.model = model;
            this.menuController = menuController;

            InitEvents();
            SetInitValues();
        }

        private void PresentList()
        {
            view.Competitors = model.EditedTeam.competitors;
        }

        private void ChangePriority(int targetValue)
        {
            var item = view.SelectedCompetitorData;
            if (item == null) return;
            
            model.ChangeCompetitorPriority(item, item.teamId + targetValue);
            PresentList();
            view.SelectedCompetitorData = item;
        }

        private void OnSubmit()
        {
            menuController.HideTeamSquad();
        }

        private void InitEvents()
        {
            view.OnSubmit += OnSubmit;
            view.OnMoveUp += () => ChangePriority(-1);
            view.OnMoveDown += () => ChangePriority(1);
            view.OnDataReload += SetInitValues;
        }

        private void SetInitValues()
        {
            PresentList();
            view.SelectedCompetitorData = model.Competitors.FirstOrDefault();
        }
    }
}