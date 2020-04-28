using System.Collections.Generic;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
{
    [CreateAssetMenu]
    public class CompetitorsData : ScriptableObject
    {
        public List<int> startList;
        public List<int> bibs;
    }
}