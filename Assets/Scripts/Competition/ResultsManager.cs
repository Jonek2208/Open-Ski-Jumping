using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;

namespace OpenSkiJumping.Competition
{
    public interface IResultsManager
    {
        List<Participant> OrderedParticipants { get; }
        Result[] Results { get; }
        int[] LastRank { get; }
        List<int> StartList { get; }
        int StartListIndex { get; }
        int SubroundIndex { get; }
        int RoundIndex { get; }
        void RegisterJump(IJumpData jumpData);
        void SubroundInit();
        void RoundInit();
        bool SubroundFinish();
        bool RoundFinish();
        bool JumpFinish();
        Result GetResultByRank(int rank);
        int GetIdByRank(int rank);
        JumpResults GetResultById(int primaryId, int secondaryId);
        int GetCurrentCompetitorId();
        int GetCurrentCompetitorLocalId();
        int GetCurrentJumperId();
        int CompetitorRank(int id);
    }

    public class ResultsManager : IResultsManager
    {
        private readonly EventInfo eventInfo;
        private readonly IHillInfo hillInfo;

        private SortedList<(decimal points, int bib, int round), int> allRoundResults;
        private SortedList<(int state, decimal points, int bib), int> finalResults;
        private SortedList<(decimal points, int bib), int> losersResults;


        private int competitorsCount;
        private int[] koState;
        private int maxBib;
        private int maxLosers;
        private int roundsCount;
        private int subRoundsCount;

        public ResultsManager(EventInfo eventInfo, List<Participant> orderedParticipants, IHillInfo hillInfo)
        {
            this.eventInfo = eventInfo;
            OrderedParticipants = orderedParticipants;
            this.hillInfo = hillInfo;

            InitializeValues();
        }

        private void InitializeValues()
        {
            competitorsCount = OrderedParticipants.Count;
            Results = new Result[competitorsCount];
            LastRank = new int[competitorsCount];
            roundsCount = eventInfo.roundInfos.Count;
            subRoundsCount = eventInfo.eventType == EventType.Individual ? 1 : 4;

            for (var index = 0; index < Results.Length; index++)
            {
                Results[index] = new Result();
                var item = Results[index];
                item.TotalResults = new decimal[subRoundsCount];
                item.Results = new JumpResults[subRoundsCount];
                for (var i = 0; i < subRoundsCount; i++) item.Results[i] = new JumpResults();

                item.Bibs = new int[roundsCount];
            }

            finalResults =
                new SortedList<(int state, decimal points, int bib), int>(
                    Comparer<(int state, decimal points, int bib)>.Create(finalResultsComp));
            allRoundResults =
                new SortedList<(decimal points, int bib, int round), int>(
                    Comparer<(decimal points, int bib, int round)>.Create(allRoundResultsComp));
            losersResults =
                new SortedList<(decimal points, int bib), int>(
                    Comparer<(decimal points, int bib)>.Create(losersResultsComp));

            koState = new int[competitorsCount];
        }

        public int StartListIndex { get; private set; }
        public int SubroundIndex { get; private set; }
        public int RoundIndex { get; private set; }

        public List<int> StartList { get; private set; }
        public List<Participant> OrderedParticipants { get; }

        public Result[] Results { get; private set; }

        public int[] LastRank { get; private set; }

        public void SubroundInit()
        {
            var currentRoundInfo = eventInfo.roundInfos[RoundIndex];
            IEnumerable<int> tmp;

            //first sub-round
            if (RoundIndex == 0 && SubroundIndex == 0)
            {
                tmp = Enumerable.Range(0, competitorsCount).Reverse();
            }
            else
            {
                tmp = currentRoundInfo.useOrdRank[SubroundIndex]
                    ? finalResults.Select(item => item.Value).OrderBy(item => item).Reverse()
                    : finalResults.Select(item => item.Value).Reverse();
            }

            for (var i = 0; i < competitorsCount; i++) koState[i] = 0;

            var tmpList = tmp.ToList();
            finalResults.Clear();

            if (currentRoundInfo.roundType == RoundType.Normal)
            {
                StartList = tmpList;
                return;
            }

            StartList = Enumerable.Range(0, tmpList.Count).Select(it => tmpList[KOIndex(it, tmpList.Count)]).ToList();
            maxLosers = Math.Max(0, currentRoundInfo.outLimit - (StartList.Count + 1) / 2);
        }

