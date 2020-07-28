﻿using NUnit.Framework;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;

namespace OpenSkiJumping.Tests
{
    public class Test
    {
        public class MockJumpData : IJumpData
        {
            public decimal Distance { get; set; }
            public decimal[] JudgesMarks { get; set; }
            public int GatesDiff { get; set; }
            public int InitGate { get; set; }
            public int Gate { get; set; }
            public decimal Wind { get; set; }
            public decimal Speed { get; set; }
        }

        [Test]
        public void JumpCalcTest_TailWind()
        {
            IJumpData jumpData = new MockJumpData
            {
                Speed = 95.5m,
                Distance = 120.5m,
                JudgesMarks = new[] {16.5m, 17.5m, 17.5m, 17.0m, 17.5m},
                GatesDiff = -2,
                Wind = -2.56m
            };

            var hillInfo = new HillInfo(123, 140, 5.5m, 6.66m, 4.2m, 0.79m);

            var jumpResult = EventProcessor.GetJumpResult(jumpData, hillInfo);

            Assert.AreEqual(55.5m, jumpResult.distancePoints);
            Assert.AreEqual(52.0m, jumpResult.judgesTotalPoints);
            Assert.AreEqual(11.9m, jumpResult.gatePoints);
            Assert.AreEqual(30.7m, jumpResult.windPoints);
            Assert.AreEqual(150.1m, jumpResult.totalPoints);
        }

        [Test]
        public void JumpCalcTest_HeadWind()
        {
            IJumpData jumpData = new MockJumpData
            {
                Speed = 84.4m,
                Distance = 103.0m,
                JudgesMarks = new[] {19.5m, 19.5m, 19.0m, 20.0m, 19.5m},
                GatesDiff = -1,
                Wind = 1.27m
            };

            var hillInfo = new HillInfo(90, 97, 4.0m, 4.84m, 3.5m, 0.49m);

            var jumpResult = EventProcessor.GetJumpResult(jumpData, hillInfo);

            Assert.AreEqual(86.0m, jumpResult.distancePoints);
            Assert.AreEqual(58.5m, jumpResult.judgesTotalPoints);
            Assert.AreEqual(3.4m, jumpResult.gatePoints);
            Assert.AreEqual(-10.2m, jumpResult.windPoints);
            Assert.AreEqual(137.7m, jumpResult.totalPoints);
        }
    }
}