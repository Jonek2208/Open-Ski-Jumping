using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Competition
{
    public abstract class ResultsProcessor
    {
        private static IEnumerable<int> TrimRankingToLimit(IEnumerable<(decimal, int)> results,
            LimitType inLimitType = LimitType.None,
            int inLimit = 0)
        {
            var resultsList = results.ToList();
            switch (inLimitType)
            {
                case LimitType.Normal:
                {
                    var minPoints = resultsList[Math.Min(resultsList.Count, inLimit) - 1].Item1;
                    return resultsList.Where(it => it.Item1 >= minPoints).Select(it => it.Item2);
                }
                case LimitType.Exact:
                {
                    var cnt = Math.Min(resultsList.Count, inLimit);
                    return resultsList.Take(cnt).Select(it => it.Item2);
                }
                default:
                    return resultsList.Select(it => it.Item2);
            }
        }

        public abstract IEnumerable<int> GetFinalResultsWithCompetitorsList(IEnumerable<int> competitors);
        protected abstract IEnumerable<(decimal, int)> GetFinalResultsWithTotalPoints();

        public IEnumerable<int> GetTrimmedFinalResults(IEnumerable<Participant> participants, LimitType inLimitType,
            int inLimit)
        {
            var tempList = GetFinalResultsWithTotalPoints();
            // remove not registered participants
            var lookup = participants.Select((val, ind) => (val, ind)).ToDictionary(it => it.val.id, it => it.ind);
            tempList = tempList.Where(it => lookup.ContainsKey(it.Item2));

            // trim list to inLimit 
            return TrimRankingToLimit(tempList, inLimitType, inLimit);
        }
    }

    public class EventResultsProcessor : ResultsProcessor
    {
        private readonly EventResults results;

        public EventResultsProcessor(EventResults results)
        {
            this.results = results;
        }

        public override IEnumerable<int> GetFinalResultsWithCompetitorsList(IEnumerable<int> competitors)
        {
            var tmpList = new List<(decimal, int, int)>();
            var competitorsList = competitors.ToList();
            var map = competitorsList.Select((val, index) => (val, index)).ToDictionary(x => x.val, x => x.index);
            var selected = new bool[competitorsList.Count];
            var it = 0;

            foreach (var localId in results.allroundResults)
            {
                var globalId = results.competitorIds[localId];
                if (!map.ContainsKey(globalId)) continue;

                selected[map[globalId]] = true;
                var points = results.results[localId].TotalPoints;
                tmpList.Add((points, it++, map[globalId]));
            }

            for (var i = 0; i < competitorsList.Count; i++)
                if (!selected[i])
                    tmpList.Add((0m, it++, i));

            return tmpList.OrderByDescending(item => item).Select(item => item.Item3);
        }

        protected override IEnumerable<(decimal, int)> GetFinalResultsWithTotalPoints()
        {
            return results.finalResults.Select(it => (results.results[it].TotalPoints, results.competitorIds[it]));
        }
    }

    public class ClassificationResultsProcessor : ResultsProcessor
    {
        private readonly ClassificationResults results;

        public ClassificationResultsProcessor(ClassificationResults results)
        {
            this.results = results;
        }

        public override IEnumerable<int> GetFinalResultsWithCompetitorsList(IEnumerable<int> competitors)
        {
            var competitorsList = competitors.ToList();
            foreach(var it in competitorsList) Debug.Log($"{it}: {results.totalResults[it]}");
            return competitorsList.OrderByDescending(it => (results.totalResults[it], it));
        }

        protected override IEnumerable<(decimal, int)> GetFinalResultsWithTotalPoints()
        {
            return results.totalSortedResults.Select(it => (results.totalResults[it], it));
        }
    }
}