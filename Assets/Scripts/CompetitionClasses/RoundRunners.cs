using System;
using System.Collections.Generic;

namespace CompCal
{
    public abstract class RoundRunner
    {
        protected EventResultsInd eventResults;
        protected RoundInfo roundInfo;
        protected int jumpIt;
        protected List<int> startList;
        protected int[] bibs;
        protected abstract void MakeBibs(bool reassignBibs);
        public abstract void Init();

        public abstract int NextJump();
    }

    public class RoundRunnerNormal
    {

    }

    public class RoundRunnerKO : RoundRunner
    {
        private SortedList<Tuple<decimal, int>, int> losersResults;
        private int[] koState;
        private int maxLosers;

        public override void Init() { }

        private void MakeStartList()
        {
            this.startList = new List<int>();
            int len = this.eventResults.sortedResults.finalResults.Count;

            for (int i = 0; i < len / 2; i++)
            {
                this.startList.Add(this.eventResults.sortedResults.finalResults.Values[(len + 1) / 2 + i]);
                this.startList.Add(this.eventResults.sortedResults.finalResults.Values[(len + 1) / 2 - i - 1]);
            }
            if (len % 2 == 1)
            {
                this.startList.Add(this.eventResults.sortedResults.finalResults.Values[0]);
            }
        }

        public override int NextJump()
        {
            if (jumpIt < startList.Count)
                return startList[jumpIt++];
            else
            {
                return -1;
            }
        }

        protected override void MakeBibs(bool reassignBibs)
        {
            // for (int i = 0; i < len; i++)
            // {
            //     int it = this.eventResults.sortedResults.finalResults.Values[i];
            //     if (reassignBibs)
            //     {
            //         if (roundInfo.reversedBibs)
            //         { this.bibs[it] = i + 1; }
            //         else
            //         { this.bibs[it] = len - i; }
            //     }
            //     else
            //     {
            //         int lastRoundBib = CurrentEventResults.bibs[it][roundIt - 1];
            //         CurrentEventResults.bibs[it].Add(lastRoundBib);
            //     }
            // }

            // Comparison<Tuple<int, decimal, int>> comp;
            // if (CurrentRoundInfo.reversedBibs)
            // { comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? x.Item3.CompareTo(y.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1)); }
            // else
            // { comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? y.Item3.CompareTo(x.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1)); }
            // Comparer<Tuple<int, decimal, int>> comparer = Comparer<Tuple<int, decimal, int>>.Create(comp);
            // CurrentEventResults.finalResults = new SortedList<Tuple<int, decimal, int>, int>(comparer);
        }

        public void RoundInit()
        {
            MakeStartList();
            this.koState = new int[this.eventResults.competitorsList.Count];
            this.maxLosers = Math.Max(0, roundInfo.outLimit - (this.startList.Count + 1) / 2);
            Comparison<Tuple<decimal, int>> compLoser = (x, y) => (x.Item1 == y.Item1 ? x.Item2.CompareTo(y.Item2) : y.Item1.CompareTo(x.Item1));
            this.losersResults = new SortedList<Tuple<decimal, int>, int>(Comparer<Tuple<decimal, int>>.Create(compLoser));
            // MakeBibs();
        }

        public int CompetitorRankKo(int competitorId)
        {
            var key = Tuple.Create(this.koState[competitorId], this.eventResults.results[competitorId].total, -1);
            int lo = 0, hi = this.eventResults.sortedResults.finalResults.Count;
            while (lo < hi)
            {
                int index = lo + (hi - lo) / 2;
                var el = this.eventResults.sortedResults.finalResults.Keys[index];
                if (this.eventResults.sortedResults.finalResults.Comparer.Compare(el, key) >= 0) { hi = index; }
                else { lo = index + 1; }
            }
            return hi + 1;
        }

        public void AddResult(JumpResult jmp)
        {
            int competitorId = this.startList[jumpIt];
            this.eventResults.results[competitorId].AddResult(jmp);
            this.jumpIt++;
        }

        public void RemoveResult(int competitorId)
        {
            var key = Tuple.Create(koState[competitorId], this.eventResults.results[competitorId].total, this.bibs[competitorId]);
            this.eventResults.sortedResults.finalResults.Remove(key);
        }

        public void AddResult(int competitorId)
        {
            var key = Tuple.Create(koState[competitorId], this.eventResults.results[competitorId].total, bibs[competitorId]);
            this.eventResults.sortedResults.finalResults.Add(key, competitorId);
        }

        public void AddLoserResult(int competitorId)
        {
            var loserKey = Tuple.Create(this.eventResults.results[competitorId].total, this.bibs[competitorId]);
            this.losersResults.Add(loserKey, competitorId);
            int loserRank = this.losersResults.IndexOfKey(loserKey);

            if ((loserRank < this.maxLosers) && (this.maxLosers < this.losersResults.Count))
            {
                int lastLoserId = this.losersResults.Values[maxLosers];

                RemoveResult(lastLoserId);
                this.koState[lastLoserId] = 1;
                AddResult(lastLoserId);
            }
            else
            {
                this.koState[competitorId] = 1;
            }

            var key = Tuple.Create(this.koState[competitorId], this.eventResults.results[competitorId].total, this.bibs[competitorId]);
            this.eventResults.sortedResults.finalResults.Add(key, competitorId);
        }


        public void AddResult(int competitorId1, int competitorId2)
        {
            RemoveResult(competitorId1);

            int loserId = competitorId1, winnerId = competitorId2;
            if (this.eventResults.results[competitorId1].total > this.eventResults.results[competitorId2].total)
            {
                winnerId = competitorId1;
                loserId = competitorId2;
            }

            AddResult(winnerId);
            AddLoserResult(loserId);
        }

    }

    public class RoundRunnerTeam
    {

    }
}
