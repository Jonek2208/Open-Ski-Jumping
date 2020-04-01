using System;
using System.Collections.Generic;
using System.Linq;

namespace Competition
{
    public static class EventProcessor
    {
        public static (RankType, string) ParseRankString(string value)
        {
            return (RankType.None, "");
        }
        public static List<int> GetCompetitors(int eventId, Calendar calendar, ResultsDatabase resultsDatabase)
        {
            EventInfo eventInfo = calendar.events[eventId];
            EventResults eventResults = resultsDatabase.eventResults[eventId];
            List<int> competitorsList;

            if (eventInfo.qualRankType == RankType.None)
            {
                //Add all registred participants
                competitorsList = eventResults.participants.Select((val, ind) => ind).ToList();
            }
            else
            {
                ResultsProcessor resultsProcessor;
                if (eventInfo.qualRankType == RankType.Event)
                {
                    resultsProcessor = new EventResultsProcessor(resultsDatabase.eventResults[eventInfo.qualRankId]);
                }
                else
                {
                    resultsProcessor = new ClassificationResultsProcessor(resultsDatabase.classificationResults[eventInfo.qualRankId]);
                }

                competitorsList = resultsProcessor.GetTrimmedFinalResults(eventResults.participants, eventInfo.inLimitType, eventInfo.inLimit);
            }

            if (eventInfo.ordRankType == RankType.Event)
            {
                return GetOrdRank(competitorsList, resultsDatabase.eventResults[eventInfo.ordRankId]);
            }
            if (eventInfo.ordRankType == RankType.Classification)
            {
                return GetOrdRank(competitorsList, resultsDatabase.classificationResults[eventInfo.ordRankId]);
            }

            return competitorsList;
        }

        // private void GetCompetitors()
        // {
        //     if (this.eventInfo.qualRankType != RankType.None)
        //     {
        //         List<(decimal, int)> tempList;
        //         if (this.eventInfo.qualRankType == RankType.Event)
        //         {
        //             EventResults qualRankEvent = this.resultsDatabase.eventResults[this.eventInfo.qualRankId];
        //             tempList = GetQualRank(qualRankEvent);
        //         }
        //         else
        //         {
        //             ClassificationResults qualRankClassification = this.resultsDatabase.classificationResults[this.eventInfo.qualRankId];
        //             tempList = GetQualRank(qualRankClassification);
        //         }

        //         // remove not registred participants
        //         var lookup = this.participants.Select((val, ind) => (val, ind)).ToDictionary(it => it.val.id, it => it.ind);
        //         tempList = tempList.Where(it => lookup.ContainsKey(it.Item2)).ToList();

        //         // trim list to inLimit 
        //         this.competitorsList = TrimRankingToLimit(tempList, eventInfo.inLimitType, eventInfo.inLimit);
        //     }
        //     else
        //     {
        //         //Add all registred participants
        //         competitorsList = eventResults.participants.Select((val, ind) => ind).ToList();
        //     }
        // }

        // public static List<int> GetInitialOrder(List<int> competitorsList, EventInfo eventInfo)
        // {
        //     List<int> tmpList;
        //     if (eventInfo.ordRankType == RankType.None)
        //     {
        //         tmpList = competitorsList.Select((val, ind) => ind).ToList();
        //     }
        //     else
        //     {
        //         if (eventInfo.ordRankType == RankType.Event)
        //         {
        //             tmpList = GetOrdRank(competitorsList, resultsDatabase.eventResults[eventInfo.ordRankId]);
        //         }
        //         else
        //         {
        //             tmpList = GetOrdRank(competitorsList, resultsDatabase.classificationResults[eventInfo.ordRankId]);
        //         }
        //     }
        //     return tmpList;
        // }

        public static List<(decimal, int)> GetQualRank(EventResults qualRankEvent)
        {
            List<(decimal, int)> tempList = new List<(decimal, int)>();
            tempList = qualRankEvent.finalResults.Select(it => (qualRankEvent.results[it].TotalPoints, qualRankEvent.competitorIds[it])).ToList();
            return tempList;
        }

        public static List<(decimal, int)> GetQualRank(ClassificationResults qualRankClassification)
        {
            List<(decimal, int)> tempList = new List<(decimal, int)>();
            tempList = qualRankClassification.totalSortedResults.Select(it => (qualRankClassification.totalResults[it], it)).ToList();
            return tempList;
        }

