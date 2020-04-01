using System;
using System.Collections.Generic;
using System.Linq;

namespace Competition
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
        public int gate;
        public decimal gatePoints;
        public decimal wind;
        public decimal windPoints;
        public decimal speed;
        public JumpResultState state;
        public JumpResult()
        {
            judgesMask = new bool[5];
        }

        public JumpResult(decimal _distance, decimal[] _judgesMarks, int _gate, decimal _wind, decimal _speed)
        {
            // competitorId = _competitorId;
            distance = _distance;
            judgesMarks = _judgesMarks;
            judgesMask = new bool[judgesMarks.Length];
            gate = _gate;
            wind = _wind;
            speed = _speed;
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