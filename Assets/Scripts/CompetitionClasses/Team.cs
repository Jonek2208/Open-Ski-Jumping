using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompCal
{
    [Serializable]
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
}
