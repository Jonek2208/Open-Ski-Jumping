using System;
using System.Collections.Generic;
namespace Competition
{
    [Serializable]
    public class EventResults
    {
        public List<Participant> participants;
        public List<int> competitorIds;
        public List<Result> results;
        public List<int> finalResults;
        public List<int> allroundResults;
        public List<int>[] startLists;
    }
}