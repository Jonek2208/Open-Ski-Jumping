using CompCal;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeHillInfo")]
public class RuntimeHillInfo : ScriptableObject
{
    [SerializeField]
    private CompCal.HillInfo value;

    public HillInfo Value { get => value; set => this.value = value; }

    private decimal GetPointsPerMeter(decimal val)
    {
        if (val < 25) return 4.8m;
        if (val < 30) return 4.4m;
        if (val < 35) return 4.0m;
        if (val < 40) return 3.6m;
        if (val < 50) return 3.2m;
        if (val < 60) return 2.8m;
        if (val < 70) return 2.4m;
        if (val < 80) return 2.2m;
        if (val < 100) return 2.0m;
        if (val < 165) return 1.8m;
        return 1.2m;
    }

    private decimal GetKPointPoints(decimal val)
    {
        if (val < 165) return 60;
        return 120;
    }

    public void Init(decimal _kPoint, decimal _hs, decimal _gatePoints = 0, decimal _gatesSpacing = 0, decimal _headWindPoints = 0, decimal _tailWindPoints = 0)
    {
        value.KPoint = _kPoint;
        value.Hs = _hs;
        value.PointsPerMeter = GetPointsPerMeter(value.KPoint);
        value.KPointPoints = GetKPointPoints(value.KPoint);
        value.GateFactor = _gatePoints;
        value.GatesSpacing = _gatesSpacing;
        value.HeadWindFactor = _headWindPoints;
        value.TailWindFactor = _tailWindPoints;
    }
    public decimal GetDistancePoints(decimal distance) => value.KPointPoints + (distance - value.KPoint) * value.PointsPerMeter;
    public decimal GetWindPoints(decimal wind) => -wind * (wind >= 0 ? value.HeadWindFactor : value.TailWindFactor) * value.PointsPerMeter;
    public decimal GetGatePoints(int gateBefore, int gateAfter) => (gateBefore - gateAfter) * value.GatesSpacing * value.GateFactor * value.PointsPerMeter;
}
