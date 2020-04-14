using System.Collections.Generic;
using Competition.Persistent;
using UnityEngine;

namespace Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeCompetitiorsList")]
    public class RuntimeCompetitorsList : ScriptableObject
    {
        public List<Competitor> competitors;
        public List<Team> teams;
    }
}
