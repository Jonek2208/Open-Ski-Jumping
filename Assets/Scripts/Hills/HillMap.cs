using System;
using System.Collections.Generic;

namespace OpenSkiJumping.Hills
{
    [Serializable]
    public class HillData
    {
        public float xPos, yPos, zPos;
        public float azimuth;
        public ProfileData profileData;
    }
    
    [Serializable]
    public class HillMap
    {
        public List<HillData> profiles;
        
    }
}