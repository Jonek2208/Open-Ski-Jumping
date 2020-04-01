using System.Collections.Generic;

namespace Competition
{
    public interface RoundResults
    {
        void Init(RoundInfo roundInfo, List<(int, decimal)> aggregateResults);
        void AddJump(JumpResult jump, int competitorId, int memberId = 0);
        int GetRank(int competitorId);
    }

    // public class IndividualNormalRoundResults : RoundResults
    // {
    //     private Dictionary<int, decimal> aggregateResults;
    //     private SortedList<(decimal, int), int> results;
    //     public void Init(List<(int, decimal)> aggregateResults)
    //     {

    //     }

    //     public void AddJump(JumpResult jump, int bib, int competitorId, int memberId = 0)
    //     {
    //         results.Add((jump.totalPoints, bib), competitorId)
    //     }

    //     public int GetRank(int competitorId)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    // }
}