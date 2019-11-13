using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Calendar
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
    public enum EventPreset { Qualification, QualificationFlying, QualificationKO, Individual2Rounds, Individual4Rounds, IndividualKO }

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
        public decimal kPoint;
        public decimal hs;
        public decimal pointsPerMeter;
        public decimal kPointPoints;
        public decimal tailWindPoints;
        public decimal headWindPoints;
        public decimal gatePoints;
        public decimal gatesSpacing;
        public HillInfo() { }
        public HillInfo(decimal _kPoint, decimal _hs)
        {
            kPoint = _kPoint;
            hs = _hs;
            decimal ptsPerMeter = 1.2m, kPointPts = (kPoint < 165m ? 60m : 120m);
            if (kPoint < 25m) ptsPerMeter = 4.8m;
            else if (kPoint < 30m) ptsPerMeter = 4.4m;
            else if (kPoint < 35m) ptsPerMeter = 4.0m;
            else if (kPoint < 40m) ptsPerMeter = 3.6m;
            else if (kPoint < 50m) ptsPerMeter = 3.2m;
            else if (kPoint < 60m) ptsPerMeter = 2.8m;
            else if (kPoint < 70m) ptsPerMeter = 2.4m;
            else if (kPoint < 80m) ptsPerMeter = 2.2m;
            else if (kPoint < 100m) ptsPerMeter = 2.0m;
            else if (kPoint < 165m) ptsPerMeter = 1.8m;
            else ptsPerMeter = 1.2m;
            kPointPoints = kPointPts;
            pointsPerMeter = ptsPerMeter;
        }
        public HillInfo(decimal _kPoint, decimal _hs, decimal _pointsPerMeter, decimal _kPointPoints, decimal _gatePoints = 0, decimal _gatesSpacing = 0, decimal _headWindPoints = 0, decimal _tailWindPoints = 0)
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
        public decimal DistancePoints(decimal distance) => kPointPoints + (distance - kPoint) * pointsPerMeter;
        public decimal WindPoints(decimal wind) => -wind * (wind >= 0 ? headWindPoints : tailWindPoints);
        public decimal GatePoints(int gateBefore, int gateAfter) => (gateBefore - gateAfter) * gatesSpacing * gatePoints;
    }

    public class RoundInfo
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RoundType roundType;
        [JsonConverter(typeof(StringEnumConverter))]
        public LimitType outLimitType;
        public int outLimit;
        public bool has95Rule;
        public bool reversedBibs;
        public bool reassignBibs;
        public RoundInfo() { }
        public RoundInfo(RoundType _roundType, LimitType _outlimitType, int _outLimit, bool _reversedBibs = false, bool _reassignBibs = false, bool _has95Rule = true)
        {
            roundType = _roundType;
            outLimitType = _outlimitType;
            outLimit = _outLimit;
            has95Rule = _has95Rule;
            reversedBibs = _reversedBibs;
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
        public List<int> hillIds;
        public List<HillInfo> hillInfos;
        public List<Classification> classifications;
        public List<Event> events;
        public Calendar()
        {
            competitors = new List<Competitor>();
            teams = new List<Team>();
            // hills = new List<HillInfo>();
            hillIds = new List<int>();
            classifications = new List<Classification>();
            events = new List<Event>();
        }
    }

}