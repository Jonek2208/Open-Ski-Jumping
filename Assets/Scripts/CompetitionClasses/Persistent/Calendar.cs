using System;
using System.Collections.Generic;

namespace CompCal
{

    [Serializable]
    public class Calendar
    {
        public string name;
        public DateTime lastModified;
        public List<string> competitorsIds;
        public List<Team> teams;
        public List<ClassificationInfo> classifications;
        public List<EventInfo> events;
        public Calendar()
        {
            competitorsIds = new List<string>();
            teams = new List<Team>();
            classifications = new List<ClassificationInfo>();
            events = new List<EventInfo>();
        }
    }
}