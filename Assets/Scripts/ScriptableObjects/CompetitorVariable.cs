using CompCal;
using UnityEngine;

[CreateAssetMenu]
public class CompetitorVariable : ScriptableObject
{
    private CompCal.Competitor value;
    public StringVariable firstName;
    public StringVariable lastName;
    public StringVariable countryCode;
    public FloatVariable result;
    public IntVariable rank;
    public IntVariable bib;
    public JudgesMarkInfo[] judgesMarks;
    public FloatVariable speed;
    public FloatVariable distance;

    public void Set(CompCal.Competitor competitor)
    {
        this.value = competitor;
        this.firstName.Value = this.value.firstName;
        this.lastName.Value = this.value.lastName;
        this.countryCode.Value = this.value.countryCode;
    }
}