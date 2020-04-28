using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class ClassificationInfo
    {
        public string name = "";

        [JsonConverter(typeof(StringEnumConverter))]
        public EventType eventType;

        [JsonConverter(typeof(StringEnumConverter))]
        public ClassificationType classificationType;

        public TeamClassificationLimitType teamClassificationLimitType;
        public int teamCompetitorsLimit;

        public int medalPlaces;
        
        public List<int> events = new List<int>();
        public List<PointsTable> pointsTables = new List<PointsTable>();
        public string leaderBibColor = "ffff00";
    }

    public enum TeamClassificationLimitType
    {
        All,
        Best
    }

    [Serializable]
    public class PointsTable
    {
        public string name;
        public int[] value;
    }
}