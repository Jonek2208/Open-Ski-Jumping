using System;
using System.Collections.Generic;
using System.Linq;

namespace CompCal
{
    public class EventManager
    {
        private CalendarResults calendarResults;
        private List<Competitor> competitors;
        private List<Team> teams;
        private List<Participant> participants;
        private EventInfo eventInfo;
        private EventResults eventResults;

        private EventResults ordRankEvent;
        private ClassificationResults ordRankClassification;

        private SortedList<(int, decimal, int), int> finalResults;
        private SortedList<(int, decimal, int), int> allroundResults;
        private List<int> competitorsList;
        private List<int>[] privatestartLists;
        private int[] rank;
        private int[] lastRank;

        public void EventInit()
        {
            List<int> competitorsList;
            if (eventInfo.qualRankType != RankType.None)
            {
                List<Tuple<decimal, int>> tempList;
                if (eventInfo.qualRankType == RankType.Event)
                {
                    EventResults qualRankEvent = calendarResults.eventResults[eventInfo.qualRankId];
                    tempList = GetQualRankEvent(qualRankEvent);
                }
                else
                {
                    ClassificationResults qualRankClassification = calendarResults.classificationResults[eventInfo.qualRankId];
                    tempList = GetQualRankClassification(qualRankClassification);
                }

                // remove not registred participants
                var lookup = participants.Select((val, ind) => (val, ind)).ToDictionary(it => it.val.id, it => it.ind);
                tempList = tempList.Where(it => lookup.ContainsKey(it.Item2)).ToList();

                // trim list to inLimit 
                competitorsList = TrimRankingToLimit(tempList, eventInfo.inLimitType, eventInfo.inLimit);
                competitorsList.Select(it => lookup[it]).ToList();
            }
            else
            {
                //Add all registred participants
                competitorsList = eventResults.participants.Select((val, ind) => ind).ToList();
            }

            switch (eventInfo.ordRankType)
            {
                case RankType.None:
                    break;
                case RankType.Event:
                    GetOrdRankEvent();
                    break;
                case RankType.Classification:
                    GetOrdRankClassification();
                    break;
            }

            if (eventInfo.useOrdRank) { InjectOrdRankResults(); }
        }

        public void AddResult(JumpResult jmp, int localId, int innerId = 0)
        {
            this.eventResults.results[localId].results[innerId].Add(jmp);
            this.eventResults.results[localId].totalResults[innerId] += jmp.totalPoints;
            this.eventResults.results[localId].totalPoints += jmp.totalPoints;

            //add to sorted results
            // this.allroundResults.Add((0, this.eventResults.results[localId].totalPoints, ))
        }

        private List<Tuple<decimal, int>> GetQualRankEvent(EventResults qualRankEvent)
        {
            List<Tuple<decimal, int>> tempList = new List<Tuple<decimal, int>>();
            tempList = qualRankEvent.finalResults.Select(it => Tuple.Create(qualRankEvent.results[it].totalPoints, qualRankEvent.competitorIds[it])).ToList();
            return tempList;
        }

        private List<Tuple<decimal, int>> GetQualRankClassification(ClassificationResults qualRankClassification)
        {
            List<Tuple<decimal, int>> tempList = new List<Tuple<decimal, int>>();
            tempList = qualRankClassification.totalSortedResults.Select(it => Tuple.Create(qualRankClassification.totalResults[it], it)).ToList();
            return tempList;
        }

        private List<int> TrimRankingToLimit(List<Tuple<decimal, int>> results, LimitType inLimitType = LimitType.None, int inLimit = 0)
        {
            List<int> competitorsList = new List<int>();

            switch (eventInfo.inLimitType)
            {
                case LimitType.None:
                    competitorsList = results.Select(it => it.Item2).ToList();
                    break;
                case LimitType.Normal:
                    decimal minPoints = results[Math.Min(results.Count, eventInfo.inLimit) - 1].Item1;
                    competitorsList = results.Where(it => it.Item1 >= minPoints).Select(it => it.Item2).ToList();
                    break;
                case LimitType.Exact:
                    int cnt = Math.Min(results.Count, eventInfo.inLimit);
                    competitorsList = results.Take(cnt).Select(it => it.Item2).ToList();
                    break;
            }

            return competitorsList;
        }

        private void GetOrdRankClassification()
        {
            ClassificationResults ordRankResults = calendarResults.classificationResults[eventInfo.ordRankId];
            for (int i = 0; i < competitorsList.Count; i++)
            {
                decimal points = ordRankResults.totalResults[competitorsList[i]];
                finalResults.Add((0, points, i), i);
            }
        }

        private void GetOrdRankEvent()
        {
            EventResults ordRankResults = calendarResults.eventResults[eventInfo.ordRankId];
            var map = competitorsList.Select((val, index) => (val, index)).ToDictionary(it => it.val, it => it.index);
            bool[] selected = new bool[competitorsList.Count];
            int it = 0;

            for (int i = 0; i < ordRankResults.allroundResults.Count; i++)
            {
                int localId = ordRankResults.allroundResults[i];
                int globalId = ordRankResults.competitorIds[localId];

                if (map.ContainsKey(globalId)) //check if id from ORE is in current event competitors list
                {
                    selected[map[globalId]] = true;
                    decimal points = ordRankResults.results[localId].totalPoints;
                    finalResults.Add((0, points, it++), map[globalId]);
                }
            }

            for (int i = 0; i < competitorsList.Count; i++)
            {
                if (!selected[i]) { finalResults.Add((0, 0m, it++), i); }
            }
        }

        private void InjectOrdRankResults()
        {
            foreach (var item in finalResults)
            {
                eventResults.results[item.Value].ordRankPoints = item.Key.Item2;
            }
        }

        private List<int> RoundInit()
        {
            //ToDo
            throw new NotImplementedException();
        }

        public void UpdateClassifications()
        {
            IEventFinalResults eventFinalResults;

            if (eventInfo.eventType == EventType.Individual)
            { eventFinalResults = new IndividualEventFinalResults(eventResults, competitors); }
            else
            { eventFinalResults = new TeamEventFinalResults(eventResults); }

            foreach (var it in eventInfo.classifications)
            {
                ClassificationInfo classificationInfo = calendarResults.calendar.classifications[it];
                ClassificationResults classificationResults = calendarResults.classificationResults[it];
                var resultsUpdate = eventFinalResults.GetPoints(classificationInfo);

                // Update results
                foreach (var item in resultsUpdate)
                {
                    classificationResults.totalResults[item.Item1] += item.Item2;
                }

                // Update sorted results
                classificationResults.totalSortedResults = classificationResults.totalResults.OrderByDescending(it => it).Select((val, ind) => ind).ToList();

                // Calculate rank
                for (int i = 0; i < classificationResults.totalSortedResults.Count; i++)
                {
                    if (i > 0 && classificationResults.totalResults[classificationResults.totalSortedResults[i]] == classificationResults.totalResults[classificationResults.totalSortedResults[i - 1]])
                    {
                        classificationResults.rank[classificationResults.totalSortedResults[i]] = classificationResults.rank[classificationResults.totalSortedResults[i + 1]];
                    }
                    else
                    {
                        classificationResults.rank[classificationResults.totalSortedResults[i]] = i + 1;
                    }
                }
            }
        }
    }
}

