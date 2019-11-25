﻿using System;
using System.Collections.Generic;

namespace CompCal
{
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