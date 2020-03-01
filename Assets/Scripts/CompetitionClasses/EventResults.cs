using System;
using System.Collections.Generic;
namespace CompCal
{
    [Serializable]
    public class JumpResults
    {
        public List<JumpResult> results = new List<JumpResult>();
    }

    [Serializable]
    public class Result
    {
        public JumpResults[] results;
        public decimal qualRankPoints;
        public decimal[] totalResults;
        public int[] bibs;
        public int rank;
        public decimal totalPoints;
    }

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