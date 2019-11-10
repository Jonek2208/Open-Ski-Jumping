﻿using System;
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
    public enum EventPreset { Qualification, QualificationFlying, Individual2Rounds, Individual4Rounds }

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
        public float kPoint;
        public float hs;
        public float pointsPerMeter;
        public float kPointPoints;
        public float tailWindPoints;
        public float headWindPoints;
        public float gatePoints;
        public float gatesSpacing;
        public HillInfo() { }
        public HillInfo(float _kPoint, float _hs, float _pointsPerMeter, float _kPointPoints, float _gatePoints = 0, float _gatesSpacing = 0, float _headWindPoints = 0, float _tailWindPoints = 0)
        {
            kPoint = _kPoint;
            hs = _hs;
            pointsPerMeter = _pointsPerMeter;
            kPointPoints = _kPointPoints;
            tailWindPoints = _tailWindPoints;
            headWindPoints = _headWindPoints;
            gatePoints = _gatePoints;
            gatesSpacing = _gatesSpacing;
        }
        public float DistancePoints(float distance) => kPointPoints + (distance - kPoint) * pointsPerMeter;
        public float WindPoints(float wind) => -wind * (wind >= 0 ? headWindPoints : tailWindPoints);
        public float GatePoints(int gateBefore, int gateAfter) => (gateBefore - gateAfter) * gatesSpacing * gatePoints;
    }

    public class EventResults
    {
        public List<float>[] roundResults;
        public float[] totalResults;
        public List<Tuple<float, int>> finalResults;
        public List<int> competitorsList;

        public int[] rank;
        public int[] lastRank;

        public EventResults()
        {
            finalResults = new List<Tuple<float, int>>();
            competitorsList = new List<int>();
        }
        public EventResults(Event e)
        {

        }
        public int AddResult(int competitorId, float val)
        {
            roundResults[competitorId].Add(val);
            totalResults[competitorId] += val;
            bool flag = false;
            // Func<IndividualResult, IndividualResult, bool> compareResults = (res1, res2) => (res1.totalPoints > res2.totalPoints) ||
            //     (res1.totalPoints == res2.totalPoints && res1.jumps[res1.jumps.Count - 1].bib > res2.jumps[res2.jumps.Count - 1].bib);
            for (int i = 0; i < finalResults.Count; i++)
            {
                if (!flag && (totalResults[competitorId] > totalResults[finalResults[i].Item2]))
                {
                    flag = true;
                    finalResults.Insert(i, Tuple.Create(totalResults[competitorId], competitorId));
                }

                if (i > 0 && finalResults[i].Item1 == finalResults[i - 1].Item1)
                {
                    rank[finalResults[i].Item2] = rank[finalResults[i - 1].Item2];
                }
                else
                {
                    rank[finalResults[i].Item2] = i + 1;
                }
            }
            if (!flag)
            {
                finalResults.Add(Tuple.Create(totalResults[competitorId], competitorId));
                if (finalResults.Count >= 2 && finalResults[finalResults.Count - 1].Item1 == finalResults[finalResults.Count - 2].Item1)
                {
                    rank[competitorId] = rank[finalResults[finalResults.Count - 2].Item2];
                }
                else
                {
                    rank[competitorId] = finalResults.Count;
                }
            }
            return rank[competitorId];
        }
    }

    public class ClassificationResults
    {
        public List<float>[] eventResults;
        public float[] totalResults;
        public List<Tuple<float, int>> totalSortedResults;

        public ClassificationResults()
        {
            totalSortedResults = new List<Tuple<float, int>>();
        }

        public void AddResult(int competitorId, float val)
        {
            eventResults[competitorId].Add(val);
            totalResults[competitorId] += val;
        }
    }

    public class Jump
    {
        public int competitorId;
        public float totalPoints;
        public float distance;
        public float distancePoints;
        public float[] judgesMarks;
        public bool[] judgesMask;
        public float judgesTotalPoints;
        public int gate;
        public float gatePoints;
        public float wind;
        public float windPoints;
        public float speed;
        public int rank;
        public int bib;

        public Jump()
        {
            judgesMask = new bool[5];
        }

        public Jump(float _distance, float[] _judgesMarks, int _gate, float _wind, float _speed, int _bib)
        {
            // competitorId = _competitorId;
            distance = _distance;
            judgesMarks = _judgesMarks;
            judgesMask = new bool[judgesMarks.Length];
            gate = _gate;
            wind = _wind;
            speed = _speed;
            bib = _bib;
            CalculateJudgesMarks();
        }

        public void CalculateJudgesMarks()
        {
            // If all values are equal then there could be a problem, but then mn will stay 0 and mx will stay 1
            int mn = 0, mx = 1;
            for (int i = 0; i < judgesMarks.Length; i++)
            {
                if (judgesMarks[mn] < judgesMarks[i]) mn = i;
                if (judgesMarks[mx] > judgesMarks[i]) mx = i;
                judgesTotalPoints += judgesMarks[i];
                judgesMask[i] = true;
            }

            judgesMask[mn] = judgesMask[mx] = false;
            judgesTotalPoints -= judgesMarks[mn] + judgesMarks[mx];
        }
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
        [JsonConverter(typeof(StringEnumConverter))]
        public EventType eventType;
        [JsonConverter(typeof(StringEnumConverter))]
        public RankType qualRankType;
        public int qualRankId;
        [JsonConverter(typeof(StringEnumConverter))]
        public LimitType inLimitType;
        public int inLimit;
        [JsonConverter(typeof(StringEnumConverter))]
        public RankType ordRankType;
        public int ordRankId;
        public List<RoundInfo> roundInfos;
        public List<int> classifications;
        [JsonConverter(typeof(StringEnumConverter))]
        public EventPreset eventPreset;
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
        public List<int> hills;
        public List<Classification> classifications;
        public List<Event> events;
        public Calendar()
        {
            competitors = new List<Competitor>();
            teams = new List<Team>();
            // hills = new List<HillInfo>();
            hills = new List<int>();
            classifications = new List<Classification>();
            events = new List<Event>();
        }
    }

    public class CalendarResults
    {
        public Calendar calendar;
        public List<HillProfile.ProfileData> hillProfiles;
        public int eventIt;
        public int roundIt;
        public int jumpIt;

        public EventResults[] eventResults;
        public ClassificationResults[] classificationResults;
        public EventResults CurrentEventResults
        {
            get => eventResults[eventIt];
            set => eventResults[eventIt] = value;
        }
    }
}