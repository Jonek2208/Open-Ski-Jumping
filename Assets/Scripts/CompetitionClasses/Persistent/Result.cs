using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CompCal
{
    [Serializable]
    public class JumpResults
    {
        public List<JumpResult> results = new List<JumpResult>();
    }

    [Serializable]
    public class Result
    {
        [SerializeField]
        private JumpResults[] results;
        [SerializeField]
        private float qualRankPoints;
        [SerializeField]
        private decimal[] totalResults;
        [SerializeField]
        private int[] bibs;
        [SerializeField]
        private int rank;
        [SerializeField]
        private float totalPoints;

        public JumpResults[] Results { get => results; set => results = value; }
        public decimal QualRankPoints { get => (decimal)qualRankPoints; set => qualRankPoints = (float)value; }
        public decimal[] TotalResults { get => totalResults; set => totalResults = value; }
        public int[] Bibs { get => bibs; set => bibs = value; }
        public int Rank { get => rank; set => rank = value; }
        public decimal TotalPoints { get => (decimal)totalPoints; set => totalPoints = (float)value; }
    }
}