        public void RoundInit()
        {
            var currentRoundInfo = eventInfo.roundInfos[RoundIndex];
            //first round
            if (RoundIndex == 0)
            {
                for (var i = 0; i < competitorsCount; i++)
                    Results[i].Bibs[RoundIndex] = currentRoundInfo.reversedBibs ? i + 1 : competitorsCount - i;
            }
            //reassign bibs
            else if (currentRoundInfo.reassignBibs)
            {
                for (var i = 0; i < finalResults.Count; i++)
                {
                    var it = finalResults.Values[i];
                    if (currentRoundInfo.reversedBibs)
                        Results[it].Bibs[RoundIndex] = i + 1;
                    else
                        Results[it].Bibs[RoundIndex] = finalResults.Count - i;
                }
            }
            //bibs from previous round
            else
            {
                for (var i = 0; i < finalResults.Count; i++)
                {
                    var id = finalResults.Values[i];
                    var lastRoundBib = Results[id].Bibs[RoundIndex - 1];
                    Results[id].Bibs[RoundIndex] = lastRoundBib;
                }
            }
        }

        public bool JumpFinish()
        {
            StartListIndex++;
            return StartListIndex < StartList.Count;
        }

        public Result GetResultByRank(int rank)
        {
            return Results[allRoundResults.Values[rank]];
        }

        public int GetIdByRank(int rank)
        {
            return finalResults.Values[rank];
        }

        public JumpResults GetResultById(int primaryId, int secondaryId)
        {
            return Results[primaryId].Results[secondaryId];
        }

        public int GetCurrentCompetitorId() => OrderedParticipants[StartList[StartListIndex]].id;
        public int GetCurrentCompetitorLocalId() => StartList[StartListIndex];
        public int GetCurrentJumperId() => OrderedParticipants[StartList[StartListIndex]].competitors[SubroundIndex];


        public bool SubroundFinish()
        {
            LastRank = Results.Select(item => item.Rank).ToArray();
            SubroundIndex++;
            StartListIndex = 0;
            return SubroundIndex < subRoundsCount;
        }

        public bool RoundFinish()
        {
            RoundIndex++;
            SubroundIndex = 0;
            return RoundIndex < roundsCount;
        }

        public void RegisterJump(IJumpData jumpData)
        {
            var jump = EventProcessor.GetJumpResult(jumpData, hillInfo);
            if (RoundIndex > 0 || SubroundIndex > 0) RemoveFromAllRoundResults();

            AddResult(StartList[StartListIndex], SubroundIndex, jump);
            AddToAllRoundResults();
            AddToFinalResults();
        }

        private void AddResult(int primaryIndex, int secondaryIndex, JumpResult jump)
        {
            Results[primaryIndex].Results[secondaryIndex].results.Add(jump);
            Results[primaryIndex].TotalResults[secondaryIndex] =
                Results[primaryIndex].Results[secondaryIndex].results.Sum(item => item.totalPoints);
            Results[primaryIndex].TotalPoints = Results[primaryIndex].TotalResults.Sum();
        }

        public int CompetitorRank(int id)
        {
            var key = (koState[id], Results[id].TotalPoints, -1);
            int lo = 0, hi = finalResults.Count;
            while (lo < hi)
            {
                var index = lo + (hi - lo) / 2;
                var el = finalResults.Keys[index];
                if (finalResults.Comparer.Compare(el, key) >= 0)
                    hi = index;
                else
                    lo = index + 1;
            }

            return hi + 1;
        }

        private void AddToFinalResults()
        {
            if (eventInfo.roundInfos[RoundIndex].roundType == RoundType.KO && StartListIndex % 2 == 1)
            {
                AddSecondKOJumper();
            }
            else
            {
                var id = StartList[StartListIndex];
                var bibCode = Results[id].Bibs[RoundIndex];
                finalResults.Add((0, Results[id].TotalPoints, bibCode), id);
            }
        }

        private void AddToAllRoundResults()
        {
            var competitorId = StartList[StartListIndex];
            var subroundNum = RoundIndex * subRoundsCount + SubroundIndex;
            var bibCode = GetBibCode(Results[competitorId].Bibs[RoundIndex]);
            allRoundResults.Add((Results[competitorId].TotalPoints, subroundNum, bibCode), competitorId);

            // Update rank
            for (var i = 0; i < Math.Min(competitorsCount, allRoundResults.Count); i++)
                if (i > 0 && allRoundResults.Keys[i].points == allRoundResults.Keys[i - 1].points)
                    Results[allRoundResults.Values[i]].Rank = Results[allRoundResults.Values[i - 1]].Rank;
                else
                    Results[allRoundResults.Values[i]].Rank = i + 1;
        }

