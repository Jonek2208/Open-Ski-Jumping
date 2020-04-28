using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class EventInfo
    {
        public string name;
        public string hillId;

        [JsonConverter(typeof(StringEnumConverter))]
        public EventType eventType;

        [JsonConverter(typeof(StringEnumConverter))]
        public RankType qualRankType;

        public string qualRankId;

        [JsonConverter(typeof(StringEnumConverter))]
        public LimitType inLimitType;

        public int inLimit;

        [JsonConverter(typeof(StringEnumConverter))]
        public RankType ordRankType;

        public string ordRankId;
        public EventRoundsInfo roundInfos;
        
        public List<ClassificationInfo> classifications;
    }

    [Serializable]
    public class EventRoundsInfo
    {
        public string name = "";
        public List<RoundInfo> roundInfos = new List<RoundInfo>();
        public int Count => roundInfos.Count;
        
        public RoundInfo this[int index]
        {
            get => roundInfos[index];
            set => roundInfos[index] = value;
        }
    }
}