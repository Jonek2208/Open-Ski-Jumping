using System;
using UnityEngine;

namespace OpenSkiJumping.Competition.Persistent
{
    public interface IHillInfo
    {
        void SetCompensations(float head, float tail, float gate);
        decimal GetDistancePoints(decimal distance);
        decimal GetWindPoints(decimal wind);
        decimal GetGatePoints(int gatesDiff);
    }

    [Serializable]
    public class HillInfo : IHillInfo
    {
        [SerializeField] private float k;
        [SerializeField] private float hs;
        [SerializeField] private float pointsPerMeter;
        [SerializeField] private float kPointPoints;
        [SerializeField] private float tailWindFactor;
        [SerializeField] private float headWindFactor;
        [SerializeField] private float gateFactor;
        [SerializeField] private float gatesSpacing;

        public decimal KPoint
        {
            get => (decimal) k;
            set => k = (float) value;
        }

        public decimal Hs
        {
            get => (decimal) hs;
            set => hs = (float) value;
        }

        public decimal PointsPerMeter
        {
            get => (decimal) pointsPerMeter;
            set => pointsPerMeter = (float) value;
        }

        public decimal KPointPoints
        {
            get => (decimal) kPointPoints;
            set => kPointPoints = (float) value;
        }

        public decimal TailWindFactor
        {
            get => (decimal) tailWindFactor;
            set => tailWindFactor = (float) value;
        }

        public decimal HeadWindFactor
        {
            get => (decimal) headWindFactor;
            set => headWindFactor = (float) value;
        }

        public decimal GateFactor
        {
            get => (decimal) gateFactor;
            set => gateFactor = (float) value;
        }

        public decimal GatesSpacing
        {
            get => (decimal) gatesSpacing;
            set => gatesSpacing = (float) value;
        }

        public HillInfo(decimal kPoint, decimal hs, decimal headWindFactor, decimal tailWindFactor, decimal gateFactor,
            decimal gatesSpacing)
        {
            KPoint = kPoint;
            Hs = hs;
            PointsPerMeter = EventProcessor.GetPointsPerMeter(KPoint);
            KPointPoints = EventProcessor.GetKPointPoints(KPoint);
            HeadWindFactor = headWindFactor;
            TailWindFactor = tailWindFactor;
            GateFactor = gateFactor;
            GatesSpacing = gatesSpacing;
        }

        public void SetCompensations(float head, float tail, float gate)
        {
            headWindFactor = (float) Math.Round(head, 2);
            tailWindFactor = (float) Math.Round(tail, 2);
            gateFactor = (float) Math.Round(gate, 2);
        }

        public decimal GetDistancePoints(decimal distance) =>
            Math.Round(KPointPoints + (distance - KPoint) * PointsPerMeter, 1);

        public decimal GetWindPoints(decimal wind) =>
            Math.Round(-wind * (wind >= 0 ? HeadWindFactor : TailWindFactor) * PointsPerMeter, 1);

        public decimal GetGatePoints(int gatesDiff) =>
            Math.Round(-gatesDiff * GatesSpacing * GateFactor * PointsPerMeter, 1);
    }
}