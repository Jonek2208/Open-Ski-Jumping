using System;

namespace CompCal
{

    [Serializable]
    public class HillInfo
    {
        public decimal kPoint;
        public decimal hs;
        public decimal pointsPerMeter;
        public decimal kPointPoints;
        public decimal tailWindPoints;
        public decimal headWindPoints;
        public decimal gatePoints;
        public decimal gatesSpacing;
        public HillInfo() { }
        public HillInfo(decimal _kPoint, decimal _hs)
        {
            kPoint = _kPoint;
            hs = _hs;
            decimal ptsPerMeter = 1.2m, kPointPts = (kPoint < 165m ? 60m : 120m);
            if (kPoint < 25m) ptsPerMeter = 4.8m;
            else if (kPoint < 30m) ptsPerMeter = 4.4m;
            else if (kPoint < 35m) ptsPerMeter = 4.0m;
            else if (kPoint < 40m) ptsPerMeter = 3.6m;
            else if (kPoint < 50m) ptsPerMeter = 3.2m;
            else if (kPoint < 60m) ptsPerMeter = 2.8m;
            else if (kPoint < 70m) ptsPerMeter = 2.4m;
            else if (kPoint < 80m) ptsPerMeter = 2.2m;
            else if (kPoint < 100m) ptsPerMeter = 2.0m;
            else if (kPoint < 165m) ptsPerMeter = 1.8m;
            else ptsPerMeter = 1.2m;
            kPointPoints = kPointPts;
            pointsPerMeter = ptsPerMeter;
        }
        public HillInfo(decimal _kPoint, decimal _hs, decimal _pointsPerMeter, decimal _kPointPoints, decimal _gatePoints = 0, decimal _gatesSpacing = 0, decimal _headWindPoints = 0, decimal _tailWindPoints = 0)
        {
            kPoint = _kPoint;
            hs = _hs;
            pointsPerMeter = _pointsPerMeter;
            kPointPoints = _kPointPoints;
            tailWindPoints = _tailWindPoints;
            headWindPoints = _headWindPoints;
            gatePoints = _gatePoints;
            gatesSpacing = _gatesSpacing;
        }
        public decimal DistancePoints(decimal distance) => kPointPoints + (distance - kPoint) * pointsPerMeter;
        public decimal WindPoints(decimal wind) => -wind * (wind >= 0 ? headWindPoints : tailWindPoints);
        public decimal GatePoints(int gateBefore, int gateAfter) => (gateBefore - gateAfter) * gatesSpacing * gatePoints;
    }

}
