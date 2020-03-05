using System;
using System.Collections.Generic;
using System.Linq;

namespace CompCal
{
    public class EventProcessor
    {
        private EventResults eventResults;
        private ResultsContainer resultsContainer;
        private Calendar calendar;
        private EventInfo eventInfo;
        private List<Participant> participants;
        private List<Competitor> competitors;
        private List<Team> teams;

        private EventResults ordRankEvent;
        private ClassificationResults ordRankClassification;

        private SortedList<(int, decimal, int), int> finalResults;
        private SortedList<(int, decimal, int), int> allroundResults;
        private List<int> competitorsList;
        private List<int>[] currentStartLists;
        private int[] rank;
        private int[] lastRank;

        private List<int> startList;

        public EventProcessor(int eventId, Calendar calendar, ResultsContainer resultsContainer)
        {
            this.eventInfo = calendar.events[eventId];
            this.eventResults = resultsContainer.eventResults[eventId];

            if (eventInfo.qualRankType == RankType.None)
            {
                //Add all registred participants
                this.competitorsList = eventResults.participants.Select((val, ind) => ind).ToList();
            }
            else
            {
                ResultsProcessor resultsProcessor;
                if (eventInfo.qualRankType == RankType.Event)
                {
                    resultsProcessor = new EventResultsProcessor(resultsContainer.eventResults[eventInfo.qualRankId]);
                }
                else
                {
                    resultsProcessor = new ClassificationResultsProcessor(resultsContainer.classificationResults[eventInfo.qualRankId]);
                }

                this.competitorsList = resultsProcessor.GetTrimmedFinalResults(eventResults.participants, eventInfo.inLimitType, eventInfo.inLimit);

                if (this.eventInfo.useQualRank)
                {
                    InjectQualRankResults(resultsProcessor.GetFinalResultsWithTotalPoints());
                }
            }


            // this.startList = resultsProcessor.GetFinalResultsWithCompetitorsList(this.competitorsList);

        }

        public void AddResult(JumpResult jmp, int localId, int innerId = 0)
        {
            this.eventResults.results[localId].Results[innerId].results.Add(jmp);
            this.eventResults.results[localId].TotalResults[innerId] += jmp.totalPoints;
            this.eventResults.results[localId].TotalPoints += jmp.totalPoints;

            //add to sorted results
            // this.allroundResults.Add((0, this.eventResults.results[localId].totalPoints, ))
        }

        private void GetCompetitors()
        {
            if (this.eventInfo.qualRankType != RankType.None)
            {
                List<(decimal, int)> tempList;
                if (this.eventInfo.qualRankType == RankType.Event)
                {
                    EventResults qualRankEvent = this.resultsContainer.eventResults[this.eventInfo.qualRankId];
                    tempList = GetQualRankEvent(qualRankEvent);
                }
                else
                {
                    ClassificationResults qualRankClassification = this.resultsContainer.classificationResults[this.eventInfo.qualRankId];
                    tempList = GetQualRankClassification(qualRankClassification);
                }

                // remove not registred participants
                var lookup = this.participants.Select((val, ind) => (val, ind)).ToDictionary(it => it.val.id, it => it.ind);
                tempList = tempList.Where(it => lookup.ContainsKey(it.Item2)).ToList();

                // trim list to inLimit 
                this.competitorsList = TrimRankingToLimit(tempList, eventInfo.inLimitType, eventInfo.inLimit);
            }
            else
            {
                //Add all registred participants
                competitorsList = eventResults.participants.Select((val, ind) => ind).ToList();
            }
        }

        private List<int> GetInitialOrder()
        {
            List<int> tmpList;
            if (eventInfo.ordRankType == RankType.None)
            {
                tmpList = competitorsList.Select((val, ind) => ind).ToList();
            }
            else
            {
                if (eventInfo.ordRankType == RankType.Event)
                {
                    tmpList = GetOrdRankEvent();
                }
                else
                {
                    tmpList = GetOrdRankClassification();
                }
            }
            return tmpList;
        }

        private List<(decimal, int)> GetQualRankEvent(EventResults qualRankEvent)
        {
            List<(decimal, int)> tempList = new List<(decimal, int)>();
            tempList = qualRankEvent.finalResults.Select(it => (qualRankEvent.results[it].TotalPoints, qualRankEvent.competitorIds[it])).ToList();
            return tempList;
        }

