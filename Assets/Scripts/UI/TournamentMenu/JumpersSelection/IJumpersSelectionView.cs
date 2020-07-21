using System;
using System.Collections.Generic;

namespace OpenSkiJumping.UI.TournamentMenu.JumpersSelection
{
    public interface IJumpersSelectionView
    {
        IEnumerable<CompetitorData> Jumpers { set; }
        
        event Action<CompetitorData, bool> OnChangeJumperState;
        event Action OnDataReload;
    }
}