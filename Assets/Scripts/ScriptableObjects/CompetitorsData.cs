using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class CompetitorsData : ScriptableObject
    {
        public List<int> startList;
        public List<int> bibs;
    }
}