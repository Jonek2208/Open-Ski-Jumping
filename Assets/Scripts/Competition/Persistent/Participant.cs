using System;
using System.Collections.Generic;

namespace Competition.Persistent
{
    [Serializable]
    public class Participant
    {
        public int id;
        public List<int> competitors;
    }
}
