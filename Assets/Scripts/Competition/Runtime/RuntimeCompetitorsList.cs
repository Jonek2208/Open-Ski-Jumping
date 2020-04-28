using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeCompetitiorsList")]
    public class RuntimeCompetitorsList : ScriptableObject
    {
        public List<Competitor> competitors;
        public List<Team> teams;
    }
}
