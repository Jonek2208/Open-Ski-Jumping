using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CompCal
{
    [Serializable]
    public class ResultsContainer
    {
        public List<HillInfo> hillInfos;
        public List<HillProfile.ProfileData> hillProfiles;
        public int eventIt;
        public EventResults[] eventResults;
        public ClassificationResults[] classificationResults;

        // [JsonIgnore]
        // public EventInfo CurrentEvent
        // {
        //     get => calendar.events[eventIt];
        //     set => calendar.events[eventIt] = value;
        // }
        // [JsonIgnore]
        // public EventResults CurrentEventResults
        // {
        //     get => eventResults[eventIt];
        //     set => eventResults[eventIt] = value;
        // }
        // [JsonIgnore]
        // public HillInfo CurrentHillInfo
        // {
        //     get => hillInfos[calendar.events[eventIt].hillId];
        //     set => hillInfos[calendar.events[eventIt].hillId] = value;
        // }
        // [JsonIgnore]
        // public RoundInfo CurrentRoundInfo
        // {
        //     get => calendar.events[eventIt].roundInfos[roundIt];
        //     set => calendar.events[eventIt].roundInfos[roundIt] = value;
        // }
        // [JsonIgnore]
        // public List<int> CurrentStartList
        // {
        //     get => eventResults[eventIt].startLists[roundIt];
        //     set => eventResults[eventIt].startLists[roundIt] = value;
        // }
        // [JsonIgnore]
        // public Competitor CurrentCompetitor
        // {
        //     get => calendar.competitors[eventResults[eventIt].participants[eventResults[eventIt].startLists[roundIt][jumpIt]]];
        // }

        // private List<int> GetCompetitorsFromQualRank(EventInfo e)
        // {
        //     List<Tuple<decimal, int>> tempList = new List<Tuple<decimal, int>>();
        //     List<int> competitorsList = new List<int>();

        //     switch (e.qualRankType)
        //     {
        //         case RankType.None:
        //             for (int i = 0; i < calendar.competitors.Count; i++) { tempList.Add(Tuple.Create(0m, i)); }
        //             break;
        //         case RankType.Event:
        //             EventResults qualRankEvent = eventResults[e.qualRankId];
        //             tempList = qualRankEvent.finalResults.Select(it => Tuple.Create(it.Key.Item2, qualRankEvent.participants[it.Value])).ToList();
        //             break;
        //         case RankType.Classification:
        //             ClassificationResults qualRankClassification = classificationResults[e.qualRankId];
        //             tempList = qualRankClassification.totalSortedResults.Select(it => Tuple.Create(it.Key.Item1, it.Value)).ToList();
        //             break;
        //     }

        //     switch (e.inLimitType)
        //     {
        //         case LimitType.None:
        //             competitorsList = tempList.Select(it => it.Item2).ToList();
        //             break;
        //         case LimitType.Normal:
        //             decimal minPoints = tempList[Math.Min(tempList.Count, e.inLimit) - 1].Item1;
        //             competitorsList = tempList.Where(it => it.Item1 >= minPoints).Select(it => it.Item2).ToList();
        //             break;
        //         case LimitType.Exact:
        //             int cnt = Math.Min(tempList.Count, e.inLimit);
        //             competitorsList = tempList.Take(cnt).Select(it => it.Item2).ToList();
        //             break;
        //     }
        //     return competitorsList;
        // }

        // public void EventInit()
        // {
        //     List<int> competitorsList = GetCompetitorsFromQualRank(CurrentEvent);

        //     CurrentEventResults = new EventResults(competitorsList, CurrentEvent.roundInfos.Count);

        //     Dictionary<int, int> map = new Dictionary<int, int>();
        //     int[] lut = new int[competitorsList.Count];
        //     if (CurrentEvent.ordRankType == RankType.Event)
        //     {
        //         for (int i = 0; i < competitorsList.Count; i++) { map[competitorsList[i]] = i; }
        //         for (int i = 0; i < competitorsList.Count; i++) { lut[i] = -1; }

        //         for (int i = 0; i < eventResults[CurrentEvent.ordRankId].participants.Count; i++)
        //         {
        //             if (map.ContainsKey(eventResults[CurrentEvent.ordRankId].participants[i]))
        //             {
        //                 lut[map[eventResults[CurrentEvent.ordRankId].participants[i]]] = i;
        //             }
        //         }
        //     }

        //     List<Tuple<decimal, int>> ordArr = new List<Tuple<decimal, int>>();
        //     for (int i = 0; i < competitorsList.Count; i++)
        //     {
        //         decimal key = i;
        //         int bib = i;
        //         if (CurrentEvent.ordRankType == RankType.Event)
        //         {
        //             if (lut[i] == -1)
        //             {
        //                 key = 0;
        //                 bib = 0;
        //             }
        //             else
        //             {
        //                 key = eventResults[CurrentEvent.ordRankId].totalResults[lut[i]];
        //                 if (calendar.events[CurrentEvent.ordRankId].roundInfos[calendar.events[CurrentEvent.ordRankId].roundInfos.Count - 1].reversedBibs)
        //                 {
        //                     bib = eventResults[CurrentEvent.ordRankId].participants.Count + 1
        //                         - eventResults[CurrentEvent.ordRankId].bibs[lut[i]][eventResults[CurrentEvent.ordRankId].bibs[lut[i]].Count - 1];
        //                 }
        //                 else
        //                 {
        //                     bib = eventResults[CurrentEvent.ordRankId].bibs[lut[i]][eventResults[CurrentEvent.ordRankId].bibs[lut[i]].Count - 1];
        //                 }
        //             }
        //         }
        //         else if (CurrentEvent.ordRankType == RankType.Classification)
        //         {
        //             key = classificationResults[CurrentEvent.ordRankId].totalResults[competitorsList[i]];
        //         }

        //         CurrentEventResults.finalResults.Add(Tuple.Create(0, key, bib), i);
        //     }
        // }

        // public void RoundInit()
        // {
        //     CurrentStartList = new List<int>();

        //     switch (CurrentRoundInfo.roundType)
        //     {
        //         case RoundType.Normal:
        //             CurrentStartList = CurrentEventResults.finalResults.Values.Select(it => it).Reverse().ToList();
        //             // for (int i = CurrentEventResults.finalResults.Count - 1; i >= 0; i--)
        //             // { CurrentStartList.Add(CurrentEventResults.finalResults.Values[i]); }
        //             break;
        //         case RoundType.KO:
        //             int len = CurrentEventResults.finalResults.Count;
        //             for (int i = 0; i < len / 2; i++)
        //             {
        //                 CurrentStartList.Add(CurrentEventResults.finalResults.Values[(len + 1) / 2 + i]);
        //                 CurrentStartList.Add(CurrentEventResults.finalResults.Values[(len + 1) / 2 - i - 1]);
        //             }
        //             if (len % 2 == 1)
        //             { CurrentStartList.Add(CurrentEventResults.finalResults.Values[0]); }

        //             CurrentEventResults.koState = new int[CurrentEventResults.participants.Count];
        //             CurrentEventResults.maxLosers = Math.Max(0, CurrentRoundInfo.outLimit - (CurrentStartList.Count + 1) / 2);

        //             Comparison<Tuple<decimal, int>> compLoser = (x, y) => (x.Item1 == y.Item1 ? x.Item2.CompareTo(y.Item2) : y.Item1.CompareTo(x.Item1));
        //             CurrentEventResults.losersResults = new SortedList<Tuple<decimal, int>, int>(Comparer<Tuple<decimal, int>>.Create(compLoser));
        //             break;
        //     }

        //     jumpIt = 0;

        //     for (int i = 0; i < CurrentEventResults.finalResults.Count; i++)
        //     {
        //         int it = CurrentEventResults.finalResults.Values[i];
        //         if (roundIt == 0 || CurrentRoundInfo.reassignBibs)
        //         {
        //             if (CurrentRoundInfo.reversedBibs)
        //             { CurrentEventResults.bibs[it].Add(i + 1); }
        //             else
        //             { CurrentEventResults.bibs[it].Add(CurrentEventResults.finalResults.Count - i); }
        //         }
        //         else
        //         {
        //             int lastRoundBib = CurrentEventResults.bibs[it][roundIt - 1];
        //             CurrentEventResults.bibs[it].Add(lastRoundBib);
        //         }
        //     }

        //     Comparison<Tuple<int, decimal, int>> comp;
        //     if (CurrentRoundInfo.reversedBibs)
        //     { comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? x.Item3.CompareTo(y.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1)); }
        //     else
        //     { comp = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? y.Item3.CompareTo(x.Item3) : y.Item2.CompareTo(x.Item2)) : x.Item1.CompareTo(y.Item1)); }
        //     Comparer<Tuple<int, decimal, int>> comparer = Comparer<Tuple<int, decimal, int>>.Create(comp);
        //     CurrentEventResults.finalResults = new SortedList<Tuple<int, decimal, int>, int>(comparer);
        // }

        // public void RecalculateFinalResults()
        // {
        //     CurrentEventResults.lastRank = CurrentEventResults.rank.Select(it => it).ToArray();

        //     switch (CurrentEvent.roundInfos[roundIt].outLimitType)
        //     {
        //         case LimitType.Normal:
        //             if (CurrentRoundInfo.roundType == RoundType.KO)
        //             {
        //                 int it = Math.Min(CurrentEvent.roundInfos[roundIt].outLimit, CurrentEventResults.finalResults.Count);
        //                 while (it < CurrentEventResults.finalResults.Count && CurrentEventResults.finalResults.Keys[it].Item1 == 0 &&
        //                     CurrentEventResults.finalResults.Keys[it - 1].Item2 == CurrentEventResults.finalResults.Keys[it].Item2)
        //                 {
        //                     it++;
        //                 }
        //                 int stop = it;
        //                 it = CurrentEventResults.finalResults.Count - 1;
        //                 while (stop <= it)
        //                 {
        //                     CurrentEventResults.finalResults.RemoveAt(it);
        //                     it--;
        //                 }
        //             }
        //             else
        //             {
        //                 decimal minPoints = CurrentEventResults.finalResults.Keys[Math.Min(CurrentEventResults.finalResults.Count - 1, CurrentEvent.roundInfos[roundIt].outLimit - 1)].Item2;
        //                 for (int i = CurrentEventResults.finalResults.Count - 1; i >= 0; i--)
        //                 {
        //                     if (CurrentEventResults.finalResults.Keys[i].Item2 < minPoints)
        //                     {
        //                         CurrentEventResults.finalResults.RemoveAt(i);
        //                     }
        //                 }
        //             }

        //             break;
        //         case LimitType.Exact:
        //             int index = Math.Min(CurrentEvent.roundInfos[roundIt].outLimit, CurrentEventResults.finalResults.Count);
        //             for (int i = CurrentEventResults.finalResults.Count - 1; i >= index; i--)
        //             {
        //                 CurrentEventResults.finalResults.RemoveAt(i);
        //             }
        //             break;
        //     }

        // }

        // public int AddJump(JumpResult jmp, out int rank1, out decimal pts1, out int rank2, out decimal pts2)
        // {
        //     jmp.distancePoints = CurrentHillInfo.DistancePoints(jmp.distance);
        //     jmp.totalPoints = Math.Max(0m, jmp.judgesTotalPoints + jmp.distancePoints);
        //     CurrentEventResults.AddResult(CurrentStartList[jumpIt], jmp);
        //     if (CurrentRoundInfo.roundType == RoundType.KO)
        //     {
        //         if (jumpIt % 2 == 0) { CurrentEventResults.AddKoResult(CurrentStartList[jumpIt]); }
        //         rank1 = CurrentEventResults.CompetitorRankKo(CurrentStartList[jumpIt]);
        //     }
        //     else
        //     {
        //         CurrentEventResults.AddNormalResult(CurrentStartList[jumpIt]);
        //         rank1 = CurrentEventResults.rank[CurrentStartList[jumpIt]];
        //     }

        //     pts1 = CurrentEventResults.totalResults[(CurrentStartList[jumpIt])];
        //     rank2 = -1;
        //     pts2 = 0;
        //     if (CurrentRoundInfo.roundType == RoundType.KO && jumpIt % 2 == 1)
        //     {
        //         CurrentEventResults.AddKoResult(CurrentStartList[jumpIt - 1], CurrentStartList[jumpIt]);
        //         rank1 = CurrentEventResults.CompetitorRankKo(CurrentStartList[jumpIt]);
        //         rank2 = CurrentEventResults.CompetitorRankKo(CurrentStartList[jumpIt - 1]);
        //         pts2 = CurrentEventResults.totalResults[(CurrentStartList[jumpIt - 1])];
        //         return 2;
        //     }
        //     return 1;
        // }

        // public void UpdateClassifications()
        // {
        //     int[] individualPlacePoints = { 100, 80, 60, 50, 45, 40, 36, 32, 29, 26, 24, 22, 20, 18, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        //     int[] teamPlacePoints = { 400, 350, 300, 250, 200, 150, 100, 50 };
        //     foreach (var it in CurrentEvent.classifications)
        //     {
        //         switch (calendar.classifications[it].classificationType)
        //         {
        //             case ClassificationType.IndividualPlace:
        //                 for (int i = 0; i < CurrentEventResults.participants.Count; i++)
        //                 {
        //                     decimal pts = 0;
        //                     int rnk = CurrentEventResults.rank[i];
        //                     if (0 < rnk && rnk < 30) { pts = individualPlacePoints[rnk - 1]; }
        //                     classificationResults[it].AddResult(CurrentEventResults.participants[i], pts);
        //                 }
        //                 break;
        //             case ClassificationType.IndividualPoints:
        //                 for (int i = 0; i < CurrentEventResults.participants.Count; i++)
        //                 {
        //                     decimal pts = CurrentEventResults.totalResults[i];
        //                     classificationResults[it].AddResult(CurrentEventResults.participants[i], pts);
        //                 }
        //                 break;
        //             case ClassificationType.TeamPlace:
        //                 break;
        //             case ClassificationType.TeamPoints:
        //                 break;
        //         }
        //     }
        // }
    }
}