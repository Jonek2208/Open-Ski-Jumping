using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompetitionClasses
{
    enum Gender { Male, Female }
    enum LimitType { None, Normal, Exact }
    enum RoundType { Normal, KO }
    enum EventType { Qualification, Individual, Team }
    enum ClassificationType { IndividualPlace, IndividualPoints, TeamPlace, TeamPoints }
    enum RankType { None, Event, Classification }

    class Competitor
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
}