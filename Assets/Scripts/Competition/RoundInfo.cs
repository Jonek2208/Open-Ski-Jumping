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
        public List<bool> useOrdRank = new List<bool> {false, false, false, false};
        public bool disableJudgesMarks = false;
        public bool has95Rule = true;
        public bool reversedBibs = false;
        public bool reassignBibs = false;
    }
}