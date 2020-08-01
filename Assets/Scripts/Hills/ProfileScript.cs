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
        public string name = "";
        public float terrainSteepness;
        public ProfileType type;

        public int gates;
        public float w;
        public float hn;
        public float gamma;
        public float alpha;
        public float e;
        public float es;
        public float t;
        public float r1;
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
        
        public ProfileData Clone()
        {
            var other =  (ProfileData) MemberwiseClone();
            other.name = string.Copy(name);
            return other;
        }
    }
}