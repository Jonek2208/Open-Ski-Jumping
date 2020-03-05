using System;
using System.Collections.Generic;
using System.Linq;
using CompCal;
using UnityEngine;



[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeResultsManager")]
public class RuntimeResultsManager : ScriptableObject
{
    public RuntimeEventInfo eventInfo;
    public RuntimeHillInfo hillInfo;
    public RuntimeJumpData jumpData;

    public Result[] results;
    public List<int> currentStartList;

    private SortedList<(int, decimal, int), int> finalResults;
    private SortedList<(int, decimal, int), int> allroundResults;
    private SortedList<(decimal, int), int> losersResults;
    private int maxLosers;
    private int[] koState;
    private int maxBib;

    public int roundsCount;
    public int subroundsCount;

    public int currentStartListIndex;
    public int roundIndex;
    public int subroundIndex;

    public void CompetitionInit()
    {
        roundsCount = eventInfo.value.roundInfos.Count;
        subroundsCount = (eventInfo.value.eventType == CompCal.EventType.Individual ? 1 : 4);

        foreach (var item in results)
        {
            item.TotalResults = new decimal[subroundsCount];
            item.Results = new JumpResults[subroundsCount];
            for(int i = 0; i < subroundsCount; i++)
            {
                item.Results[i] = new JumpResults();
            }
            
            item.Bibs = new int[roundsCount];
        }

        allroundResults = new SortedList<(int, decimal, int), int>();
        Comparison<(int, decimal, int)> comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? x.Item3.CompareTo(y.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1));
        Comparer<(int, decimal, int)> comparer = Comparer<(int, decimal, int)>.Create(comp);
        finalResults = new SortedList<(int, decimal, int), int>(comparer);
        // roundResults = new List<JumpResult>[competitorsList.Count];
        // totalResults = new decimal[competitorsList.Count];
        // rank = new int[competitorsList.Count];
        // bibs = new List<int>[competitorsList.Count];
        // lastRank = new int[competitorsList.Count];
        Comparison<(int, decimal, int)> comp2 = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? y.Item3.CompareTo(x.Item3) : x.Item2.CompareTo(y.Item2)) : y.Item1.CompareTo(x.Item1));
        allroundResults = new SortedList<(int, decimal, int), int>(Comparer<(int, decimal, int)>.Create(comp2));
        // losersResults = new SortedList<(int, decimal, int), int>();
        // startLists = new List<int>[roundsCount];
        // for (int i = 0; i < competitorsList.Count; i++)
        // {
        //     roundResults[i] = new List<JumpResult>();
        //     bibs[i] = new List<int>();
        // }
    }

    public bool JumpFinish()
    {
        currentStartListIndex++;
        if (currentStartListIndex < currentStartList.Count) { return true; }
        return false;
    }

    public void RoundInit()
    {
        finalResults.Clear();
    }

    public bool RoundFinish()
    {
        roundIndex++;
        subroundIndex = 0;
        if (roundIndex < roundsCount) { return true; }
        return false;
    }

    public bool SubroundFinish()
    {
        subroundIndex++;
        currentStartListIndex = 0;
        if (subroundIndex < subroundsCount) { return true; }
        return false;
    }

    public void SubroundInit()
    {
        finalResults.Clear();
    }

    public void SetStartList(List<int> startList)
    {
        currentStartList = startList;
    }

    private void AddResult(int primaryIndex, int secondaryIndex, JumpResult jump)
    {
        int id = currentStartList[currentStartListIndex];
        Result result = results[id];
        Debug.Assert(result.Results[secondaryIndex].results != null);
        result.Results[secondaryIndex].results.Add(jump);
        result.TotalResults[secondaryIndex] = result.Results[secondaryIndex].results.Sum(item => item.totalPoints);
        result.TotalPoints = result.TotalResults.Sum();
    }

    public void RegisterJump()
    {
        JumpResult jump = new JumpResult(jumpData.Distance, jumpData.JudgesMarks, jumpData.Gate, jumpData.Wind, jumpData.Speed);
        jump.distancePoints = hillInfo.GetDistancePoints(jump.distance);
        jump.windPoints = hillInfo.GetWindPoints(jump.wind);
        jump.gatePoints = hillInfo.GetGatePoints(0, jump.gate);
        jump.totalPoints = jump.distancePoints + jump.judgesTotalPoints + jump.windPoints + jump.gatePoints;
        if (roundIndex > 0 || (roundIndex == 0 && subroundIndex > 0))
        {
            RemoveFromAllroundResults(currentStartList[currentStartListIndex]);
        }

        AddResult(currentStartListIndex, subroundIndex, jump);
        AddToAllroundResults();
        AddToFinalResults();
    }

    private void AddToFinalResults()
    {
        int id = currentStartList[currentStartListIndex];
        int bib = results[id].Bibs[roundIndex];

        if (eventInfo.value.roundInfos[roundIndex].roundType == RoundType.KO && currentStartListIndex % 2 == 1)
        {
            finalResults.Remove((0, id, bib));

            int id2 = currentStartList[currentStartListIndex - 1];
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

    private void AddToAllroundResults()
    {

    }

    private void RemoveFromAllroundResults(int index)
    {

    }

    private int GetBibCode(int bib)
    {
        if (eventInfo.value.roundInfos[roundIndex].reversedBibs)
        {
            return maxBib - bib;
        }

        return bib;
    }
}