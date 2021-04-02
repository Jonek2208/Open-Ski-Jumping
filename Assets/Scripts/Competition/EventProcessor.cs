using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;

namespace OpenSkiJumping.Competition
{
    public static class EventProcessor
    {
        public static IEnumerable<int> GetCompetitors(Calendar calendar, ResultsDatabase resultsDatabase)
        {
            var eventId = resultsDatabase.eventIndex;
            var eventInfo = calendar.events[eventId];
            var eventResults = resultsDatabase.eventResults[eventId];
            IEnumerable<int> preQualifiedCompetitors;
            IEnumerable<int> competitorsList;
            ResultsProcessor ordRankProcessor;

            if (eventInfo.preQualRankType == RankType.None)
            {
                //Add all registered participants
                preQualifiedCompetitors = Enumerable.Empty<int>();
            }
            else
            {
                ResultsProcessor preQualRankProcessor;
                EventType preQualEventType;
                if (eventInfo.preQualRankType == RankType.Event)
                {
                    preQualRankProcessor =
                        new EventResultsProcessor(resultsDatabase.eventResults[eventInfo.preQualRankId]);
                    preQualEventType = calendar.events[eventInfo.preQualRankId].eventType;
                }
                else
                {
                    preQualRankProcessor =
                        new ClassificationResultsProcessor(
                            resultsDatabase.classificationResults[eventInfo.preQualRankId]);
                    preQualEventType = calendar.classifications[eventInfo.preQualRankId].eventType;
                }

                if (preQualEventType != eventInfo.eventType)
                    preQualifiedCompetitors = Enumerable.Empty<int>();
                else
                    preQualifiedCompetitors = preQualRankProcessor.GetTrimmedFinalResults(eventResults.participants,
                        eventInfo.preQualLimitType, eventInfo.preQualLimit);
            }

            if (eventInfo.qualRankType == RankType.None)
            {
                //Add all registered participants
                competitorsList = eventResults.participants.Select(x => x.id);
            }
            else
            {
                EventType qualEventType;

                ResultsProcessor qualRankProcessor;
                if (eventInfo.qualRankType == RankType.Event)
                {
                    qualRankProcessor = new EventResultsProcessor(resultsDatabase.eventResults[eventInfo.qualRankId]);
                    qualEventType = calendar.events[eventInfo.qualRankId].eventType;
                }
                else
                {
                    qualRankProcessor =
                        new ClassificationResultsProcessor(resultsDatabase.classificationResults[eventInfo.qualRankId]);
                    qualEventType = calendar.classifications[eventInfo.qualRankId].eventType;
                }

                if (qualEventType != eventInfo.eventType)
                    competitorsList = eventResults.participants.Select(x => x.id);
                else
                    competitorsList = qualRankProcessor.GetTrimmedFinalResultsPreQual(eventResults.participants,
                        eventInfo.inLimitType, eventInfo.inLimit, preQualifiedCompetitors);
            }

            EventType ordEventType;
            if (eventInfo.ordRankType == RankType.None) return competitorsList;
            if (eventInfo.ordRankType == RankType.Event)
            {
                ordRankProcessor = new EventResultsProcessor(resultsDatabase.eventResults[eventInfo.ordRankId]);
                ordEventType = calendar.events[eventInfo.ordRankId].eventType;
            }
            else
            {
                ordRankProcessor =
                    new ClassificationResultsProcessor(resultsDatabase.classificationResults[eventInfo.ordRankId]);
                ordEventType = calendar.classifications[eventInfo.ordRankId].eventType;
            }

            return eventInfo.eventType == ordEventType
                ? ordRankProcessor.GetFinalResultsWithCompetitorsList(competitorsList)
                : competitorsList;
        }

        public static List<Participant> EventParticipants(GameSave save, int eventId)
        {
            var eventParticipants =
                save.calendar.events[eventId].eventType == EventType.Individual
                    ? save.competitors.Where(it => it.registered).Select(it => new Participant
                    {
                        competitors = new List<int> {it.calendarId}, id = it.calendarId,
                        teamId = save.competitors[it.calendarId].teamId
                    }).ToList()
                    : save.teams.Where(it => it.registered && it.competitors.Count >= 4).Select(it => new Participant
                        {
                            competitors = it.competitors.Select(x => x.calendarId).Take(4).ToList(), id = it.calendarId,
                            teamId = it.calendarId
                        })
                        .ToList();
            return eventParticipants;
        }

        public static JumpResult GetJumpResult(IJumpData jumpData, IHillInfo hillInfo, bool gateComp, bool windComp)
        {
            var jump = new JumpResult(jumpData.Distance, jumpData.JudgesMarks, jumpData.GatesDiff, jumpData.Wind,
                jumpData.Speed);
            jump.distancePoints = hillInfo.GetDistancePoints(jump.distance);
            jump.windPoints = windComp ? hillInfo.GetWindPoints(jump.wind) : 0m;
            jump.gatePoints = gateComp ? hillInfo.GetGatePoints(jump.gatesDiff) : 0m;
            jump.totalPoints = Math.Max(0,
                jump.distancePoints + jump.judgesTotalPoints + jump.windPoints + jump.gatePoints);
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
            return val < 165 ? 60 : 120;
        }
    }
}