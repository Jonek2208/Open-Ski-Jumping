using System;
using System.Collections.Generic;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public struct Podiums
    {
        public int first;
        public int second;
        public int third;

        public Podiums(int f = 0, int s = 0, int t = 0)
        {
            first = f;
            second = s;
            third = t;
        }

        public void Add(int place)
        {
            switch (place)
            {
                case 1:
                    first++;
                    break;
                case 2:
                    second++;
                    break;
                case 3:
                    third++;
                    break;
            }
        }
        
    }

    [Serializable]
    public class ClassificationResults
    {
        public List<decimal> totalResults = new List<decimal>();
        public List<int> rank = new List<int>();
        public List<int> totalSortedResults = new List<int>();
        public List<Podiums> podiums = new List<Podiums>();
    }
}