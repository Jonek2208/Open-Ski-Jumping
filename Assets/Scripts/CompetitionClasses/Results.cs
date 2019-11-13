﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Calendar
{
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

    public class EventResults
    {
        public List<JumpResult>[] roundResults;
        public decimal[] totalResults;
        public SortedList<Tuple<decimal, int>, int> finalResults;
        public SortedList<Tuple<decimal, int, int>, int> allroundResults;
        public SortedList<Tuple<decimal, int, int>, int> koFinalResults;
        public List<int> competitorsList;
        public List<int>[] startLists;
        public List<int>[] bibs;

        public int[] rank;
        public int[] lastRank;


        public EventResults(List<int> _competitorsList, int roundsCount)
        {
            competitorsList = _competitorsList;
            Comparison<Tuple<decimal, int>> comp = (x, y) => (x.Item1 == y.Item1 ? x.Item2.CompareTo(y.Item2) : y.Item1.CompareTo(x.Item1));
            finalResults = new SortedList<Tuple<decimal, int>, int>(Comparer<Tuple<decimal, int>>.Create(comp));
            roundResults = new List<JumpResult>[competitorsList.Count];
            totalResults = new decimal[competitorsList.Count];
            rank = new int[competitorsList.Count];
            bibs = new List<int>[competitorsList.Count];
            lastRank = new int[competitorsList.Count];
            Comparison<Tuple<decimal, int, int>> comp2 = (x, y) => (x.Item1 == y.Item1 ? (x.Item2 == y.Item2 ? y.Item3.CompareTo(x.Item3) : x.Item2.CompareTo(y.Item2)) : y.Item1.CompareTo(x.Item1));
            allroundResults = new SortedList<Tuple<decimal, int, int>, int>(Comparer<Tuple<decimal, int, int>>.Create(comp2));
            koFinalResults = new SortedList<Tuple<decimal, int, int>, int>();
            startLists = new List<int>[roundsCount];
            for (int i = 0; i < competitorsList.Count; i++)
            {
                roundResults[i] = new List<JumpResult>();
                bibs[i] = new List<int>();
            }
        }

        public int AddResult(int competitorId, JumpResult jmp)
        {
            int round = roundResults[competitorId].Count;
            if (round > 0)
            {
                allroundResults.Remove(Tuple.Create(totalResults[competitorId], round - 1, bibs[competitorId][round - 1]));
            }

            roundResults[competitorId].Add(jmp);
            totalResults[competitorId] += jmp.totalPoints;
            finalResults.Add(Tuple.Create(totalResults[competitorId], bibs[competitorId][roundResults[competitorId].Count - 1]), competitorId);
            allroundResults.Add(Tuple.Create(totalResults[competitorId], round, bibs[competitorId][round]), competitorId);

            // Update rank
            for (int i = 0; i < Math.Min(competitorsList.Count, allroundResults.Count); i++)
            {
                if (i > 0 && allroundResults.Keys[i].Item1 == allroundResults.Keys[i - 1].Item1)
                {
                    rank[allroundResults.Values[i]] = rank[allroundResults.Values[i - 1]];
                }
                else
                {
                    rank[allroundResults.Values[i]] = i + 1;
                }
            }

            return rank[competitorId];
        }
    }

    public class ClassificationResults
    {
        public List<decimal>[] eventResults;
        public decimal[] totalResults;
        public SortedList<Tuple<decimal, int>, int> totalSortedResults;

        public ClassificationResults()
        {
            totalSortedResults = new SortedList<Tuple<decimal, int>, int>();
        }

        public void AddResult(int competitorId, decimal val)
        {
            totalSortedResults.Remove(Tuple.Create(totalResults[competitorId], competitorId));
            eventResults[competitorId].Add(val);
            totalResults[competitorId] += val;
            totalSortedResults.Add(Tuple.Create(totalResults[competitorId], competitorId), competitorId);
        }
    }

    public class CalendarResults
    {
        public Calendar calendar;
        public List<HillProfile.ProfileData> hillProfiles;
        public int eventIt;
        public int roundIt;
        public int jumpIt;

        public EventResults[] eventResults;
        public ClassificationResults[] classificationResults;

        [JsonIgnore]
        public Event CurrentEvent
        {
            get => calendar.events[eventIt];
            set => calendar.events[eventIt] = value;
        }
        [JsonIgnore]
        public EventResults CurrentEventResults
        {
            get => eventResults[eventIt];
            set => eventResults[eventIt] = value;
        }
        [JsonIgnore]
        public HillInfo CurrentHillInfo
        {
            get => calendar.hillInfos[calendar.events[eventIt].hillId];
            set => calendar.hillInfos[calendar.events[eventIt].hillId] = value;
        }
        [JsonIgnore]
        public RoundInfo CurrentRoundInfo
        {
            get => calendar.events[eventIt].roundInfos[roundIt];
            set => calendar.events[eventIt].roundInfos[roundIt] = value;
        }
        [JsonIgnore]
        public List<int> CurrentStartList
        {
            get => eventResults[eventIt].startLists[roundIt];
            set => eventResults[eventIt].startLists[roundIt] = value;
        }
        [JsonIgnore]
        public Competitor CurrentCompetitor
        {
            get => calendar.competitors[eventResults[eventIt].competitorsList[eventResults[eventIt].startLists[roundIt][jumpIt]]];
        }

        public void EventInit()
        {
            List<Tuple<decimal, int>> tempList = new List<Tuple<decimal, int>>();
            List<int> competitorsList = new List<int>();

            switch (CurrentEvent.qualRankType)
            {
                case RankType.None:
                    // Add all competitors
                    competitorsList = new List<int>();
                    for (int i = 0; i < calendar.competitors.Count; i++) { tempList.Add(Tuple.Create(0m, i)); }
                    break;
                case RankType.Event:
                    // Add competitors from some event output rank
                    EventResults qualRankEvent = eventResults[CurrentEvent.qualRankId];
                    for (int i = 0; i < qualRankEvent.finalResults.Count; i++)
                    {
                        tempList.Add(Tuple.Create(qualRankEvent.finalResults.Keys[i].Item1, qualRankEvent.competitorsList[qualRankEvent.finalResults.Values[i]]));
                    }
                    break;
                case RankType.Classification:
                    // Add competitors from some event output rank
                    ClassificationResults qualRankClassification = classificationResults[CurrentEvent.qualRankId];
                    for (int i = 0; i < qualRankClassification.totalSortedResults.Count; i++)
                    {
                        tempList.Add(Tuple.Create(qualRankClassification.totalSortedResults.Keys[i].Item1, qualRankClassification.totalSortedResults.Values[i]));
                    }
                    break;
            }

            switch (CurrentEvent.inLimitType)
            {
                case LimitType.None:
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        competitorsList.Add(tempList[i].Item2);
                    }
                    break;
                case LimitType.Normal:
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        if (tempList[i].Item1 < tempList[CurrentEvent.inLimit - 1].Item1) { break; }
                        competitorsList.Add(tempList[i].Item2);
                    }
                    break;
                case LimitType.Exact:
                    for (int i = 0; i < CurrentEvent.inLimit; i++)
                    {
                        competitorsList.Add(tempList[i].Item2);
                    }
                    break;
            }

            CurrentEventResults = new EventResults(competitorsList, CurrentEvent.roundInfos.Count);

            Dictionary<int, int> map = new Dictionary<int, int>();
            int[] lut = new int[competitorsList.Count];
            if (CurrentEvent.ordRankType == RankType.Event)
            {
                for (int i = 0; i < competitorsList.Count; i++)
                {
                    map[competitorsList[i]] = i;
                }
                for (int i = 0; i < competitorsList.Count; i++) { lut[i] = -1; }

                for (int i = 0; i < eventResults[CurrentEvent.ordRankId].competitorsList.Count; i++)
                {
                    if (map.ContainsKey(eventResults[CurrentEvent.ordRankId].competitorsList[i]))
                    {
                        lut[map[eventResults[CurrentEvent.ordRankId].competitorsList[i]]] = i;
                    }
                }
            }

            List<Tuple<decimal, int>> ordArr = new List<Tuple<decimal, int>>();
            for (int i = 0; i < competitorsList.Count; i++)
            {
                decimal key = i;
                if (CurrentEvent.ordRankType == RankType.Event)
                {
                    if (lut[i] == -1)
                    {
                        key = 0;
                    }
                    else
                    {
                        key = eventResults[CurrentEvent.ordRankId].totalResults[lut[i]];
                    }
                }
                else if (CurrentEvent.ordRankType == RankType.Classification)
                {
                    key = classificationResults[CurrentEvent.ordRankId].totalResults[competitorsList[i]];
                }

                CurrentEventResults.finalResults.Add(Tuple.Create(key, i), i);
            }
        }

        public void RoundInit()
        {
            CurrentStartList = new List<int>();
            Comparison<Tuple<decimal, int>> comp = null;
            switch (CurrentRoundInfo.roundType)
            {
                case RoundType.Normal:
                    for (int i = CurrentEventResults.finalResults.Count - 1; i >= 0; i--)
                    {
                        CurrentStartList.Add(CurrentEventResults.finalResults.Values[i]);
                    }

                    jumpIt = 0;
                    comp = (x, y) => (x.Item1 == y.Item1 ? y.Item2.CompareTo(x.Item2) : y.Item1.CompareTo(x.Item1));
                    break;
                case RoundType.KO:
                    Comparison<Tuple<decimal, int>> comp2 = (x, y) => (y.Item1 == x.Item1 ? x.Item2.CompareTo(y.Item2) : y.Item1.CompareTo(x.Item1));;
                    CurrentEventResults.koFinalResults = new SortedList<Tuple<decimal, int, int>, int>();

                    int len = CurrentEventResults.finalResults.Count;
                    for (int i = 0; i < len / 2; i++)
                    {
                        CurrentStartList.Add(CurrentEventResults.finalResults.Values[(len + 1) / 2 + i]);
                        CurrentStartList.Add(CurrentEventResults.finalResults.Values[(len + 1) / 2 - i - 1]);
                    }
                    if (len % 2 == 1)
                    {
                        CurrentStartList.Add(CurrentEventResults.finalResults.Values[0]);
                    }

                    jumpIt = 0;
                    comp = (x, y) => (y.Item1 == x.Item1 ? x.Item2.CompareTo(y.Item2) : y.Item1.CompareTo(x.Item1));
                    break;
            }
            if (roundIt == 0 || CurrentRoundInfo.reassignBibs)
            {
                if (CurrentRoundInfo.reversedBibs)
                {
                    for (int i = 0; i < CurrentEventResults.finalResults.Count; i++)
                    {
                        CurrentEventResults.bibs[CurrentEventResults.finalResults.Values[i]].Add(i + 1);
                    }
                }
                else
                {
                    for (int i = 0; i < CurrentEventResults.finalResults.Count; i++)
                    {
                        CurrentEventResults.bibs[CurrentEventResults.finalResults.Values[CurrentEventResults.finalResults.Count - 1 - i]].Add(i + 1);
                    }
                }
            }
            else
            {
                for (int i = 0; i < CurrentEventResults.finalResults.Count; i++)
                {
                    CurrentEventResults.bibs[CurrentEventResults.finalResults.Values[i]].Add(CurrentEventResults.bibs[CurrentEventResults.finalResults.Values[i]][roundIt - 1]);
                }
            }

            CurrentEventResults.finalResults = new SortedList<Tuple<decimal, int>, int>(Comparer<Tuple<decimal, int>>.Create(comp));
        }

        public void RecalculateFinalResults()
        {
            for (int i = 0; i < CurrentEventResults.competitorsList.Count; i++) { CurrentEventResults.lastRank[i] = CurrentEventResults.rank[i]; }

            switch (CurrentEvent.roundInfos[roundIt].outLimitType)
            {
                case LimitType.Normal:
                    decimal minPoints = CurrentEventResults.finalResults.Keys[Math.Min(CurrentEventResults.finalResults.Count - 1, CurrentEvent.roundInfos[roundIt].outLimit - 1)].Item1;
                    for (int i = CurrentEventResults.finalResults.Count - 1; i >= 0; i--)
                    {
                        if (CurrentEventResults.finalResults.Keys[i].Item1 < minPoints)
                        {
                            CurrentEventResults.finalResults.RemoveAt(i);
                        }
                    }
                    break;
                case LimitType.Exact:
                    int index = Math.Min(CurrentEvent.roundInfos[roundIt].outLimit, CurrentEventResults.finalResults.Count - 1);
                    for (int i = CurrentEventResults.finalResults.Count - 1; i >= index; i--)
                    {
                        CurrentEventResults.finalResults.RemoveAt(i);
                    }
                    break;
            }
        }

        // returns Tuple - (rank, total points)
        public Tuple<int, decimal> AddJump(JumpResult jmp)
        {
            jmp.distancePoints = CurrentHillInfo.DistancePoints(jmp.distance);
            jmp.totalPoints = Math.Max(0m, jmp.judgesTotalPoints + jmp.distancePoints);
            return Tuple.Create(CurrentEventResults.AddResult(CurrentStartList[jumpIt], jmp), CurrentEventResults.totalResults[(CurrentStartList[jumpIt])]);
        }

        public void UpdateClassifications()
        {
            int[] individualPlacePoints = { 100, 80, 60, 50, 45, 40, 36, 32, 29, 26, 24, 22, 20, 18, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            int[] teamPlacePoints = { 400, 350, 300, 250, 200, 150, 100, 50 };
            foreach (var it in CurrentEvent.classifications)
            {
                switch (calendar.classifications[it].classificationType)
                {
                    case ClassificationType.IndividualPlace:
                        foreach (var jt in CurrentEventResults.finalResults.Values)
                        {
                            decimal pts = 0;
                            int rnk = CurrentEventResults.rank[jt];
                            if (0 < rnk && rnk < 30) { pts = individualPlacePoints[rnk - 1]; }
                            classificationResults[it].AddResult(CurrentEventResults.competitorsList[jt], pts);
                        }
                        break;
                    case ClassificationType.IndividualPoints:
                        for (int i = 0; i < CurrentEventResults.competitorsList.Count; i++)
                        {
                            decimal pts = CurrentEventResults.totalResults[i];
                            classificationResults[it].AddResult(CurrentEventResults.competitorsList[i], pts);
                        }
                        break;
                    case ClassificationType.TeamPlace:
                        break;
                    case ClassificationType.TeamPoints:
                        break;
                }
            }
        }
    }
}