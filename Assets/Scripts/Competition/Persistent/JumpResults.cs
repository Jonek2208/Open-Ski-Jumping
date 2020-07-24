using System;

namespace OpenSkiJumping.Competition.Persistent
{
    [Serializable]
    public class JumpResult
    {
        public decimal totalPoints;
        public decimal distance;
        public decimal distancePoints;
        public decimal[] judgesMarks;
        public bool[] judgesMask;
        public decimal judgesTotalPoints;
        public int gatesDiff;
        public decimal gatePoints;
        public decimal wind;
        public decimal windPoints;
        public decimal speed;
        public JumpResultState state;

        public JumpResult()
        {
        }

        public JumpResult(decimal distance, decimal[] judgesMarks, int gate, decimal wind, decimal speed)
        {
            // competitorId = _competitorId;
            this.distance = distance;
            this.judgesMarks = judgesMarks;
            judgesMask = new bool[this.judgesMarks.Length];
            gatesDiff = gate;
            this.wind = wind;
            this.speed = speed;
            CalculateJudgesMarks();
        }

        public void CalculateJudgesMarks()
        {
            // If all values are equal then there could be a problem, but then mn will stay 0 and mx will stay 1
            int mn = 0, mx = 1;
            for (int i = 0; i < judgesMarks.Length; i++)
            {
                if (judgesMarks[mn] < judgesMarks[i]) mn = i;
                if (judgesMarks[mx] > judgesMarks[i]) mx = i;
                judgesTotalPoints += judgesMarks[i];
                judgesMask[i] = true;
            }

            judgesMask[mn] = judgesMask[mx] = false;
            judgesTotalPoints -= judgesMarks[mn] + judgesMarks[mx];
        }
    }
}