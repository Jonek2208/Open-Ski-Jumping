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

        private SortedList<Tuple<int, decimal, int>, int> finalResults;
        private SortedList<Tuple<decimal, int, int>, int> allroundResults;
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
                competitorsList = eventResults.participants.Select(it => it.id).ToList();
            }

            switch (eventInfo.ordRankType)
            {
                case RankType.None:
                    break;
                case RankType.Event:
                    InjectOrdRankEvent();
                    break;
                case RankType.Classification:
                    InjectOrdRankClassification();
                    break;
            }
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
            tempList = qualRankClassification.totalSortedResults.Select(it => Tuple.Create(qualRankClassification.totalResults[it.Item1], it.Item1)).ToList();
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

        private void InjectOrdRankClassification()
        {
            ClassificationResults ordRankResults = calendarResults.classificationResults[eventInfo.ordRankId];
            for (int i = 0; i < competitorsList.Count; i++)
            {
                decimal points = ordRankResults.totalResults[competitorsList[i]];
                finalResults.Add(Tuple.Create(0, points, i), i);
            }
        }

        private void InjectOrdRankEvent()
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
                    finalResults.Add(Tuple.Create(0, points, it++), map[globalId]);
                }
            }

            for (int i = 0; i < competitorsList.Count; i++)
            {
                if (!selected[i]) { finalResults.Add(Tuple.Create(0, 0m, it++), i); }
            }
        }

        private List<int> RoundInit()
        {
            //ToDo
            return new List<int>();
        }

        public void UpdateClassifications()
        {
            foreach (var it in eventInfo.classifications)
            {
                ClassificationInfo classificationInfo = calendarResults.calendar.classifications[it];
                ClassificationResults classificationResults = calendarResults.classificationResults[it];
                IClassificationUpdater classificationUpdater;

                if (classificationInfo.eventType == EventType.Individual)
                { classificationUpdater = new ClassificationUpdaterIndividual(); }
                else
                { classificationUpdater = new ClassificationUpdaterTeam(); }

                if (classificationInfo.classificationType == ClassificationType.Points)
                {
                    for (int i = 0; i < competitorsList.Count; i++)
                    {
                        int id = competitorsList[i];
                        classificationResults.totalResults[id] += classificationUpdater.GetPoints(classificationInfo, eventResults.results[i]);
                        //update sorted results
                    }
                }
            }
        }


    }
}

