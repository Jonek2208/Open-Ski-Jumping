using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeHillInfo")]
    public class RuntimeHillInfo : ScriptableObject
    {
        [SerializeField]
        private HillInfo value;

        public HillInfo Value { get => value; set => this.value = value; }

        private static decimal GetPointsPerMeter(decimal val)
        {
            return val switch
            {
                < 25 => 4.8m,
                < 30 => 4.4m,
                < 35 => 4.0m,
                < 40 => 3.6m,
                < 50 => 3.2m,
                < 60 => 2.8m,
                < 70 => 2.4m,
                < 80 => 2.2m,
                < 100 => 2.0m,
                < 165 => 1.8m,
                _ => 1.2m
            };
        }

        private decimal GetKPointPoints(decimal val)
        {
            return val < 165 ? 60 : 120;
        }

        public void Init(decimal kPoint, decimal hs, decimal gatePoints = 0, decimal gatesSpacing = 0, decimal headWindPoints = 0, decimal tailWindPoints = 0)
        {
            value.KPoint = kPoint;
            value.Hs = hs; 
            value.PointsPerMeter = GetPointsPerMeter(value.KPoint);
            value.KPointPoints = GetKPointPoints(value.KPoint);
            value.GateFactor = gatePoints;
            value.GatesSpacing = gatesSpacing;
            value.HeadWindFactor = headWindPoints;
            value.TailWindFactor = tailWindPoints;
        }
        public decimal GetDistancePoints(decimal distance) => value.KPointPoints + (distance - value.KPoint) * value.PointsPerMeter;
        public decimal GetWindPoints(decimal wind) => -wind * (wind >= 0 ? value.HeadWindFactor : value.TailWindFactor) * value.PointsPerMeter;
        public decimal GetGatePoints(int gateBefore, int gateAfter) => (gateBefore - gateAfter) * value.GatesSpacing * value.GateFactor * value.PointsPerMeter;
    }
}
