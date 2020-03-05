using System;
using System.Collections.Generic;

namespace CompCal
{

    [Serializable]
    public class Calendar
    {
        public string name;
        public List<Competitor> competitors;
        public List<Team> teams;
        // public List<HillInfo> hills;
        public List<int> hillIds;
        public List<ClassificationInfo> classifications;
        public List<EventInfo> events;
        public Calendar()
        {
            competitors = new List<Competitor>();
            teams = new List<Team>();
            // hills = new List<HillInfo>();
            hillIds = new List<int>();
            classifications = new List<ClassificationInfo>();
            events = new List<EventInfo>();
        }
    }
}