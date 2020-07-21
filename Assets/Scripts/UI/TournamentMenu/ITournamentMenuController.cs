using System;

namespace OpenSkiJumping.UI.TournamentMenu
{
    public interface ITournamentMenuController
    {
        event Action OnReloadTeamsList;
        void ShowTeamSquad();
        void HideTeamSquad();
    }
}