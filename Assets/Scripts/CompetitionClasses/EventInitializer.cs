using System.Collections.Generic;
using System.Linq;

namespace CompCal
{
    public class EventInitializer
    {
        public static List<int> GetCompetitorsFromQualRank(int eventId, Calendar calendar, ResultsContainer resultsContainer)
        {
            EventInfo eventInfo = calendar.events[eventId];
            EventResults eventResults = resultsContainer.eventResults[eventId];
            ResultsProcessor resultsProcessor;

            if (eventInfo.qualRankType == RankType.None)
            {
                //Add all registred participants
                return eventResults.participants.Select((val, ind) => ind).ToList();
            }

            if (eventInfo.qualRankType == RankType.Event)
            {
                resultsProcessor = new EventResultsProcessor(resultsContainer.eventResults[eventInfo.qualRankId]);
            }
            else
            {
                resultsProcessor = new ClassificationResultsProcessor(resultsContainer.classificationResults[eventInfo.qualRankId]);
            }

            return resultsProcessor.GetTrimmedFinalResults(eventResults.participants, eventInfo.inLimitType, eventInfo.inLimit);
        }

        // public static List<int> GetInitialOrder(int eventId, Calendar calendar, ResultsContainer resultsContainer)
        // {
        //     EventInfo eventInfo = calendar.events[eventId];
        //     EventResults eventResults = resultsContainer.eventResults[eventId];
        //     ResultsProcessor resultsProcessor;
            
        //     List<int> tmpList;
        //     if (eventInfo.ordRankType == RankType.None)
        //     {
        //         tmpList = competitorsList.Select((val, ind) => ind).ToList();
        //     }
        //     else
        //     {
        //         if (eventInfo.ordRankType == RankType.Event)
        //         {
        //             tmpList = GetOrdRankEvent();
        //         }
        //         else
        //         {
        //             tmpList = GetOrdRankClassification();
        //         }
        //     }
        //     return tmpList;
        // }
    }
}