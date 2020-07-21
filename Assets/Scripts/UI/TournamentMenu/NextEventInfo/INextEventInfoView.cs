using System;
using OpenSkiJumping.Competition;

namespace OpenSkiJumping.UI.TournamentMenu.NextEventInfo
{
    public interface INextEventInfoView
    {
        string Hill { set; }
        EventType EventType { set; }
        event Action OnDataReload;
    }
}