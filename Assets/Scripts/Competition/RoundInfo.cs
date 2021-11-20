using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenSkiJumping.Competition
{
    [Serializable]
    public class RoundInfo
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RoundType roundType;

        [JsonConverter(typeof(StringEnumConverter))]
        public LimitType outLimitType;

        public int outLimit;
        public List<bool> useOrdRank = new();
        public bool disableJudgesMarks;
        public bool has95Rule = true;
        public bool reversedBibs;
        public bool reassignBibs;
        public bool gateCompensation = true;
        public bool windCompensation = true;
    }
}