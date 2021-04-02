using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenSkiJumping.Hills
{
    public enum ProfileType
    {
        ICR1992,
        ICR1996,
        ICR2008,
        Finland
    }

    public enum InrunTrackType
    {
        SingleTrackGreen,
        SingleTrackPurple,
        SingleTrackRubber,
        SingleTrackGray,
        SnowTrack,
        SnowTrackRubber
    }

    [Serializable]
    public struct InrunData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public InrunTrackType summerTrack;
        [JsonConverter(typeof(StringEnumConverter))]
        public InrunTrackType winterTrack;
    }

    [Serializable]
    public struct LandingAreaData
    {
        public int metersLow;
        public int metersHigh;
        public int summerLinesSpacing;
        public int summerLinesLow;
        public int summerLinesHigh;
        public int winterLinesSpacing;
        public int winterLinesLow;
        public int winterLinesHigh;
    }

    [Serializable]
    public class ProfileData
    {
        public string name = "";
        public float terrainSteepness;
        public ProfileType type;

        public int gates;
        public float w;
        public float hS;
        public float h;
        public float n;
        public float gamma;
        public float alpha;
        public float e;
        public float es;
        public float t;
        public float r1;
        public float beta0;
        public float betaP;
        public float betaK;
        public float betaL;
        public float s;
        public float l1;
        public float l2;
        public float rL;
        public float r2L;
        public float r2;

        public float a;
        public float rA;
        public float betaA;
        public float b1;
        public float b2;
        public float bK;
        public float bU;
        public float d;
        public float q;

        public bool gateStairsLeft = true;
        public bool gateStairsRight = true;
        public bool inrunStairsLeft = true;
        public bool inrunStairsRight = true;
        public float inrunStairsAngle = 0.001f;
        public InrunData inrunData;
        public LandingAreaData landingAreaData;

        public ProfileData Clone()
        {
            var other = (ProfileData) MemberwiseClone();
            other.name = string.Copy(name);
            return other;
        }
    }
}