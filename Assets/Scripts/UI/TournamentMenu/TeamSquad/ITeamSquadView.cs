using System;
using System.Collections.Generic;

namespace OpenSkiJumping.UI.TournamentMenu.TeamSquad
{
    public interface ITeamSquadView
    {
        CompetitorData SelectedCompetitorData { get; set; }
        IEnumerable<CompetitorData> Competitors { set; }

        event Action OnSelectionChanged;
        event Action OnSubmit;
        event Action OnMoveUp;
        event Action OnMoveDown;
        event Action OnDataReload;
    }
}