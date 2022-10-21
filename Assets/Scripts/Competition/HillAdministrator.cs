using OpenSkiJumping.Data;
using UnityEngine;

namespace OpenSkiJumping.Competition
{
    public enum HillUsage
    {
        Training, Competition
    }
    public class HillAdministrator : MonoBehaviour
    {
        [SerializeField] private CompetitionRunner competitionRunner;
        [SerializeField] private MapXRuntime hillsRuntime;

        public HillUsage hillUsage;
        
        

    }
}