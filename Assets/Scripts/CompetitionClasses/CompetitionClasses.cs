﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompetitionClasses
{
    public class Country
    {
        public string ioc;
        public string alpha2;
        public string en;
    }
    public class CountryData
    {
        public List<Country> countryList;
        public List<string> spritesList;
    }

    public enum Gender { Male, Female }
    public enum LimitType { None, Normal, Exact }
    public enum RoundType { Normal, KO }
    public enum EventType { Qualification, Individual, Team }
    public enum ClassificationType { IndividualPlace, IndividualPoints, TeamPlace, TeamPoints }
    public enum RankType { None, Event, Classification }

    public class Competitor
    {
        public string lastName;
        public string firstName;
        public string countryCode;
        [JsonConverter(typeof(StringEnumConverter))]
        public Gender gender;
        public DateTime birthdate;
        public Competitor() { }
        public Competitor(string _lastName, string _firstName, string _countryCode, Gender _gender, int _year, int _month, int _day)
        {
            lastName = _lastName;
            firstName = _firstName;
            countryCode = _countryCode;
            gender = _gender;
            birthdate = new DateTime(_year, _month, _day);
        }
    }

    public class Team
    {
        public string countryCode;
        public List<int> competitors;
        public Team(string _countryCode)
        {
            countryCode = _countryCode;
            competitors = new List<int>();
        }
    }

    public class HillInfo
    {

    }

    public class EventResults
    {
        public List<Tuple<int, List<float>>> roundResults;
        public List<Tuple<int, float>> totalResults;
        public List<Tuple<int, float>> finalResults;
        public List<int> competitorsList;

        public EventResults() { }
        public EventResults(Event e)
        {

        }
    }

    public class ClassificationResults
    {
        public List<Tuple<int, List<float>>> eventResults;
        public List<Tuple<int, float>> totalResults;
    }

    public class Results
    {

    }

    public class RoundInfo
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RoundType roundType;
        [JsonConverter(typeof(StringEnumConverter))]
        public LimitType outLimitType;
        public int outLimit;
        public bool has95Rule;
        public bool reassignBibs;
        public RoundInfo() { }
        public RoundInfo(RoundType _roundType, LimitType _outlimitType, int _outLimit, bool _has95Rule = true, bool _reassignBibs = false)
        {
            roundType = _roundType;
            outLimitType = _outlimitType;
            outLimit = _outLimit;
            has95Rule = _has95Rule;
            reassignBibs = _reassignBibs;
        }
    }

    public class Classification
    {
        public string name;
        [JsonConverter(typeof(StringEnumConverter))]
        public ClassificationType classificationType;
        public List<int> events;

        // Classification() { }
        public Classification(string _name, ClassificationType _classificationType)
        {
            name = _name;
            classificationType = _classificationType;
            events = new List<int>();
        }
    }
    public class Event
    {
        public string name;
        public int hillId;
        public EventType eventType;
        public RankType qualRankType;
        public int qualRankId;
        public LimitType inLimitType;
        public int inLimit;
        public RankType ordRankType;
        public int ordRankId;
        public List<RoundInfo> roundInfos;
        public List<int> classifications;
        public Event(string _name, int _hillId, EventType _eventType, List<RoundInfo> _roundInfos, List<int> _classifications, RankType _qualRankType, int _qualRankId, RankType _ordRankType, int _ordRankId, LimitType _inLimitType = LimitType.None, int _inLimit = 0)
        {
            name = _name;
            hillId = _hillId;
            roundInfos = _roundInfos;
            classifications = _classifications;
            qualRankType = _qualRankType;
            qualRankId = _qualRankId;
            ordRankType = _ordRankType;
            ordRankId = _ordRankId;
            inLimitType = _inLimitType;
            inLimit = _inLimit;
        }
    }

    public class Calendar
    {
        public List<Competitor> competitors;
        public List<Team> teams;
        // public List<HillInfo> hills;
        public List<string> hills;
        public List<Classification> classifications;
        public List<Event> events;
        public Calendar()
        {
            competitors = new List<Competitor>();
            teams = new List<Team>();
            // hills = new List<HillInfo>();
            hills = new List<string>();
            classifications = new List<Classification>();
            events = new List<Event>();
        }
    }

    public class CalendarResults
    {
        public Calendar calendar;
        public int eventIt;
        public int roundIt;
        public int jumpIt;

        public List<EventResults> eventResults;
        public List<ClassificationResults> classificationResults;
    }
}