using Competition;
using UnityEngine;

[CreateAssetMenu]
public class JumpResultVariable : ScriptableObject
{
    private Competition.Competitor value;

    public StringVariable competitorFirstName;
    public StringVariable competitorLastName;
    public StringVariable competitorCountryCode;
    public FloatVariable competitorResult;
    public IntVariable competitorRank;
    public IntVariable competitorBib;

    public void Set(Competition.Competitor competitor, int bib)
    {
        this.value = competitor;
        this.competitorFirstName.Value = this.value.firstName;
        this.competitorLastName.Value = this.value.lastName;
        this.competitorCountryCode.Value = this.value.countryCode;
        this.competitorBib.Value = bib;
    }

    public void SetResult(float result, int rank)
    {
        competitorResult.Value = result;
        competitorRank.Value = rank;
    }
}