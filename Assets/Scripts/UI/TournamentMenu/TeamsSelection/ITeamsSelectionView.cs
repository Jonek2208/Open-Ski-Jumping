using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.UI.TournamentMenu.TeamsSelection
{
    public interface ITeamsSelectionView
    {
        IEnumerable<TeamData> Teams { set; }
        void RefreshTeams();
        event Action<TeamData, bool> OnChangeTeamState;
        event Action OnDataReload;
        event Action<TeamData> OnEditRequest;
    }
}