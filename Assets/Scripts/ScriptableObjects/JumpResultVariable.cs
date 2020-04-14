using Competition.Persistent;
using ScriptableObjects.Variables;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class JumpResultVariable : ScriptableObject
    {
        private Competitor value;

        public StringVariable competitorFirstName;
        public StringVariable competitorLastName;
        public StringVariable competitorCountryCode;
        public FloatVariable competitorResult;
        public IntVariable competitorRank;
        public IntVariable competitorBib;

        public void Set(Competitor competitor, int bib)
        {
            value = competitor;
            competitorFirstName.Value = value.firstName;
            competitorLastName.Value = value.lastName;
            competitorCountryCode.Value = value.countryCode;
            competitorBib.Value = bib;
        }

        public void SetResult(float result, int rank)
        {
            competitorResult.Value = result;
            competitorRank.Value = rank;
        }
    }
}