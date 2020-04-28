using System;
using System.Collections.Generic;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class ClassificationResults
    {
        public List<decimal> totalResults = new List<decimal>();
        public List<int> rank = new List<int>();
        public List<int> totalSortedResults = new List<int>();
    }
}