using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.Competition
{
    public abstract class ResultsProcessor
    {
        private static IEnumerable<int> TrimRankingToLimit(IEnumerable<(decimal, int)> results,
            LimitType inLimitType = LimitType.None,
            int inLimit = 0)
        {
            var resultsList = results.ToList();
            if (resultsList.Count == 0) return Enumerable.Empty<int>();
            
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

        public IEnumerable<int> GetTrimmedFinalResultsPreQual(IEnumerable<Participant> participants,
            LimitType inLimitType, int inLimit, IEnumerable<int> preQualified)
        {
            var tempList = GetFinalResultsWithTotalPoints();
            // remove not registered participants
            var lookup = participants.Select((val, ind) => (val, ind)).ToDictionary(it => it.val.id, it => it.ind);
            var preQualSet = new HashSet<int>(preQualified);
            tempList = tempList.Where(it => lookup.ContainsKey(it.Item2) && !preQualSet.Contains(it.Item2));
            var newInLimit = Math.Max(0, inLimit - preQualSet.Count);
            // trim list to inLimit 
            return TrimRankingToLimit(tempList, inLimitType, newInLimit).Concat(preQualSet);
        }

        public IEnumerable<int> GetTrimmedFinalResults(IEnumerable<Participant> participants, LimitType inLimitType,
            int inLimit)
        {
            return GetTrimmedFinalResultsPreQual(participants, inLimitType, inLimit, Enumerable.Empty<int>());
        }
    }

    public class EventResultsProcessor : ResultsProcessor
    {
        private readonly EventResults _results;

        public EventResultsProcessor(EventResults results)
        {
            _results = results;
        }

        public override IEnumerable<int> GetFinalResultsWithCompetitorsList(IEnumerable<int> competitors)
        {
            var tmpList = new List<(decimal, int, int)>();
            var competitorsList = competitors.ToList();
            var map = competitorsList.Select((val, index) => (val, index)).ToDictionary(x => x.val, x => x.index);
            var selected = new bool[competitorsList.Count];
            var it = 0;

            foreach (var localId in _results.allroundResults)
            {
                var globalId = _results.competitorIds[localId];
                if (!map.ContainsKey(globalId)) continue;

                selected[map[globalId]] = true;
                var points = _results.results[localId].TotalPoints;
                tmpList.Add((points, it++, map[globalId]));
            }

            for (var i = 0; i < competitorsList.Count; i++)
                if (!selected[i])
                    tmpList.Add((0m, it++, i));

            return tmpList.OrderByDescending(item => item).Select(item => item.Item3);
        }

        protected override IEnumerable<(decimal, int)> GetFinalResultsWithTotalPoints()
        {
            return _results.finalResults.Select(it => (_results.results[it].TotalPoints, _results.competitorIds[it]));
        }
    }

    public class ClassificationResultsProcessor : ResultsProcessor
    {
        private readonly ClassificationResults _results;

        public ClassificationResultsProcessor(ClassificationResults results)
        {
            this._results = results;
        }

        public override IEnumerable<int> GetFinalResultsWithCompetitorsList(IEnumerable<int> competitors)
        {
            var competitorsList = competitors.ToList();
            return competitorsList.OrderByDescending(it => (_results.totalResults[it], it));
        }

        protected override IEnumerable<(decimal, int)> GetFinalResultsWithTotalPoints()
        {
            return _results.totalSortedResults.Select(it => (_results.totalResults[it], it));
        }
    }
}