        // private static List<int> TrimRankingToLimit(List<(decimal, int)> results, LimitType inLimitType = LimitType.None, int inLimit = 0)
        // {
        //     List<int> competitorsList = new List<int>();

        //     switch (eventInfo.inLimitType)
        //     {
        //         case LimitType.None:
        //             competitorsList = results.Select(it => it.Item2).ToList();
        //             break;
        //         case LimitType.Normal:
        //             decimal minPoints = results[Math.Min(results.Count, eventInfo.inLimit) - 1].Item1;
        //             competitorsList = results.Where(it => it.Item1 >= minPoints).Select(it => it.Item2).ToList();
        //             break;
        //         case LimitType.Exact:
        //             int cnt = Math.Min(results.Count, eventInfo.inLimit);
        //             competitorsList = results.Take(cnt).Select(it => it.Item2).ToList();
        //             break;
        //     }

        //     return competitorsList;
        // }

        public static List<int> GetOrdRank(List<int> competitorsList, ClassificationResults ordRankResults)
        {
            return competitorsList.Select((val, ind) => (val, ind)).OrderByDescending(item => (ordRankResults.totalResults[competitorsList[item.ind]], item.ind)).Select(item => item.val).ToList();
        }

        public static List<int> GetOrdRank(List<int> competitorsList, EventResults ordRankResults)
        {
            List<(decimal, int, int)> tmpList = new List<(decimal, int, int)>();
            var map = competitorsList.Select((val, index) => (val, index)).ToDictionary(x => x.val, x => x.index);
            bool[] selected = new bool[competitorsList.Count];
            int it = 0;

            for (int i = 0; i < ordRankResults.allroundResults.Count; i++)
            {
                int localId = ordRankResults.allroundResults[i];
                int globalId = ordRankResults.competitorIds[localId];

                if (map.ContainsKey(globalId)) //check if id from OrdRank is in current event competitors list
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

        public static List<(int, int)> AssignBibs(List<int> orderedParticipants, bool reversedOrder)
        {
            if (reversedOrder)
            {
                return orderedParticipants.Select((val, ind) => (val, ind + 1)).ToList();
            }
            return orderedParticipants.Select((val, ind) => (val, orderedParticipants.Count - ind)).ToList();
        }
        public static List<int> GetStartList(List<int> orderedParticipants, RoundInfo roundInfo)
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

        // private List<int> SubRoundStartList(List<int> roundStartList, int subRoundId = 0, List<int> orderedParticipants = null, bool changeOrder = false)
        // {
        //     if (changeOrder)
        //     {
        //         return orderedParticipants.Select(item => participants[competitorsList[item]].competitors[subRoundId]).Reverse().ToList();
        //     }
        //     return roundStartList.Select(item => participants[competitorsList[item]].competitors[subRoundId]).ToList();
        // }


        // public void UpdateClassifications()
        // {
        //     IEventFinalResults eventFinalResults;

        //     if (eventInfo.eventType == EventType.Individual)
        //     { eventFinalResults = new IndividualEventFinalResults(eventResults, competitors); }
        //     else
        //     { eventFinalResults = new TeamEventFinalResults(eventResults); }

        //     foreach (var it in eventInfo.classifications)
        //     {
        //         ClassificationInfo classificationInfo = calendar.classifications[it];
        //         ClassificationResults classificationResults = resultsDatabase.classificationResults[it];
        //         var resultsUpdate = eventFinalResults.GetPoints(classificationInfo);

        //         // Update results
        //         foreach (var item in resultsUpdate)
        //         {
        //             classificationResults.totalResults[item.Item1] += item.Item2;
        //         }

        //         // Update sorted results
        //         classificationResults.totalSortedResults = classificationResults.totalResults.OrderByDescending(x => x).Select((val, ind) => ind).ToList();

        //         // Calculate rank
        //         for (int i = 0; i < classificationResults.totalSortedResults.Count; i++)
        //         {
        //             if (i > 0 && classificationResults.totalResults[classificationResults.totalSortedResults[i]] == classificationResults.totalResults[classificationResults.totalSortedResults[i - 1]])
        //             {
        //                 classificationResults.rank[classificationResults.totalSortedResults[i]] = classificationResults.rank[classificationResults.totalSortedResults[i + 1]];
        //             }
        //             else
        //             {
        //                 classificationResults.rank[classificationResults.totalSortedResults[i]] = i + 1;
        //             }
        //         }
        //     }
        // }
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
