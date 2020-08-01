using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;

namespace OpenSkiJumping.Tests
{
    public class Test
    {
        private class MockJumpData : IJumpData
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

        private static List<Participant> GetMockParticipantsIndividual(int count)
        {
            var res = new List<Participant>();
            for (var i = 0; i < count; i++)
                res.Add(new Participant {competitors = new List<int> {i}, id = i});
            return res;
        }

        private static List<Participant> GetMockParticipantsTeam(int count)
        {
            var res = new List<Participant>();
            for (var i = 0; i < count; i++)
                res.Add(new Participant {competitors = new List<int> {4 * i, 4 * i + 1, 4 * i + 2, 4 * i + 3}, id = i});
            return res;
        }


        private static EventRoundsInfo IndividualNormal()
        {
            return new EventRoundsInfo
            {
                name = "Individual2Rounds5",
                roundInfos =
                {
                    new RoundInfo
                    {
                        roundType = RoundType.Normal,
                        outLimitType = LimitType.Normal,
                        outLimit = 5,
                        useOrdRank = {false, false, false, false},
                        disableJudgesMarks = false,
                        has95Rule = true,
                        reversedBibs = false,
                        reassignBibs = false
                    },
                    new RoundInfo
                    {
                        roundType = RoundType.Normal,
                        outLimitType = LimitType.None,
                        outLimit = 0,
                        useOrdRank = {false, false, false, false},
                        disableJudgesMarks = false,
                        has95Rule = true,
                        reversedBibs = false,
                        reassignBibs = false
                    }
                }
            };
        }

        private static EventRoundsInfo TeamNormal()
        {
            return new EventRoundsInfo
            {
                name = "Team2Rounds3",
                roundInfos =
                {
                    new RoundInfo
                    {
                        roundType = RoundType.Normal,
                        outLimitType = LimitType.Normal,
                        outLimit = 3,
                        useOrdRank = {true, true, true, true},
                        disableJudgesMarks = false,
                        has95Rule = true,
                        reversedBibs = false,
                        reassignBibs = false
                    },
                    new RoundInfo
                    {
                        roundType = RoundType.Normal,
                        outLimitType = LimitType.None,
                        outLimit = 0,
                        useOrdRank = {true, true, true, false},
                        disableJudgesMarks = false,
                        has95Rule = true,
                        reversedBibs = false,
                        reassignBibs = false
                    }
                }
            };
        }

        private static EventRoundsInfo IndividualKO()
        {
            return new EventRoundsInfo
            {
                name = "Individual2Rounds5KO",
                roundInfos =
                {
                    new RoundInfo
                    {
                        roundType = RoundType.KO,
                        outLimitType = LimitType.Normal,
                        outLimit = 5,
                        useOrdRank = {false, false, false, false},
                        disableJudgesMarks = false,
                        has95Rule = true,
                        reversedBibs = true,
                        reassignBibs = false
                    },
                    new RoundInfo
                    {
                        roundType = RoundType.Normal,
                        outLimitType = LimitType.None,
                        outLimit = 0,
                        useOrdRank = {false, false, false, false},
                        disableJudgesMarks = false,
                        has95Rule = true,
                        reversedBibs = true,
                        reassignBibs = false
                    }
                }
            };
        }

        private static MockJumpData MockJumpDataFromDist(decimal dist)
        {
            return new MockJumpData
                {Distance = dist, JudgesMarks = new[] {18.0m, 18.0m, 18.0m, 18.0m, 18.0m}};
        }

        private static bool ListEqual(IList<int> a, IList<int> b)
        {
            if (a.Count != b.Count) return false;
            return !a.Where((t, i) => t != b[i]).Any();
        }

        private static void RegisterJumps(IResultsManager resultsManager, IEnumerable<decimal> distances)
        {
            foreach (var it in distances)
            {
                resultsManager.RegisterJump(MockJumpDataFromDist(it));
                resultsManager.JumpFinish();
            }
        }

