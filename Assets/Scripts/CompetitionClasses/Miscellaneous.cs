using System;
using System.Collections.Generic;

namespace CompCal
{
    [Flags]
    public enum JumpResultState
    {
        None = 0,
        KoLoser = 1,
        LuckyLoser = 2
    }

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
    public enum EventType { Individual, Team }
    public enum ClassificationType { Place, Points }
    public enum RankType { None, Event, Classification }
}