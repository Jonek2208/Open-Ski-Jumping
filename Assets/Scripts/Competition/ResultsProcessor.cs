using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.Competition
{
    public abstract class ResultsProcessor
    {
        protected virtual List<int> TrimRankingToLimit(List<(decimal, int)> results, LimitType inLimitType = LimitType.None, int inLimit = 0)
        {
            List<int> competitorsList = new List<int>();

            switch (inLimitType)
            {
                case LimitType.None:
                    competitorsList = results.Select(it => it.Item2).ToList();
                    break;
                case LimitType.Normal:
                    decimal minPoints = results[Math.Min(results.Count, inLimit) - 1].Item1;
                    competitorsList = results.Where(it => it.Item1 >= minPoints).Select(it => it.Item2).ToList();
                    break;
                case LimitType.Exact:
                    int cnt = Math.Min(results.Count, inLimit);
                    competitorsList = results.Take(cnt).Select(it => it.Item2).ToList();
                    break;
            }

            return competitorsList;
        }

        public abstract List<int> GetFinalResultsWithCompetitorsList(List<int> competitorsList);
        public abstract List<(decimal, int)> GetFinalResultsWithTotalPoints();

        public virtual List<int> GetTrimmedFinalResults(List<Participant> participants, LimitType inLimitType, int inLimit)
        {
            List<(decimal, int)> tempList = GetFinalResultsWithTotalPoints();
            // remove not registred participants
            var lookup = participants.Select((val, ind) => (val, ind)).ToDictionary(it => it.val.id, it => it.ind);
            tempList = tempList.Where(it => lookup.ContainsKey(it.Item2)).ToList();

            // trim list to inLimit 
            return TrimRankingToLimit(tempList, inLimitType, inLimit);
        }
    }

    public class EventResultsProcessor : ResultsProcessor
    {
        private readonly EventResults eventResults;
        public EventResultsProcessor(EventResults eventResults)
        {
            this.eventResults = eventResults;
        }

        public override List<int> GetFinalResultsWithCompetitorsList(List<int> competitorsList)
        {
            List<(decimal, int, int)> tmpList = new List<(decimal, int, int)>();
            var map = competitorsList.Select((val, index) => (val, index)).ToDictionary(x => x.val, x => x.index);
            bool[] selected = new bool[competitorsList.Count];
            int it = 0;

            for (int i = 0; i < eventResults.allroundResults.Count; i++)
            {
                int localId = eventResults.allroundResults[i];
                int globalId = eventResults.competitorIds[localId];

                if (map.ContainsKey(globalId)) //check if id from ORE is in current event competitors list
                {
                    selected[map[globalId]] = true;
                    decimal points = eventResults.results[localId].TotalPoints;
                    tmpList.Add((points, it++, map[globalId]));
                }
            }

            for (int i = 0; i < competitorsList.Count; i++)
            {
                if (!selected[i]) { tmpList.Add((0m, it++, i)); }
            }

            return tmpList.OrderByDescending(item => item).Select(item => item.Item3).ToList();
        }

        public override List<(decimal, int)> GetFinalResultsWithTotalPoints()
        {
            return eventResults.finalResults.Select(it => (eventResults.results[it].TotalPoints, eventResults.competitorIds[it])).ToList();
        }
    }

    public class ClassificationResultsProcessor : ResultsProcessor
    {
        private readonly ClassificationResults classificationResults;
        public ClassificationResultsProcessor(ClassificationResults classificationResults)
        {
            this.classificationResults = classificationResults;
        }

        public override List<int> GetFinalResultsWithCompetitorsList(List<int> competitorsList)
        {
            return competitorsList.Select((val, ind) => (val, ind))
                .OrderByDescending(item => (classificationResults.totalResults[competitorsList[item.ind]], item.ind))
                .Select(item => item.val).ToList();
        }

        public override List<(decimal, int)> GetFinalResultsWithTotalPoints()
        {
            return classificationResults.totalSortedResults.Select(it => (classificationResults.totalResults[it], it)).ToList();
        }
    }
}