        [Test]
        public void TestNormalCompetition()
        {
            var eventInfo = new EventInfo {eventType = EventType.Individual, roundInfos = IndividualNormal()};
            var hillInfo = new HillInfo(123, 140, 5.5m, 6.66m, 4.2m, 0.79m);

            var competitorsCount = 8;
            var orderedParticipants = GetMockParticipantsIndividual(competitorsCount);
            var resultsManager = new ResultsManager(eventInfo, orderedParticipants, hillInfo);
            resultsManager.RoundInit();
            resultsManager.SubroundInit();

            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Bibs[0]).ToList(),
                Enumerable.Range(1, competitorsCount).Reverse().ToList()));
            Assert.IsTrue(ListEqual(resultsManager.StartList,
                Enumerable.Range(0, competitorsCount).Reverse().ToList()));

            RegisterJumps(resultsManager, Enumerable.Repeat(123m, competitorsCount));

            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                Enumerable.Range(0, competitorsCount).ToList()));

            resultsManager.SubroundFinish();
            resultsManager.RoundInit();
            resultsManager.SubroundInit();
            Assert.IsTrue(ListEqual(resultsManager.StartList,
                Enumerable.Range(0, competitorsCount).Reverse().ToList()));
        }


        [Test]
        public void TestKOCompetition()
        {
            var eventInfo = new EventInfo {eventType = EventType.Individual, roundInfos = IndividualKO()};
            var hillInfo = new HillInfo(123, 140, 5.5m, 6.66m, 4.2m, 0.79m);

            const int competitorsCount = 8, competitorsCount2 = 5;
            ;
            var orderedParticipants = GetMockParticipantsIndividual(competitorsCount);
            var resultsManager = new ResultsManager(eventInfo, orderedParticipants, hillInfo);
            resultsManager.RoundInit();
            resultsManager.SubroundInit();

            // bibs & start list 1st round
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Bibs[0]).ToList(),
                Enumerable.Range(1, competitorsCount).ToList()));
            Assert.IsTrue(ListEqual(resultsManager.StartList, new List<int> {4, 3, 5, 2, 6, 1, 7, 0}));
            RegisterJumps(resultsManager, Enumerable.Repeat(123m, competitorsCount));

            // order 1st round
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                Enumerable.Range(0, competitorsCount).ToList()));

            resultsManager.SubroundFinish();
            resultsManager.RoundFinish();
            resultsManager.RoundInit();
            resultsManager.SubroundInit();

            // bibs & start list 2nd round
            Assert.IsTrue(
                ListEqual(resultsManager.StartList, Enumerable.Range(0, competitorsCount2).Reverse().ToList()));
            Assert.IsTrue(ListEqual(resultsManager.StartList.Select(it => resultsManager.Results[it].Bibs[1]).ToList(),
                resultsManager.StartList.Select(it => resultsManager.Results[it].Bibs[0]).ToList()));
            RegisterJumps(resultsManager, Enumerable.Repeat(123m, competitorsCount2));

            // order 2nd round
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount2).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                Enumerable.Range(0, competitorsCount2).ToList()));
        }

        [Test]
        public void TestKOCompetitionOdd()
        {
            var eventInfo = new EventInfo {eventType = EventType.Individual, roundInfos = IndividualKO()};
            var hillInfo = new HillInfo(123, 140, 5.5m, 6.66m, 4.2m, 0.79m);

            const int competitorsCount = 7, competitorsCount2 = 5;
            var orderedParticipants = GetMockParticipantsIndividual(competitorsCount);
            var resultsManager = new ResultsManager(eventInfo, orderedParticipants, hillInfo);
            resultsManager.RoundInit();
            resultsManager.SubroundInit();

            // bibs & start list 1st round
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Bibs[0]).ToList(),
                Enumerable.Range(1, competitorsCount).ToList()));
            Assert.IsTrue(ListEqual(resultsManager.StartList, new List<int> {4, 3, 5, 2, 6, 1, 0}));

            RegisterJumps(resultsManager, Enumerable.Repeat(123m, competitorsCount));

            // order 1st round
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                Enumerable.Range(0, competitorsCount).ToList()));

            resultsManager.SubroundFinish();
            resultsManager.RoundFinish();
            resultsManager.RoundInit();
            resultsManager.SubroundInit();

            // bibs & start list 2nd round

            Assert.IsTrue(
                ListEqual(resultsManager.StartList, Enumerable.Range(0, competitorsCount2).Reverse().ToList()));
            Assert.IsTrue(ListEqual(resultsManager.StartList.Select(it => resultsManager.Results[it].Bibs[1]).ToList(),
                resultsManager.StartList.Select(it => resultsManager.Results[it].Bibs[0]).ToList()));
            RegisterJumps(resultsManager, Enumerable.Repeat(123m, competitorsCount2));

            // order 2nd round
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount2).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                Enumerable.Range(0, competitorsCount2).ToList()));
        }

        [Test]
        public void TestTeamCompetition()
        {
            var eventInfo = new EventInfo {eventType = EventType.Team, roundInfos = TeamNormal()};
            var hillInfo = new HillInfo(123, 140, 5.5m, 6.66m, 4.2m, 0.79m);

            const int competitorsCount = 5, competitorsCount2 = 3;
            var orderedParticipants = GetMockParticipantsTeam(competitorsCount);
            var resultsManager = new ResultsManager(eventInfo, orderedParticipants, hillInfo);
            resultsManager.RoundInit();
            resultsManager.SubroundInit();

            // bibs 1st round
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Bibs[0]).ToList(),
                Enumerable.Range(1, competitorsCount).Reverse().ToList()));

            // 1st round 1st group
            Assert.IsTrue(ListEqual(resultsManager.StartList,
                Enumerable.Range(0, competitorsCount).Reverse().ToList()));
            RegisterJumps(resultsManager, Enumerable.Repeat(123m, competitorsCount));
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                Enumerable.Range(0, competitorsCount).ToList()));
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Rank).ToList(),
                new List<int> {1, 1, 1, 1, 1}));
            resultsManager.SubroundFinish();

            // 1st round 2nd group
            resultsManager.SubroundInit();
            Assert.IsTrue(ListEqual(resultsManager.StartList,
                Enumerable.Range(0, competitorsCount).Reverse().ToList()));
            RegisterJumps(resultsManager, new List<decimal> {123, 124, 125, 124, 123});
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                new List<int> {2, 1, 3, 0, 4}));
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Rank).ToList(),
                new List<int> {4, 2, 1, 2, 4}));
            resultsManager.SubroundFinish();

            // 1st round 3rd group
            resultsManager.SubroundInit();
            Assert.IsTrue(ListEqual(resultsManager.StartList,
                Enumerable.Range(0, competitorsCount).Reverse().ToList()));
            RegisterJumps(resultsManager, new List<decimal> {123, 124, 125, 126, 127});
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                Enumerable.Range(0, competitorsCount).ToList()));
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Rank).ToList(),
                new List<int> {1, 1, 1, 4, 5}));
            resultsManager.SubroundFinish();

            // start list 1st round 4th group
            resultsManager.SubroundInit();
            Assert.IsTrue(ListEqual(resultsManager.StartList,
                Enumerable.Range(0, competitorsCount).Reverse().ToList()));
            RegisterJumps(resultsManager, new List<decimal> {130, 123, 123, 124, 124});
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                new List<int> {4, 0, 1, 2, 3}));
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Rank).ToList(),
                new List<int> {2, 2, 4, 5, 1}));
            resultsManager.SubroundFinish();
            resultsManager.RoundFinish();
            resultsManager.RoundInit();
            resultsManager.SubroundInit();

            // bibs 2nd round
            Assert.IsTrue(resultsManager.StartList.Select(it => resultsManager.Results[it])
                .Any(it => it.Bibs[0] == it.Bibs[1]));

            // 2nd round 1st group
            Assert.IsTrue(ListEqual(resultsManager.StartList, new List<int> {4, 1, 0}));
            RegisterJumps(resultsManager, Enumerable.Repeat(123m, competitorsCount2));
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount2).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                new List<int> {4, 0, 1}));
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Rank).ToList(),
                new List<int> {2, 2, 4, 5, 1}));
            resultsManager.SubroundFinish();

            // 2nd round 2nd group
            resultsManager.SubroundInit();
            Assert.IsTrue(ListEqual(resultsManager.StartList, new List<int> {4, 1, 0}));
            RegisterJumps(resultsManager, Enumerable.Repeat(123m, competitorsCount2));
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount2).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                new List<int> {4, 0, 1}));
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Rank).ToList(),
                new List<int> {2, 2, 4, 5, 1}));
            resultsManager.SubroundFinish();

            // 2nd round 3rd group
            resultsManager.SubroundInit();
            Assert.IsTrue(ListEqual(resultsManager.StartList, new List<int> {4, 1, 0}));
            RegisterJumps(resultsManager, new List<decimal> {129, 131, 129});
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount2).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                new List<int> {1, 4, 0}));
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Rank).ToList(),
                new List<int> {3, 1, 4, 5, 1}));
            resultsManager.SubroundFinish();

            // 2nd round 4th group
            resultsManager.SubroundInit();
            Assert.IsTrue(ListEqual(resultsManager.StartList, new List<int> {0, 4, 1}));
            RegisterJumps(resultsManager, new List<decimal> {135, 131, 131});
            Assert.IsTrue(ListEqual(
                Enumerable.Range(0, competitorsCount2).Select(it => resultsManager.GetIdByRank(it)).ToList(),
                new List<int> {0, 1, 4}));
            Assert.IsTrue(ListEqual(resultsManager.Results.Select(it => it.Rank).ToList(),
                new List<int> {1, 2, 4, 5, 2}));
        }
    }
}