using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class EventInfo
    {
        public int id;

        public List<int> classifications = new List<int>();

        [JsonConverter(typeof(StringEnumConverter))]
        public EventType eventType;

        public string hillId;
        public EventRoundsInfo roundInfos;

        #region OrdRank

        [JsonConverter(typeof(StringEnumConverter))]
        public RankType ordRankType;

        public int ordRankId;

        #endregion

        #region QualRank

        [JsonConverter(typeof(StringEnumConverter))]
        public RankType qualRankType;

        public int qualRankId;

        [JsonConverter(typeof(StringEnumConverter))]
        public LimitType inLimitType;

        public int inLimit;

        #endregion

        #region PreQualRank

        [JsonConverter(typeof(StringEnumConverter))]
        public RankType preQualRankType;

        public int preQualRankId;

        [JsonConverter(typeof(StringEnumConverter))]
        public LimitType preQualLimitType;

        public int preQualLimit;

        #endregion
    }

    [Serializable]
    public class EventRoundsInfo
    {
        public string name = "";
        public List<RoundInfo> roundInfos = new List<RoundInfo>();
        [JsonIgnore] public int Count => roundInfos.Count;

        [JsonIgnore]
        public RoundInfo this[int index]
        {
            get => roundInfos[index];
            set => roundInfos[index] = value;
        }
    }
}