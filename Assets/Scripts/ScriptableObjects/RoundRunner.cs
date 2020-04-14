using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class RoundRunner : ScriptableObject
    {
        public List<int> startList;
        public List<int> bibs;
    }
}