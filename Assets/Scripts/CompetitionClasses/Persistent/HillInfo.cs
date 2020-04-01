using System;
using UnityEngine;

namespace Competition
{

    [Serializable]
    public class HillInfo
    {
        [SerializeField]
        private float k; 
        [SerializeField]
        private float hs;
        [SerializeField]
        private float pointsPerMeter;
        [SerializeField]
        private float kPointPoints;
        [SerializeField]
        private float tailWindFactor;
        [SerializeField]
        private float headWindFactor;
        [SerializeField]
        private float gateFactor;
        [SerializeField]
        private float gatesSpacing;

        public decimal KPoint { get => (decimal)k; set => k = (float)value; }
        public decimal Hs { get => (decimal)hs; set => hs = (float)value; }
        public decimal PointsPerMeter { get => (decimal)pointsPerMeter; set => pointsPerMeter = (float)value; }
        public decimal KPointPoints { get => (decimal)kPointPoints; set => kPointPoints = (float)value; }
        public decimal TailWindFactor { get => (decimal)tailWindFactor; set => tailWindFactor = (float)value; }
        public decimal HeadWindFactor { get => (decimal)headWindFactor; set => headWindFactor = (float)value; }
        public decimal GateFactor { get => (decimal)gateFactor; set => gateFactor = (float)value; }
        public decimal GatesSpacing { get => (decimal)gatesSpacing; set => gatesSpacing = (float)value; }
    }
}
