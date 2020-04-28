using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeResultsManager")]
    public class RuntimeResultsManager : ScriptableObject
    {
        public RuntimeEventInfo eventInfo;
        public RuntimeHillInfo hillInfo;
        public RuntimeJumpData jumpData;

        public Result[] results;
        public List<int> currentStartList;

        [SerializeField]
        private List<(int, int)> resultsWithRank;
        private int[] ordRankOrder;
        private SortedList<(int, decimal, int), int> finalResults;
        private SortedList<(decimal, int, int), int> allroundResults;
        public SortedList<(decimal, int, int), int> AllroundResults { get => allroundResults; }
        private SortedList<(decimal, int), int> losersResults;
        private int maxLosers;
        private int[] koState;
        public int[] lastRank;
        private int maxBib;
        private int competitorsCount;
        public int roundsCount;
        public int subroundsCount;

        public int startListIndex;
        public int roundIndex;
        public int subroundIndex;
        public bool resetOnStart;

        public void CompetitionInit()
        {
            if (resetOnStart)
            {
                startListIndex = 0;
                roundIndex = 0;
                subroundIndex = 0;
            }

            roundsCount = eventInfo.value.roundInfos.Count;
            subroundsCount = (eventInfo.value.eventType == EventType.Individual ? 1 : 4);

            competitorsCount = results.Length;

            lastRank = new int[competitorsCount];

            foreach (var item in results)
            {
                item.TotalResults = new decimal[subroundsCount];
                item.Results = new JumpResults[subroundsCount];
                for (int i = 0; i < subroundsCount; i++)
                {
                    item.Results[i] = new JumpResults();
                }

                item.Bibs = new int[roundsCount];
            }

            allroundResults = new SortedList<(decimal, int, int), int>();
            Comparison<(int, decimal, int)> comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? x.Item3.CompareTo(y.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1));
            Comparer<(int, decimal, int)> comparer = Comparer<(int, decimal, int)>.Create(comp);
            finalResults = new SortedList<(int, decimal, int), int>(comparer);
            // roundResults = new List<JumpResult>[competitorsList.Count];
            // totalResults = new decimal[competitorsList.Count];
            // rank = new int[competitorsList.Count];
            // bibs = new List<int>[competitorsList.Count];
            // lastRank = new int[competitorsList.Count];
            Comparison<(decimal, int, int)> comp2 = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? y.Item3.CompareTo(x.Item3) : x.Item2.CompareTo(y.Item2)) : y.Item1.CompareTo(x.Item1));
            allroundResults = new SortedList<(decimal, int, int), int>(Comparer<(decimal, int, int)>.Create(comp2));
            // losersResults = new SortedList<(int, decimal, int), int>();
            // startLists = new List<int>[roundsCount];

            //Temporary
            ordRankOrder = results.Select((item, index) => index).ToArray();
        }

        #region InitFunctions
        public void SubroundInit()
        {
            Debug.Log($"SUBROUND INIT: {subroundIndex}");

            RoundInfo currentRoundInfo = eventInfo.value.roundInfos[roundIndex];
            if (roundIndex > 0 || subroundIndex > 0)
            {
                if (currentRoundInfo.useOrdRank[subroundIndex])
                {
                    currentStartList = finalResults.Select(item => item.Value).OrderBy(item => item).ToList();
                }
                else
                {
                    currentStartList = finalResults.Select(item => item.Value).Reverse().ToList();
                }
            }
            else
            {
                currentStartList = Enumerable.Range(0, competitorsCount).ToList();
            }

            finalResults.Clear();
        }

        public void RoundInit()
        {
            Debug.Log($"ROUND INIT: {roundIndex}");

            //Temporary
            for (int i = 0; i < results.Length; i++)
            {
                results[i].Bibs[roundIndex] = i + 1;
            }
        }

        #endregion

        #region FinishFunctions
        public bool JumpFinish()
        {
            startListIndex++;
            if (startListIndex < currentStartList.Count) { return true; }
            return false;
        }

        public bool SubroundFinish()
        {
            Debug.Log($"SUBROUND FINISH: {subroundIndex}");

            lastRank = results.Select(item => item.Rank).ToArray();

            subroundIndex++;
            startListIndex = 0;
            if (subroundIndex < subroundsCount) { return true; }
            return false;
        }

        public bool RoundFinish()
        {
            Debug.Log($"ROUND FINISH: {subroundIndex}");

            //Calculate next round

            roundIndex++;
            subroundIndex = 0;
            if (roundIndex < roundsCount) { return true; }
            return false;
        }

        #endregion



        private void AddResult(int primaryIndex, int secondaryIndex, JumpResult jump)
        {
            int id = currentStartList[startListIndex];
            Debug.Assert(results[id].Results[secondaryIndex].results != null);
            results[id].Results[secondaryIndex].results.Add(jump);

            results[id].TotalResults[secondaryIndex] = results[id].Results[secondaryIndex].results.Sum(item => item.totalPoints);
            results[id].TotalPoints = results[id].TotalResults.Sum();
        }

        public void RegisterJump()
        {
            JumpResult jump = new JumpResult(jumpData.Distance, jumpData.JudgesMarks, jumpData.Gate, jumpData.Wind, jumpData.Speed);
            jump.distancePoints = hillInfo.GetDistancePoints(jump.distance);
            jump.windPoints = hillInfo.GetWindPoints(jump.wind);
            jump.gatePoints = hillInfo.GetGatePoints(0, jump.gatesDiff);
            jump.totalPoints = Math.Max(0, jump.distancePoints + jump.judgesTotalPoints + jump.windPoints + jump.gatePoints);
            if (roundIndex > 0 || subroundIndex > 0)
            {
                RemoveFromAllroundResults();
            }

            AddResult(startListIndex, subroundIndex, jump);
            DebugAllRoundResults();
            AddToAllroundResults();
            DebugAllRoundResults();
            AddToFinalResults();
            Debug.Log(results[currentStartList[startListIndex]].TotalPoints);
        }

        private void AddToFinalResults()
        {
            int id = currentStartList[startListIndex];
            int bib = results[id].Bibs[roundIndex];

            if (eventInfo.value.roundInfos[roundIndex].roundType == RoundType.KO && startListIndex % 2 == 1)
            {
                finalResults.Remove((0, id, bib));

                int id2 = currentStartList[startListIndex - 1];
                int bib2 = results[id2].Bibs[roundIndex];
                int loserId = id2, winnerId = id;

                if (results[id2].TotalPoints > results[id].TotalPoints)
                {
                    winnerId = id2;
                    loserId = id;
                }

                int loserBib = results[loserId].Bibs[roundIndex];
                int winnerBib = results[winnerId].Bibs[roundIndex];

                results[winnerId].Results[subroundIndex].results[roundIndex].state = JumpResultState.None;
                results[loserId].Results[subroundIndex].results[roundIndex].state = JumpResultState.KoLoser;

                finalResults.Add((0, results[winnerId].TotalPoints, winnerBib), winnerId);

                losersResults.Add((results[loserId].TotalPoints, loserBib), loserId);
                int loserRank = losersResults.IndexOfKey((results[loserId].TotalPoints, loserBib));
                if (loserRank < maxLosers)
                {
                    if (losersResults.Count > maxLosers)
                    {
                        var lastLoser = losersResults.Keys[maxLosers];
                        int lastLoserId = losersResults.Values[maxLosers];
                        koState[lastLoserId] = 1;
                        results[loserId].Results[subroundIndex].results[roundIndex].state = JumpResultState.KoLoser;
                        finalResults.Remove((0, lastLoser.Item1, lastLoser.Item2));
                        finalResults.Add((1, lastLoser.Item1, lastLoser.Item2), lastLoserId);
                    }
                    finalResults.Add((0, results[loserId].TotalPoints, loserBib), loserId);
                }
                else
                {
                    koState[loserId] = 1;
                    finalResults.Add((1, results[loserId].TotalPoints, loserBib), loserId);
                }
            }
            else
            {
                finalResults.Add((0, results[id].TotalPoints, bib), id);
                results[id].Results[subroundIndex].results[roundIndex].state = JumpResultState.Advanced;
            }
        }

        private void DebugAllRoundResults()
        {
            Debug.Log($"DEGUG ALLROUND RESULTS {startListIndex} {subroundIndex} {roundIndex}");
            foreach (var item in allroundResults)
            {
                Debug.Log($"{results[item.Value].Rank} {item.Key.Item1} {item.Key.Item2} {item.Key.Item3} {item.Value}");
            }
        }

        private void AddToAllroundResults()
        {
            int competitorId = currentStartList[startListIndex];
            int subroundNum = roundIndex * subroundsCount + subroundIndex;

            allroundResults.Add((results[competitorId].TotalPoints, subroundNum, results[competitorId].Bibs[roundIndex]), competitorId);

            // Update rank
            for (int i = 0; i < Math.Min(competitorsCount, allroundResults.Count); i++)
            {
                if (i > 0 && allroundResults.Keys[i].Item1 == allroundResults.Keys[i - 1].Item1)
                {
                    results[allroundResults.Values[i]].Rank = results[allroundResults.Values[i - 1]].Rank;
                }
                else
                {
                    results[allroundResults.Values[i]].Rank = i + 1;
                }
            }
        }

        private void RemoveFromAllroundResults()
        {
            int competitorId = currentStartList[startListIndex];
            int subroundNum = roundIndex * subroundsCount + subroundIndex - 1;
            int bibRoundIndex = (subroundIndex > 0 ? roundIndex : roundIndex - 1);
            allroundResults.Remove((results[competitorId].TotalPoints, subroundNum, results[competitorId].Bibs[bibRoundIndex]));
        }

        private int GetBibCode(int bib)
        {
            if (eventInfo.value.roundInfos[roundIndex].reversedBibs)
            {
                return maxBib - bib;
            }

            return bib;
        }


        private IResultsManager value;
        public void Initialize(EventInfo eventInfo, List<int> orderedParticipants, IHillInfo hillInfo)
        {
            value = new ResultsManager(eventInfo, orderedParticipants, hillInfo);
        }
    }
}