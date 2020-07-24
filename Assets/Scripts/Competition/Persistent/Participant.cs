using System;
using System.Collections.Generic;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class Participant
    {
        public int id;
        public int teamId;
        public List<int> competitors;
    }
}