using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Competition.Persistent
{
    [Serializable]
    public class Competitor
    {
        public string id = "";
        public string lastName = "";
        public string firstName = "";
        public string countryCode = "";
        public int teamId;

        [JsonConverter(typeof(StringEnumConverter))]
        public Gender gender;

        public DateTime birthdate = new DateTime(1999, 1, 1);
        public string imagePath = "";
        public string helmetColor = "";
        public string suitTopFrontColor = "000000";
        public string suitTopBackColor = "000000";
        public string suitBottomFrontColor = "000000";
        public string suitBottomBackColor = "000000";
        public string skisColor = "000000";
    }
}