using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
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
        [SerializeField] private List<ClassificationData> classifications;
        [SerializeField] private List<CompetitorData> competitors;
        [SerializeField] private List<TeamData> teams;
        [SerializeField] private GameSave gameSave;

        public Calendar Calendar => gameSave.calendar;

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

        public List<ClassificationData> Classifications
        {
            get => classifications;
            set => classifications = value;
        }

        public List<CompetitorData> Competitors
        {
            get => competitors;
            set => competitors = value;
        }

        public List<TeamData> Teams
        {
            get => teams;
            set => teams = value.Where(data => data.competitors.Count >= 4).ToList();
        }

        public GameSave GameSave
        {
            get => gameSave;
            set => gameSave = value;
        }

        public TeamData EditedTeam { get; set; }
        public EventInfo SelectedEventCalendar { get; set; }

        public void ChangeClassificationPriority(ClassificationData item, int value)
        {
            if (0 > value || value >= classifications.Count) return;
            var buf = classifications[value];
            classifications[value] = item;
            classifications[item.priority] = buf;
            buf.priority = item.priority;
            item.priority = value;
        }

        public void ChangeCompetitorPriority(CompetitorData item, int value)
        {
            if (0 > value || value >= EditedTeam.competitors.Count) return;
            var buf = EditedTeam.competitors[value];
            EditedTeam.competitors[value] = item;
            EditedTeam.competitors[item.calendarId] = buf;
            buf.calendarId = item.calendarId;
            item.calendarId = value;
        }

        public EventInfo GetCurrentEvent()
        {
            var currentId = gameSave.resultsContainer.eventIndex;
            return gameSave.calendar.events[currentId];
        }

        public int GetCurrentEventId()
        {
            return gameSave.resultsContainer.eventIndex;
        }
    }
}