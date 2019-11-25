using CompCal;
using UnityEngine;

[CreateAssetMenu]
public class CompetitorVariable : ScriptableObject
{
    public GameEvent uiUpdateEvent;
    private CompCal.Competitor value;

    [SerializeField]
    private Label competitorName;

    [SerializeField]
    private Label countryCode;

    public Competitor Value
    {
        get => this.value;
        set
        {
            this.value = value;
            SetLabels();
            uiUpdateEvent.Raise();
        }
    }

    private void SetLabels()
    {
        this.competitorName.Value = this.Value.firstName + " " + this.Value.lastName.ToUpper();
        this.countryCode.Value = this.Value.countryCode;
    }

}