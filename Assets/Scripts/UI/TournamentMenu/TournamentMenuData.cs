using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using UnityEngine;

namespace OpenSkiJumping.UI.TournamentMenu
{
    [Serializable]
    public class ClassificationData
    {
        public int priority;
        public bool useBib;
        public ClassificationInfo classification;
    }

    [Serializable]
    public class CompetitorData
    {
        public int calendarId;
        public int teamId;
        public bool registered;
        public Competitor competitor;
    }

    [Serializable]
    public class TeamData
    {
        public int calendarId;
        public bool registered;
        public Team team;
        public List<CompetitorData> competitors;
        public IEnumerable<Competitor> GetTeamMembers() => competitors.Take(4).Select(it => it.competitor);
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/TournamentMenuData")]
    public class TournamentMenuData : ScriptableObject
    {
        [SerializeField] private SavesRuntime savesRuntime;
        public Calendar Calendar => GameSave.calendar;

        public string GetRank(RankType rankType, int id)
        {
            if (rankType == RankType.Classification)
                return Calendar.classifications[id].name;
            if (rankType == RankType.Event)
                return $"{id + 1}. {Calendar.events[id].hillId}";
            return "";
        }

        public IEnumerable<EventInfo> GetEvents()
        {
            return Calendar.events;
        }

        public List<ClassificationData> Classifications => GameSave.classificationsData;

        public List<CompetitorData> Competitors => GameSave.competitors;

        public List<TeamData> Teams => GameSave.teams.Where(data => data.competitors.Count >= 4).ToList();

        public GameSave GameSave => savesRuntime.GetCurrentSave();

        public TeamData EditedTeam { get; set; }
        public EventInfo SelectedEventCalendar { get; set; }

        public void ChangeClassificationPriority(ClassificationData item, int value)
        {
            if (0 > value || value >= GameSave.classificationsData.Count) return;
            var buf = GameSave.classificationsData[value];
            GameSave.classificationsData[value] = item;
            GameSave.classificationsData[item.priority] = buf;
            buf.priority = item.priority;
            item.priority = value;
        }

        public void ChangeCompetitorPriority(CompetitorData item, int value)
        {
            if (0 > value || value >= EditedTeam.competitors.Count) return;
            var buf = EditedTeam.competitors[value];
            EditedTeam.competitors[value] = item;
            EditedTeam.competitors[item.teamId] = buf;
            buf.teamId = item.teamId;
            item.teamId = value;
        }

        public EventInfo GetCurrentEvent()
        {
            var currentId = GameSave.resultsContainer.eventIndex;
            return GameSave.calendar.events[currentId];
        }

        public int GetCurrentEventId()
        {
            return GameSave.resultsContainer.eventIndex;
        }
    }
}