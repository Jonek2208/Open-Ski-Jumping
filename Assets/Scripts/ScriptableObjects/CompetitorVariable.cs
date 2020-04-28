using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.ScriptableObjects.Variables;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
{
    [CreateAssetMenu]
    public class CompetitorVariable : ScriptableObject
    {
        private Competitor value;
        public StringVariable firstName;
        public StringVariable lastName;
        public StringVariable countryCode;
        public FloatVariable result;
        public IntVariable rank;
        public IntVariable bib;
        public JudgesMarkInfo[] judgesMarks;
        public FloatVariable speed;
        public FloatVariable distance;

        public void Set(Competitor competitor)
        {
            value = competitor;
            firstName.Value = value.firstName;
            lastName.Value = value.lastName;
            countryCode.Value = value.countryCode;
        }
    }
}