        private void RemoveFromAllRoundResults()
        {
            var competitorId = StartList[StartListIndex];
            var subroundNum = RoundIndex * subRoundsCount + SubroundIndex - 1;
            var bibRoundIndex = SubroundIndex > 0 ? RoundIndex : RoundIndex - 1;
            var bibCode = GetBibCode(Results[competitorId].Bibs[bibRoundIndex]);
            allRoundResults.Remove((Results[competitorId].TotalPoints, subroundNum, bibCode));
        }

        private void AddSecondKOJumper()
        {
            var id1 = StartList[StartListIndex - 1];
            var id2 = StartList[StartListIndex];
            var bibCode1 = GetBibCode(Results[id1].Bibs[RoundIndex]);
            var bibCode2 = GetBibCode(Results[id2].Bibs[RoundIndex]);

            finalResults.Remove((0, id1, bibCode1));

            int loserId = id1, winnerId = id2;
            int loserBib = bibCode1, winnerBib = bibCode2;

            if (Results[id1].TotalPoints > Results[id2].TotalPoints)
            {
                winnerId = id1;
                winnerBib = bibCode1;
                loserId = id2;
                loserBib = bibCode2;
            }

            finalResults.Add((0, Results[winnerId].TotalPoints, winnerBib), winnerId);

            Results[loserId].Results[SubroundIndex].results[RoundIndex].state = JumpResultState.KoLoser;
            losersResults.Add((Results[loserId].TotalPoints, loserBib), loserId);
            var loserRank = losersResults.IndexOfKey((Results[loserId].TotalPoints, loserBib));

            //lost
            if (loserRank >= maxLosers)
            {
                koState[loserId] = 1;
                finalResults.Add((1, Results[loserId].TotalPoints, loserBib), loserId);
            }
            //lucky loser
            else
            {
                //remove last lucky loser
                if (losersResults.Count > maxLosers)
                {
                    var (lastLoserPoints, lastLoserBib) = losersResults.Keys[maxLosers];
                    var lastLoserId = losersResults.Values[maxLosers];
                    koState[lastLoserId] = 1;
                    Results[loserId].Results[SubroundIndex].results[RoundIndex].state = JumpResultState.KoLoser;

                    finalResults.Remove((0, lastLoserPoints, lastLoserBib));
                    finalResults.Add((1, lastLoserPoints, lastLoserBib), lastLoserId);
                }

                finalResults.Add((0, Results[loserId].TotalPoints, loserBib), loserId);
            }
        }

        private readonly Comparison<(int state, decimal points, int bib)> finalResultsComp = (x, y) =>
            x.state == y.state
                ? x.points == y.points
                    ? x.bib.CompareTo(y.bib)
                    : y.points.CompareTo(x.points)
                : x.state.CompareTo(y.state);

        private readonly Comparison<(decimal points, int bib, int round)> allRoundResultsComp = (x, y) =>
            x.points == y.points
                ? x.bib == y.bib
                    ? y.round.CompareTo(x.round)
                    : x.bib.CompareTo(y.bib)
                : y.points.CompareTo(x.points);

        private readonly Comparison<(decimal points, int bib)> losersResultsComp = (x, y) =>
            x.points == y.points
                ? x.bib.CompareTo(y.bib)
                : y.points.CompareTo(x.points);

        private int GetBibCode(int bib) =>
            eventInfo.roundInfos[RoundIndex].reversedBibs ? bib : competitorsCount - bib;

