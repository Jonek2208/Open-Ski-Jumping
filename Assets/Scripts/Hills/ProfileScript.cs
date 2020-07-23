using System;
using System.Collections.Generic;

namespace OpenSkiJumping.Hills
{
    public enum ProfileType
    {
        ICR1992,
        ICR1996,
        ICR2008
    }

    [Serializable]
    public class ProfileData
    {
        public float a;
        public float alpha;
        public float b1;
        public float b2;
        public float betaA;
        public float betaK;
        public float betaL;
        public float betaP;
        public float bK;
        public float bU;
        public float d;
        public float e;
        public float es;
        public float gamma;

        public int gates;

        public bool gateStairsLeft;
        public bool gateStairsRight;
        public float hn;
        public float inrunStairsAngle;
        public bool inrunStairsLeft;
        public bool inrunStairsRight;
        public float l1;
        public float l2;
        public string name;
        public float q;
        public float r1;
        public float r2;
        public float r2L;
        public float rA;
        public float rL;
        public float s;
        public float t;
        public float terrainSteepness;
        public ProfileType type;
        public float w;
    }

    [Serializable]
    public class AllData
    {
        public List<ProfileData> profileData;
    }
}