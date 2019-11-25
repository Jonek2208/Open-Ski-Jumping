using System;
using System.Collections.Generic;

namespace CompCal
{
    [Serializable]
    public class Country
    {
        public string ioc;
        public string alpha2;
        public string en;
    }

    [Serializable]
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
}