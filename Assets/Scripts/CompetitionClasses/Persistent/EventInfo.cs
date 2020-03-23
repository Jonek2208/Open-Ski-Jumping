using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompCal
{
    [Serializable]
    public class EventInfo
    {
        public string name;
        public string hillId;
        [JsonConverter(typeof(StringEnumConverter))] public EventType eventType;
        [JsonConverter(typeof(StringEnumConverter))] public RankType qualRankType;
        public int qualRankId;
        [JsonConverter(typeof(StringEnumConverter))] public LimitType inLimitType;
        public int inLimit;
        [JsonConverter(typeof(StringEnumConverter))] public RankType ordRankType;
        public int ordRankId;
        public List<RoundInfo> roundInfos;
        public List<int> classifications;
        public EventInfo(string _hillId, EventType _eventType, List<RoundInfo> _roundInfos, List<int> _classifications, RankType _qualRankType, int _qualRankId, RankType _ordRankType, int _ordRankId, LimitType _inLimitType = LimitType.None, int _inLimit = 0)
        {
            hillId = _hillId;
            roundInfos = _roundInfos;
            classifications = _classifications;
            qualRankType = _qualRankType;
            qualRankId = _qualRankId;
            ordRankType = _ordRankType;
            ordRankId = _ordRankId;
            inLimitType = _inLimitType;
            inLimit = _inLimit;
        }
    }
} 