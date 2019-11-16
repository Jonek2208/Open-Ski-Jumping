﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CompCal
{
    [Serializable]
    public class JumpResult
    {
        public decimal totalPoints;
        public decimal distance;
        public decimal distancePoints;
        public decimal[] judgesMarks;
        public bool[] judgesMask;
        public decimal judgesTotalPoints;
        public int gate;
        public decimal gatePoints;
        public decimal wind;
        public decimal windPoints;
        public decimal speed;
        public JumpResult()
        {
            judgesMask = new bool[5];
        }

        public JumpResult(decimal _distance, decimal[] _judgesMarks, int _gate, decimal _wind, decimal _speed)
        {
            // competitorId = _competitorId;
            distance = _distance;
            judgesMarks = _judgesMarks;
            judgesMask = new bool[judgesMarks.Length];
            gate = _gate;
            wind = _wind;
            speed = _speed;
            CalculateJudgesMarks();
        }

        public void CalculateJudgesMarks()
        {
            // If all values are equal then there could be a problem, but then mn will stay 0 and mx will stay 1
            int mn = 0, mx = 1;
            for (int i = 0; i < judgesMarks.Length; i++)
            {
                if (judgesMarks[mn] < judgesMarks[i]) mn = i;
                if (judgesMarks[mx] > judgesMarks[i]) mx = i;
                judgesTotalPoints += judgesMarks[i];
                judgesMask[i] = true;
            }

            judgesMask[mn] = judgesMask[mx] = false;
            judgesTotalPoints -= judgesMarks[mn] + judgesMarks[mx];
        }
    }

    [Serializable]
    public class EventResults
    {
        public List<JumpResult>[] roundResults;
        public decimal[] totalResults;
        public SortedList<Tuple<int, decimal, int>, int> finalResults;
        public SortedList<Tuple<decimal, int, int>, int> allroundResults;
        public SortedList<Tuple<decimal, int>, int> losersResults;
        public List<int> competitorsList;
        public List<int>[] startLists;
        public int[] koState;
        public List<int>[] bibs;
        public int[] rank;
        public int[] lastRank;
        public int maxLosers;

        public EventResults(List<int> _competitorsList, int roundsCount)
        {
            competitorsList = _competitorsList;
            Comparison<Tuple<int, decimal, int>> comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? x.Item3.CompareTo(y.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1));
            Comparer<Tuple<int, decimal, int>> comparer = Comparer<Tuple<int, decimal, int>>.Create(comp);
            finalResults = new SortedList<Tuple<int, decimal, int>, int>(comparer);
            roundResults = new List<JumpResult>[competitorsList.Count];
            totalResults = new decimal[competitorsList.Count];
            rank = new int[competitorsList.Count];
            bibs = new List<int>[competitorsList.Count];
            lastRank = new int[competitorsList.Count];
            Comparison<Tuple<decimal, int, int>> comp2 = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? y.Item3.CompareTo(x.Item3) : x.Item2.CompareTo(y.Item2)) : y.Item1.CompareTo(x.Item1));
            allroundResults = new SortedList<Tuple<decimal, int, int>, int>(Comparer<Tuple<decimal, int, int>>.Create(comp2));
            losersResults = new SortedList<Tuple<decimal, int>, int>();
            startLists = new List<int>[roundsCount];
            for (int i = 0; i < competitorsList.Count; i++)
            {
                roundResults[i] = new List<JumpResult>();
                bibs[i] = new List<int>();
            }
        }

        public void AddResult(int competitorId, JumpResult jmp)
        {
            int round = roundResults[competitorId].Count;
            if (round > 0)
            {
                allroundResults.Remove(Tuple.Create(totalResults[competitorId], round - 1, bibs[competitorId][round - 1]));
            }

            roundResults[competitorId].Add(jmp);
            totalResults[competitorId] += jmp.totalPoints;

            allroundResults.Add(Tuple.Create(totalResults[competitorId], round, bibs[competitorId][round]), competitorId);

            // Update rank
            for (int i = 0; i < Math.Min(competitorsList.Count, allroundResults.Count); i++)
            {
                if (i > 0 && allroundResults.Keys[i].Item1 == allroundResults.Keys[i - 1].Item1)
                {
                    rank[allroundResults.Values[i]] = rank[allroundResults.Values[i - 1]];
                }
                else
                {
                    rank[allroundResults.Values[i]] = i + 1;
                }
            }

        }

        public int CompetitorRankKo(int competitorId)
        {
            var key = Tuple.Create(koState[competitorId], totalResults[competitorId], -1);
            int lo = 0, hi = finalResults.Count;
            while (lo < hi)
            {
                int index = lo + (hi - lo) / 2;
                var el = finalResults.Keys[index];
                if (finalResults.Comparer.Compare(el, key) >= 0) { hi = index; }
                else { lo = index + 1; }
            }
            return hi + 1;
        }
        public void AddNormalResult(int competitorId)
        {
            finalResults.Add(Tuple.Create(0, totalResults[competitorId], bibs[competitorId][bibs[competitorId].Count - 1]), competitorId);
        }
        public void AddKoResult(int competitorId)
        {
            finalResults.Add(Tuple.Create(0, totalResults[competitorId], bibs[competitorId][bibs[competitorId].Count - 1]), competitorId);
        }
        public void AddKoResult(int competitorId1, int competitorId2)
        {
            finalResults.Remove(Tuple.Create(0, totalResults[competitorId1], bibs[competitorId1][bibs[competitorId1].Count - 1]));
            int loserId = competitorId1, winnerId = competitorId2;
            if (totalResults[competitorId1] > totalResults[competitorId2])
            {
                winnerId = competitorId1;
                loserId = competitorId2;
            }

            int loserBib = bibs[loserId][bibs[loserId].Count - 1];
            int winnerBib = bibs[winnerId][bibs[winnerId].Count - 1];

            finalResults.Add(Tuple.Create(0, totalResults[winnerId], winnerBib), winnerId);

            losersResults.Add(Tuple.Create(totalResults[loserId], loserBib), loserId);
            int loserRank = losersResults.IndexOfKey(Tuple.Create(totalResults[loserId], loserBib));
            if (loserRank < maxLosers)
            {
                if (losersResults.Count > maxLosers)
                {
                    var lastLoser = losersResults.Keys[maxLosers];
                    int lastLoserId = losersResults.Values[maxLosers];
                    koState[lastLoserId] = 1;
                    finalResults.Remove(Tuple.Create(0, lastLoser.Item1, lastLoser.Item2));
                    finalResults.Add(Tuple.Create(1, lastLoser.Item1, lastLoser.Item2), lastLoserId);
                }
                finalResults.Add(Tuple.Create(0, totalResults[loserId], loserBib), loserId);
            }
            else
            {
                koState[loserId] = 1;
                finalResults.Add(Tuple.Create(1, totalResults[loserId], loserBib), loserId);
            }
        }
    }

    [Serializable]
    public class ClassificationResults
    {
        public List<decimal>[] eventResults;
        public decimal[] totalResults;
        public int[] rank;
        public SortedList<Tuple<decimal, int>, int> totalSortedResults;

        public ClassificationResults()
        {
            Comparison<Tuple<decimal, int>> comp = (x, y) => (x.Item1 == y.Item1 ? x.Item2.CompareTo(y.Item2) : y.Item1.CompareTo(x.Item1));
            Comparer<Tuple<decimal, int>> comparer = Comparer<Tuple<decimal, int>>.Create(comp);
            totalSortedResults = new SortedList<Tuple<decimal, int>, int>(comparer);
        }

        public void AddResult(int competitorId, decimal val)
        {
            totalSortedResults.Remove(Tuple.Create(totalResults[competitorId], competitorId));
            eventResults[competitorId].Add(val);
            totalResults[competitorId] += val;
            totalSortedResults.Add(Tuple.Create(totalResults[competitorId], competitorId), competitorId);

            // Update rank
            for (int i = 0; i < totalSortedResults.Count; i++)
            {
                if (i > 0 && totalSortedResults.Keys[i].Item1 == totalSortedResults.Keys[i - 1].Item1)
                {
                    rank[totalSortedResults.Values[i]] = rank[totalSortedResults.Values[i - 1]];
                }
                else
                {
                    rank[totalSortedResults.Values[i]] = i + 1;
                }
            }
        }
    }
}