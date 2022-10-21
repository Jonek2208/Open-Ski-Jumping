using System;
using System.Collections.Generic;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class ResultsDatabase
    {
        public List<HillInfo> hillInfos;
        public int eventIndex;
        public int roundIndex;
        public EventResults[] eventResults;
        public ClassificationResults[] classificationResults;
    }
}