        private static int KOIndex(int index, int length)
        {
            var halfLen = length / 2;
            var halfIndex = index / 2;

            if (index == 2 * halfLen) return index;
            if (index % 2 == 0)
                return halfLen - 1 - halfIndex;
            return halfLen + halfIndex;
        }
    }
}
//
// using System;
// using System.Collections.Generic;
//
// namespace CompCal
// {
//     [Serializable]
//     public class Result
//     {
//         public List<JumpResult> rounds = new List<JumpResult>();
//         public decimal total;
//         public int rank;
//         public int lastRank;
//
//         public void AddResult(JumpResult jmp)
//         {
//             rounds.Add(jmp);
//             total += rounds[rounds.Count - 1].totalPoints;
//         }
//     }
//
//     [Serializable]
//     public class SortedResults
//     {
//         public SortedList<Tuple<int, decimal, int>, int> finalResults;
//         public SortedList<Tuple<decimal, int, int>, int> allroundResults;
//         public int[] rank;
//         int round;
//
//         public SortedResults(int comprtitorsCount)
//         {
//             rank = new int[comprtitorsCount];
//             Comparison<Tuple<int, decimal, int>> comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? x.Item3.CompareTo(y.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1));
//             finalResults = new SortedList<Tuple<int, decimal, int>, int>(Comparer<Tuple<int, decimal, int>>.Create(comp));
//             Comparison<Tuple<decimal, int, int>> comp2 = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? y.Item3.CompareTo(x.Item3) : x.Item2.CompareTo(y.Item2)) : y.Item1.CompareTo(x.Item1));
//             allroundResults = new SortedList<Tuple<decimal, int, int>, int>(Comparer<Tuple<decimal, int, int>>.Create(comp2));
//         }
//         public void AddResult(int competitorId, Result res, JumpResult jmp, int bib, int lastBib = 0)
//         {
//             if (lastBib > 0)
//             {
//                 allroundResults.Remove(Tuple.Create(res.total, round - 1, lastBib));
//             }
//
//             res.AddResult(jmp);
//
//             allroundResults.Add(Tuple.Create(res.total, round, bib), competitorId);
//
//             for (int i = 0; i < allroundResults.Count; i++)
//             {
//                 if (i > 0 && allroundResults.Keys[i].Item1 == allroundResults.Keys[i - 1].Item1)
//                 { rank[allroundResults.Values[i]] = rank[allroundResults.Values[i - 1]]; }
//                 else
//                 { rank[allroundResults.Values[i]] = i + 1; }
//             }
//         }
//     }
//
//     [Serializable]
//     public class EventResultsInd
//     {
//         public int eventId;
//         public List<int> competitorsList;
//         public List<int>[] startLists;
//         public Result[] results;
//         public List<int>[] bibs;
//         public SortedResults sortedResults;
//         public int[] lastRank;
//
//         public EventResultsInd(List<int> competitorsList, int roundsCount)
//         {
//             this.competitorsList = competitorsList;
//             this.results = new Result[this.competitorsList.Count];
//             this.bibs = new List<int>[this.competitorsList.Count];
//             this.sortedResults = new SortedResults(competitorsList.Count);
//             this.startLists = new List<int>[roundsCount];
//             for (int i = 0; i < this.competitorsList.Count; i++) { bibs[i] = new List<int>(); }
//         }
//     }
//
//     [Serializable]
//     public class EventResultsTeam
//     {
//         public int eventId;
//         public List<Team> teamsList;
//         public List<int>[] startLists;
//         public Result[] results;
//         public List<int>[] bibs;
//         public SortedResults sortedResults;
//         public int[] lastRank;
//
//         public EventResultsTeam(List<Team> teamsList, int roundsCount)
//         {
//             this.sortedResults = new SortedResults(this.teamsList.Count);
//             this.teamsList = teamsList;
//             this.results = new Result[this.teamsList.Count];
//             this.bibs = new List<int>[this.teamsList.Count];
//             this.lastRank = new int[this.teamsList.Count];
//             this.startLists = new List<int>[roundsCount];
//             for (int i = 0; i < this.teamsList.Count; i++) { bibs[i] = new List<int>(); }
//         }
//     }
//
//     [Serializable]
//     public class EventResults
//     {
//         public List<JumpResult>[] roundResults;
//         public decimal[] totalResults;
//         public SortedList<Tuple<int, decimal, int>, int> finalResults;
//         public SortedList<Tuple<decimal, int, int>, int> allroundResults;
//         public SortedList<Tuple<decimal, int>, int> losersResults;
//         public List<int> competitorsList;
//         public List<int>[] startLists;
//         public int[] koState;
//         public List<int>[] bibs;
//         public int[] rank;
//         public int[] lastRank;
//         public int maxLosers;
//
//         public EventResults(List<int> _competitorsList, int roundsCount)
//         {
//             competitorsList = _competitorsList;
//             Comparison<Tuple<int, decimal, int>> comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? x.Item3.CompareTo(y.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1));
//             Comparer<Tuple<int, decimal, int>> comparer = Comparer<Tuple<int, decimal, int>>.Create(comp);
//             finalResults = new SortedList<Tuple<int, decimal, int>, int>(comparer);
//             roundResults = new List<JumpResult>[competitorsList.Count];
//             totalResults = new decimal[competitorsList.Count];
//             rank = new int[competitorsList.Count];
//             bibs = new List<int>[competitorsList.Count];
//             lastRank = new int[competitorsList.Count];
//             Comparison<Tuple<decimal, int, int>> comp2 = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? y.Item3.CompareTo(x.Item3) : x.Item2.CompareTo(y.Item2)) : y.Item1.CompareTo(x.Item1));
//             allroundResults = new SortedList<Tuple<decimal, int, int>, int>(Comparer<Tuple<decimal, int, int>>.Create(comp2));
//             losersResults = new SortedList<Tuple<decimal, int>, int>();
//             startLists = new List<int>[roundsCount];
//             for (int i = 0; i < competitorsList.Count; i++)
//             {
//                 roundResults[i] = new List<JumpResult>();
//                 bibs[i] = new List<int>();
//             }
//         }
//
//         public void AddResult(int competitorId, JumpResult jmp)
//         {
//             int round = roundResults[competitorId].Count;
//             if (round > 0)
//             {
//                 allroundResults.Remove(Tuple.Create(totalResults[competitorId], round - 1, bibs[competitorId][round - 1]));
//             }
//
//             roundResults[competitorId].Add(jmp);
//             totalResults[competitorId] += jmp.totalPoints;
//
//             allroundResults.Add(Tuple.Create(totalResults[competitorId], round, bibs[competitorId][round]), competitorId);
//
//             // Update rank
//             for (int i = 0; i < Math.Min(competitorsList.Count, allroundResults.Count); i++)
//             {
//                 if (i > 0 && allroundResults.Keys[i].Item1 == allroundResults.Keys[i - 1].Item1)
//                 {
//                     rank[allroundResults.Values[i]] = rank[allroundResults.Values[i - 1]];
//                 }
//                 else
//                 {
//                     rank[allroundResults.Values[i]] = i + 1;
//                 }
//             }
//
//         }
//
//         public int CompetitorRankKo(int competitorId)
//         {
//             var key = Tuple.Create(koState[competitorId], totalResults[competitorId], -1);
//             int lo = 0, hi = finalResults.Count;
//             while (lo < hi)
//             {
//                 int index = lo + (hi - lo) / 2;
//                 var el = finalResults.Keys[index];
//                 if (finalResults.Comparer.Compare(el, key) >= 0) { hi = index; }
//                 else { lo = index + 1; }
//             }
//             return hi + 1;
//         }
//         public void AddNormalResult(int competitorId)
//         {
//             finalResults.Add(Tuple.Create(0, totalResults[competitorId], bibs[competitorId][bibs[competitorId].Count - 1]), competitorId);
//         }
//         public void AddKoResult(int competitorId)
//         {
//             finalResults.Add(Tuple.Create(0, totalResults[competitorId], bibs[competitorId][bibs[competitorId].Count - 1]), competitorId);
//         }
//         public void AddKoResult(int competitorId1, int competitorId2)
//         {
//             finalResults.Remove(Tuple.Create(0, totalResults[competitorId1], bibs[competitorId1][bibs[competitorId1].Count - 1]));
//             int loserId = competitorId1, winnerId = competitorId2;
//             if (totalResults[competitorId1] > totalResults[competitorId2])
//             {
//                 winnerId = competitorId1;
//                 loserId = competitorId2;
//             }
//
//             int loserBib = bibs[loserId][bibs[loserId].Count - 1];
//             int winnerBib = bibs[winnerId][bibs[winnerId].Count - 1];
//
//             finalResults.Add(Tuple.Create(0, totalResults[winnerId], winnerBib), winnerId);
//
//             losersResults.Add(Tuple.Create(totalResults[loserId], loserBib), loserId);
//             int loserRank = losersResults.IndexOfKey(Tuple.Create(totalResults[loserId], loserBib));
//             if (loserRank < maxLosers)
//             {
//                 if (losersResults.Count > maxLosers)
//                 {
//                     var lastLoser = losersResults.Keys[maxLosers];
//                     int lastLoserId = losersResults.Values[maxLosers];
//                     koState[lastLoserId] = 1;
//                     finalResults.Remove(Tuple.Create(0, lastLoser.Item1, lastLoser.Item2));
//                     finalResults.Add(Tuple.Create(1, lastLoser.Item1, lastLoser.Item2), lastLoserId);
//                 }
//                 finalResults.Add(Tuple.Create(0, totalResults[loserId], loserBib), loserId);
//             }
//             else
//             {
//                 koState[loserId] = 1;
//                 finalResults.Add(Tuple.Create(1, totalResults[loserId], loserBib), loserId);
//             }
//         }
//     }
//
// }