using System;
using System.Collections.Generic;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class Team
    {
        public string teamName = "";
        public string countryCode = "";
        public List<string> competitorsIds = new List<string>();
    }
}