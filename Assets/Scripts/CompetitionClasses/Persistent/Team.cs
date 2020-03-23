using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompCal
{
    [Serializable]
    public class Team
    {
        public string teamName;
        public string countryCode;
        public List<string> competitorsIds;
        public Team(string _countryCode)
        {
            countryCode = _countryCode;
            competitorsIds = new List<string>();
        }
    }
}
