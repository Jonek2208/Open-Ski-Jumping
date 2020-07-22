using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.UI.TournamentMenu;

namespace OpenSkiJumping
{
    [Serializable]
    public class GameSave
    {
        public Calendar calendar;
        public List<ClassificationData> classificationsData;
        public List<CompetitorData> competitors;
        public string name;
        public ResultsDatabase resultsContainer;
        public List<TeamData> teams;

        public GameSave()
        {
        }

        public GameSave(string name, Calendar calendar, CompetitorsRuntime competitorsRuntime)
        {
            this.name = name;
            resultsContainer = new ResultsDatabase();
            this.calendar = calendar;

            resultsContainer.eventResults = new EventResults[calendar.events.Count];
            resultsContainer.classificationResults =
                new ClassificationResults[calendar.classifications.Count];

            for (var i = 0; i < resultsContainer.classificationResults.Length; i++)
                resultsContainer.classificationResults[i] = new ClassificationResults();

            classificationsData = calendar.classifications.Select((it, ind) =>
                new ClassificationData {useBib = false, priority = ind, classification = it}).ToList();

            competitors = calendar.competitorsIds.Select((item, index) =>
                    new CompetitorData
                        {calendarId = index, registered = true, competitor = competitorsRuntime.GetJumperById(item)})
                .ToList();

            var competitorsByCountry = competitors.ToLookup(it => it.competitor.countryCode, it => it);

            teams = calendar.teams.Select((item, index) => new TeamData
            {
                calendarId = index, registered = true, team = item,
                competitors = competitorsByCountry[item.countryCode].Select((it, ind) => new CompetitorData
                    {
                        calendarId = it.calendarId, teamId = ind, competitor = it.competitor, registered = it.registered
                    })
                    .ToList()
            }).ToList();

            for (int i = 0; i < calendar.classifications.Count; i++)
            {
                resultsContainer.classificationResults[i] = new ClassificationResults();
                var cnt = calendar.classifications[i].eventType == EventType.Individual
                    ? calendar.competitorsIds.Count
                    : calendar.teams.Count;
                resultsContainer.classificationResults[i].rank = Enumerable.Repeat(1, cnt).ToList();
                resultsContainer.classificationResults[i].totalResults = Enumerable.Repeat(0m, cnt).ToList();
                resultsContainer.classificationResults[i].totalSortedResults = Enumerable.Range(0, cnt).ToList();
            }
        }
    }

    [Serializable]
    public class SaveData
    {
        public int currentSaveId;
        public List<GameSave> savesList;

        public SaveData(int currentSaveId, List<GameSave> savesList)
        {
            this.currentSaveId = currentSaveId;
            this.savesList = savesList;
        }
    }
}