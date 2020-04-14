using System;
using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;

namespace Competition
{
    public static class EventProcessor
    {
        public static List<int> GetCompetitors(Calendar calendar, ResultsDatabase resultsDatabase)
        {
            int eventId = resultsDatabase.eventIndex;
            EventInfo eventInfo = calendar.events[eventId];
            EventResults eventResults = resultsDatabase.eventResults[eventId];
            List<int> competitorsList;
            ResultsProcessor qualRankProcessor;
            ResultsProcessor ordRankProcessor;

            if (eventInfo.qualRankType == RankType.None)
            {
                //Add all registred participants
                competitorsList = eventResults.participants.Select((val, ind) => ind).ToList();
            }
            else
            {
                if (eventInfo.qualRankType == RankType.Event)
                {
                    qualRankProcessor = new EventResultsProcessor(resultsDatabase.eventResults[eventInfo.qualRankId]);
                }
                else
                {
                    qualRankProcessor = new ClassificationResultsProcessor(resultsDatabase.classificationResults[eventInfo.qualRankId]);
                }

                competitorsList = qualRankProcessor.GetTrimmedFinalResults(eventResults.participants, eventInfo.inLimitType, eventInfo.inLimit);
            }

            if (eventInfo.ordRankType == RankType.None) { return competitorsList; }

            if (eventInfo.ordRankType == RankType.Event)
            {
                ordRankProcessor = new EventResultsProcessor(resultsDatabase.eventResults[eventInfo.ordRankId]);
            }
            else
            {
                ordRankProcessor = new ClassificationResultsProcessor(resultsDatabase.classificationResults[eventInfo.ordRankId]);
            }

            return ordRankProcessor.GetFinalResultsWithCompetitorsList(competitorsList);
        }

        public static JumpResult GetJumpResult(JumpData jumpData, IHillInfo hillInfo)
        {
            JumpResult jump = new JumpResult(jumpData.Distance, jumpData.JudgesMarks, jumpData.GatesDiff, jumpData.Wind, jumpData.Speed);
            jump.distancePoints = hillInfo.GetDistancePoints(jump.distance);
            jump.windPoints = hillInfo.GetWindPoints(jump.wind);
            jump.gatePoints = hillInfo.GetGatePoints(jump.gatesDiff);
            jump.totalPoints = Math.Max(0, jump.distancePoints + jump.judgesTotalPoints + jump.windPoints + jump.gatePoints);
            return jump;
        }

        public static decimal GetPointsPerMeter(decimal val)
        {
            if (val < 25) return 4.8m;
            if (val < 30) return 4.4m;
            if (val < 35) return 4.0m;
            if (val < 40) return 3.6m;
            if (val < 50) return 3.2m;
            if (val < 60) return 2.8m;
            if (val < 70) return 2.4m;
            if (val < 80) return 2.2m;
            if (val < 100) return 2.0m;
            if (val < 165) return 1.8m;
            return 1.2m;
        }

        public static decimal GetKPointPoints(decimal val)
        {
            if (val < 165) return 60;
            return 120;
        }


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