        private List<(decimal, int)> GetQualRankClassification(ClassificationResults qualRankClassification)
        {
            List<(decimal, int)> tempList = new List<(decimal, int)>();
            tempList = qualRankClassification.totalSortedResults.Select(it => (qualRankClassification.totalResults[it], it)).ToList();
            return tempList;
        }

        private List<int> TrimRankingToLimit(List<(decimal, int)> results, LimitType inLimitType = LimitType.None, int inLimit = 0)
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

        private List<int> GetOrdRankClassification()
        {
            ClassificationResults ordRankResults = resultsContainer.classificationResults[eventInfo.ordRankId];
            return competitorsList.Select((val, ind) => (val, ind)).OrderByDescending(item => (ordRankResults.totalResults[competitorsList[item.ind]], item.ind)).Select(item => item.val).ToList();
        }

        private List<int> GetOrdRankEvent()
        {
            List<(decimal, int, int)> tmpList = new List<(decimal, int, int)>();
            EventResults ordRankResults = resultsContainer.eventResults[eventInfo.ordRankId];
            var map = competitorsList.Select((val, index) => (val, index)).ToDictionary(x => x.val, x => x.index);
            bool[] selected = new bool[competitorsList.Count];
            int it = 0;

            for (int i = 0; i < ordRankResults.allroundResults.Count; i++)
            {
                int localId = ordRankResults.allroundResults[i];
                int globalId = ordRankResults.competitorIds[localId];

                if (map.ContainsKey(globalId)) //check if id from ORE is in current event competitors list
                {
                    selected[map[globalId]] = true;
                    decimal points = ordRankResults.results[localId].TotalPoints;
                    tmpList.Add((points, it++, map[globalId]));
                }
            }

            for (int i = 0; i < competitorsList.Count; i++)
            {
                if (!selected[i]) { tmpList.Add((0m, it++, i)); }
            }

            return tmpList.OrderByDescending(item => item).Select(item => item.Item3).ToList();
        }

        private List<(int, int)> AssignBibs(List<int> orderedParticipants, bool reversedOrder)
        {
            if (reversedOrder)
            {
                return orderedParticipants.Select((val, ind) => (val, ind + 1)).ToList();
            }
            return orderedParticipants.Select((val, ind) => (val, orderedParticipants.Count - ind)).ToList();
        }
        private List<int> RoundStartList(List<int> orderedParticipants, RoundInfo roundInfo)
        {
            if (roundInfo.roundType == RoundType.Normal)
            {
                return orderedParticipants.Select(item => item).Reverse().ToList();
            }
            List<int> tmpList = new List<int>();

            int len = orderedParticipants.Count;
            for (int i = 0; i < len / 2; i++)
            {
                tmpList.Add(orderedParticipants[(len + 1) / 2 + i]);
                tmpList.Add(orderedParticipants[(len + 1) / 2 - i - 1]);
            }

            if (len % 2 == 1)
            {
                tmpList.Add(orderedParticipants[0]);
            }

            return tmpList;
        }

        private List<int> SubRoundStartList(List<int> roundStartList, int subRoundId = 0, List<int> orderedParticipants = null, bool changeOrder = false)
        {
            if (changeOrder)
            {
                return orderedParticipants.Select(item => participants[competitorsList[item]].competitors[subRoundId]).Reverse().ToList();
            }
            return roundStartList.Select(item => participants[competitorsList[item]].competitors[subRoundId]).ToList();
        }

        private void CalculateFinalResults()
        {

        }
        private void InjectQualRankResults(List<(decimal, int)> resultsList)
        {
            foreach (var item in resultsList)
            {
                eventResults.results[item.Item2].QualRankPoints = item.Item1;
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
                ClassificationInfo classificationInfo = calendar.classifications[it];
                ClassificationResults classificationResults = resultsContainer.classificationResults[it];
                var resultsUpdate = eventFinalResults.GetPoints(classificationInfo);

                // Update results
                foreach (var item in resultsUpdate)
                {
                    classificationResults.totalResults[item.Item1] += item.Item2;
                }

                // Update sorted results
                classificationResults.totalSortedResults = classificationResults.totalResults.OrderByDescending(x => x).Select((val, ind) => ind).ToList();

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

/*
1. GetQualRank
2. GetOrdRank
(Inject QualRank)

  Round
  (AssignBibs)
    SubRound
      CreateStartList
      RunSubRound
  CalculateFinalResults


*/
