using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class EventInfo
    {
        public List<int> classifications = new List<int>();

        [JsonConverter(typeof(StringEnumConverter))]
        public EventType eventType;

        public string hillId;

        public int inLimit;

        [JsonConverter(typeof(StringEnumConverter))]
        public LimitType inLimitType;

        public int ordRankId;

        [JsonConverter(typeof(StringEnumConverter))]
        public RankType ordRankType;

        public int qualRankId;

        [JsonConverter(typeof(StringEnumConverter))]
        public RankType qualRankType;

        public EventRoundsInfo roundInfos;
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