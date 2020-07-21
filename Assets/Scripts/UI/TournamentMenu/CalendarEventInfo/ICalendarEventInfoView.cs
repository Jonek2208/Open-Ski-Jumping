using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEventInfo
{
    public interface ICalendarEventInfoView
    {
        EventRoundsInfo RoundsInfo { set; }
        IEnumerable<ClassificationInfo> Classifications { set; }
        string Hill { set; }

        EventType EventType { set; }
        string OrdRank { set; }
        string QualRank { set; }
        string QualLimit { set; }
        string PreQualRank { set; }
        string PreQualLimit { set; }
        
        bool EventInfoEnabled { set; }
        
        event Action OnDataReload;
    }
}