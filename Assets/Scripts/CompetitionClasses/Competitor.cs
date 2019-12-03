using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompCal
{
    [Serializable]
    public class Competitor
    {
        public string lastName;
        public string firstName;
        public string countryCode;
        [JsonConverter(typeof(StringEnumConverter))]
        public Gender gender;
        public DateTime birthdate;
        public string helmetColor;
        public string suitTopFrontColor;
        public string suitTopBackColor;
        public string suitBottomFrontColor;
        public string suitBottomBackColor;
        public string skisColor;
        public Competitor() { }
        public Competitor(string _lastName, string _firstName, string _countryCode, Gender _gender, int _year, int _month, int _day,
        string _helmetColor = "000000", string _suitTopFrontColor = "000000", string _suitTopBackColor = "000000", string _suitBottomFrontColor = "000000", string _suitBottomBackColor = "000000", string _skisColor = "000000")
        {
            lastName = _lastName;
            firstName = _firstName;
            countryCode = _countryCode;
            gender = _gender;
            birthdate = new DateTime(_year, _month, _day);
            helmetColor = _helmetColor;
            suitTopFrontColor = _suitTopFrontColor;
            suitTopBackColor = _suitTopBackColor;
            suitBottomFrontColor = _suitBottomFrontColor;
            suitBottomBackColor = _suitBottomBackColor;
            skisColor = _skisColor;
        }
    }

}
