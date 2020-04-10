using System;
using System.Collections.Generic;

namespace Competition
{

    [Serializable]
    public class Calendar
    {
        public string name;
        public List<string> competitorsIds = new List<string>();
        public List<Team> teams = new List<Team>();
        public List<ClassificationInfo> classifications = new List<ClassificationInfo>();
        public List<EventInfo> events=new List<EventInfo>();
    }
}