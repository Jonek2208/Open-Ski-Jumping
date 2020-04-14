using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Competition.Persistent
{
    [Serializable]
    public class ClassificationInfo
    {
        public string name;
        [JsonConverter(typeof(StringEnumConverter))]
        public EventType eventType;
        public ClassificationType classificationType;
        public List<int> events = new List<int>();
        public int teamCompetitorsLimit;
        public List<PointsTable> pointsTables = new List<PointsTable>();

        public ClassificationInfo(string _name, ClassificationType _classificationType)
        {
            name = _name;
            classificationType = _classificationType;
        }
    }

    [Serializable]
    public class PointsTable
    {
        public string name;
        public int[] value;
    }
}
