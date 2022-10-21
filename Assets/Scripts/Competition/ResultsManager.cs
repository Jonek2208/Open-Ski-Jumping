using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;

namespace OpenSkiJumping.Competition
{
    public interface IResultsManager
    {
        IList<Participant> OrderedParticipants { get; }
        IList<Result> Results { get; }
        IList<int> LastRank { get; }
        IList<int> StartList { get; }
        int StartListIndex { get; }
        int SubroundIndex { get; }
        int RoundIndex { get; }
        void RegisterJump(IJumpData jumpData);
        void SubroundInit();
        void RoundInit();
        bool SubroundFinish();
        bool RoundFinish();
        bool JumpFinish();
        Result GetResultByRank(int rank);
        int GetIdByRank(int rank);
        JumpResults GetResultById(int primaryId, int secondaryId);
        int GetCurrentCompetitorId();
        int GetCurrentCompetitorLocalId();
        int GetCurrentJumperId();
        int CompetitorRank(int id);
        IEnumerable<(int, decimal)> GetPoints(ClassificationInfo classificationInfo);
        EventInfo EventInfo { get; }
        EventResults GetEventResults();
    }

    public class TrainingResultsManager : IResultsManager
    {
        public IList<Participant> OrderedParticipants { get; }
        public IList<Result> Results { get; private set; }
        public IList<int> LastRank { get; }
        public IList<int> StartList { get; }
        public int StartListIndex { get; }
        public int SubroundIndex => 0;
        public int RoundIndex => 0;

        public void RegisterJump(IJumpData jumpData)
        {
            throw new NotImplementedException();
        }

        public void SubroundInit()
        {
            throw new NotImplementedException();
        }

        public void RoundInit()
        {
            throw new NotImplementedException();
        }

        public bool SubroundFinish()
        {
            throw new NotImplementedException();
        }

        public bool RoundFinish()
        {
            throw new NotImplementedException();
        }

        public bool JumpFinish()
        {
            throw new NotImplementedException();
        }

        public Result GetResultByRank(int rank)
        {
            throw new NotImplementedException();
        }

        public int GetIdByRank(int rank)
        {
            throw new NotImplementedException();
        }

        public JumpResults GetResultById(int primaryId, int secondaryId)
        {
            throw new NotImplementedException();
        }

        public int GetCurrentCompetitorId()
        {
            throw new NotImplementedException();
        }

        public int GetCurrentCompetitorLocalId()
        {
            throw new NotImplementedException();
        }

        public int GetCurrentJumperId()
        {
            throw new NotImplementedException();
        }

        public int CompetitorRank(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<(int, decimal)> GetPoints(ClassificationInfo classificationInfo)
        {
            throw new NotImplementedException();
        }

        public EventInfo EventInfo { get; }

        public EventResults GetEventResults()
        {
            throw new NotImplementedException();
        }

        public TrainingResultsManager(IHillInfo hillInfo, IList<Participant> orderedParticipants,
            bool disableJudgesMarks)
        {
            EventInfo = new EventInfo { };
            OrderedParticipants = orderedParticipants;
            InitializeValues();
        }

        private void InitializeValues()
        {
            Results = new List<Result>();

            for (var index = 0; index < Results.Count; index++)
            {
                Results[index] = new Result();
                var item = Results[index];
                item.TotalResults = new decimal[1];
                item.Results = new JumpResults[] {new()};
                item.Bibs = new int[1];
            }

            // _finalResults =
            //     new SortedList<(int state, decimal points, int bib), int>(
            //         Comparer<(int state, decimal points, int bib)>.Create(_finalResultsComp));
            // _allRoundResults =
            //     new SortedList<(decimal points, int bib, int round), int>(
            //         Comparer<(decimal points, int bib, int round)>.Create(_allRoundResultsComp));
            // _losersResults =
            //     new SortedList<(decimal points, int bib), int>(
            //         Comparer<(decimal points, int bib)>.Create(_losersResultsComp));
        }
    }

    public class ResultsManager : IResultsManager
    {
        private readonly IHillInfo _hillInfo;

        private SortedList<(decimal points, int bib, int round), int> _allRoundResults;
        private SortedList<(int state, decimal points, int bib), int> _finalResults;
        private SortedList<(decimal points, int bib), int> _losersResults;
        private readonly int[] _initGates;

        private int _competitorsCount;
        private int[] _koState;
        private int _maxBib;
        private int _maxLosers;
        private int _roundsCount;
        private int _subRoundsCount;

        public ResultsManager(EventInfo eventInfo, IList<Participant> orderedParticipants, IHillInfo hillInfo)
        {
            EventInfo = eventInfo;
            OrderedParticipants = orderedParticipants;
            _hillInfo = hillInfo;
            _initGates = new int[eventInfo.roundInfos.Count];

            InitializeValues();
        }

        private void InitializeValues()
        {
            _competitorsCount = OrderedParticipants.Count;
            Results = new Result[_competitorsCount];
            LastRank = new int[_competitorsCount];
            _roundsCount = EventInfo.roundInfos.Count;
            _subRoundsCount = EventInfo.eventType == EventType.Individual ? 1 : 4;

            for (var index = 0; index < Results.Count; index++)
            {
                Results[index] = new Result();
                var item = Results[index];
                item.TotalResults = new decimal[_subRoundsCount];
                item.Results = new JumpResults[_subRoundsCount];
                for (var i = 0; i < _subRoundsCount; i++) item.Results[i] = new JumpResults();

                item.Bibs = new int[_roundsCount];
            }

            _finalResults =
                new SortedList<(int state, decimal points, int bib), int>(
                    Comparer<(int state, decimal points, int bib)>.Create(_finalResultsComp));
            _allRoundResults =
                new SortedList<(decimal points, int bib, int round), int>(
                    Comparer<(decimal points, int bib, int round)>.Create(_allRoundResultsComp));
            _losersResults =
                new SortedList<(decimal points, int bib), int>(
                    Comparer<(decimal points, int bib)>.Create(_losersResultsComp));

            _koState = new int[_competitorsCount];
        }

        public int StartListIndex { get; private set; }
        public int SubroundIndex { get; private set; }
        public int RoundIndex { get; private set; }

        public IList<int> StartList { get; private set; }
        public IList<Participant> OrderedParticipants { get; }

        public IList<Result> Results { get; private set; }

        public IList<int> LastRank { get; private set; }

        public void SubroundInit()
        {
            var currentRoundInfo = EventInfo.roundInfos[RoundIndex];
            IEnumerable<int> tmp;

            //first sub-round
            if (RoundIndex == 0 && SubroundIndex == 0)
            {
                tmp = Enumerable.Range(0, _competitorsCount).Reverse();
            }
            else
            {
                tmp = currentRoundInfo.useOrdRank[SubroundIndex]
                    ? _finalResults.Select(item => item.Value).OrderBy(item => item).Reverse()
                    : _finalResults.Select(item => item.Value).Reverse();
            }

            for (var i = 0; i < _competitorsCount; i++) _koState[i] = 0;

            var tmpList = tmp.ToList();
            _finalResults.Clear();

            if (currentRoundInfo.roundType == RoundType.Normal)
            {
                StartList = tmpList;
                return;
            }

            StartList = Enumerable.Range(0, tmpList.Count).Select(it => tmpList[KOIndex(it, tmpList.Count)]).ToList();
            _maxLosers = Math.Max(0, currentRoundInfo.outLimit - (StartList.Count + 1) / 2);
        }

        public void RoundInit()
        {
            var currentRoundInfo = EventInfo.roundInfos[RoundIndex];

            //first round
            if (RoundIndex == 0)
            {
                for (var i = 0; i < _competitorsCount; i++)
                    Results[i].Bibs[RoundIndex] = currentRoundInfo.reversedBibs ? i + 1 : _competitorsCount - i;
            }
            //reassign bibs
            else if (currentRoundInfo.reassignBibs)
            {
                for (var i = 0; i < _finalResults.Count; i++)
                {
                    var it = _finalResults.Values[i];
                    if (currentRoundInfo.reversedBibs)
                        Results[it].Bibs[RoundIndex] = i + 1;
                    else
                        Results[it].Bibs[RoundIndex] = _finalResults.Count - i;
                }
            }
            //bibs from previous round
            else
            {
                for (var i = 0; i < _finalResults.Count; i++)
                {
                    var id = _finalResults.Values[i];
                    var lastRoundBib = Results[id].Bibs[RoundIndex - 1];
                    Results[id].Bibs[RoundIndex] = lastRoundBib;
                }
            }
        }

        public bool JumpFinish()
        {
            StartListIndex++;
            return StartListIndex < StartList.Count;
        }

        public Result GetResultByRank(int rank)
        {
            return Results[_allRoundResults.Values[rank]];
        }

        public int GetIdByRank(int rank)
        {
            return _finalResults.Values[rank];
        }

        public JumpResults GetResultById(int primaryId, int secondaryId)
        {
            return Results[primaryId].Results[secondaryId];
        }

        public int GetCurrentCompetitorId() => OrderedParticipants[StartList[StartListIndex]].id;
        public int GetCurrentCompetitorLocalId() => StartList[StartListIndex];
        public int GetCurrentJumperId() => OrderedParticipants[StartList[StartListIndex]].competitors[SubroundIndex];


        public bool SubroundFinish()
        {
            LastRank = Results.Select(item => item.Rank).ToArray();
            SubroundIndex++;
            StartListIndex = 0;
            return SubroundIndex < _subRoundsCount;
        }

        public bool RoundFinish()
        {
            var currentRoundInfo = EventInfo.roundInfos[RoundIndex];

            switch (currentRoundInfo.outLimitType)
            {
                case LimitType.Normal:
                    var lastIndex = Math.Min(_finalResults.Keys.Count, currentRoundInfo.outLimit) - 1;
                    if (currentRoundInfo.roundType == RoundType.KO)
                    {
                        var it = Math.Min(currentRoundInfo.outLimit, _finalResults.Count);
                        while (it < _finalResults.Count && _finalResults.Keys[it].state == 0 &&
                               _finalResults.Keys[it - 1].points == _finalResults.Keys[it].points) it++;

                        var stop = it;
                        it = _finalResults.Count - 1;
                        while (stop <= it)
                        {
                            _finalResults.RemoveAt(it);
                            it--;
                        }
                    }
                    else
                    {
                        var minPts = _finalResults.Keys[lastIndex].points;
                        var it = _finalResults.Count - 1;
                        while (_finalResults.Keys[it].points < minPts)
                            _finalResults.RemoveAt(it--);
                    }

                    break;
                case LimitType.Exact:
                    for (var i = _finalResults.Count - 1; i >= currentRoundInfo.outLimit; i--)
                        _finalResults.RemoveAt(i);
                    break;
            }


            RoundIndex++;
            SubroundIndex = 0;
            return RoundIndex < _roundsCount;
        }

        public void RegisterJump(IJumpData jumpData)
        {
            // Handle disableJudgesMarks
            var currentRoundInfo = EventInfo.roundInfos[RoundIndex];
            if (currentRoundInfo.disableJudgesMarks)
                for (var i = 0; i < jumpData.JudgesMarks.Length; i++)
                    jumpData.JudgesMarks[i] = 0m;

            //Set init gate for round
            if (StartListIndex == 0 && SubroundIndex == 0)
            {
                _initGates[RoundIndex] = jumpData.Gate;
                jumpData.InitGate = jumpData.Gate;
            }

            var jump = EventProcessor.GetJumpResult(jumpData, _hillInfo, currentRoundInfo.gateCompensation,
                currentRoundInfo.windCompensation);
            if (RoundIndex > 0 || SubroundIndex > 0) RemoveFromAllRoundResults();

            AddResult(StartList[StartListIndex], SubroundIndex, jump);
            AddToAllRoundResults();
            AddToFinalResults();
        }

        private void AddResult(int primaryIndex, int secondaryIndex, JumpResult jump)
        {
            Results[primaryIndex].Results[secondaryIndex].results.Add(jump);
            Results[primaryIndex].TotalResults[secondaryIndex] =
                Results[primaryIndex].Results[secondaryIndex].results.Sum(item => item.totalPoints);
            Results[primaryIndex].TotalPoints = Results[primaryIndex].TotalResults.Sum();
        }

        public int CompetitorRank(int id)
        {
            var key = (_koState[id], Results[id].TotalPoints, -1);
            int lo = 0, hi = _finalResults.Count;
            while (lo < hi)
            {
                var index = lo + (hi - lo) / 2;
                var el = _finalResults.Keys[index];
                if (_finalResults.Comparer.Compare(el, key) >= 0)
                    hi = index;
                else
                    lo = index + 1;
            }

            return hi + 1;
        }

        private void AddToFinalResults()
        {
            if (EventInfo.roundInfos[RoundIndex].roundType == RoundType.KO && StartListIndex % 2 == 1)
            {
                AddSecondKOJumper();
            }
            else
            {
                var id = StartList[StartListIndex];
                var bibCode = GetBibCode(Results[id].Bibs[RoundIndex]);
                _finalResults.Add((0, Results[id].TotalPoints, bibCode), id);
            }
        }

        private void AddToAllRoundResults()
        {
            var competitorId = StartList[StartListIndex];
            var subroundNum = RoundIndex * _subRoundsCount + SubroundIndex;
            var bibCode = GetBibCode(Results[competitorId].Bibs[RoundIndex]);
            _allRoundResults.Add((Results[competitorId].TotalPoints, bibCode, subroundNum), competitorId);

            // Update rank
            for (var i = 0; i < Math.Min(_competitorsCount, _allRoundResults.Count); i++)
                if (i > 0 && _allRoundResults.Keys[i].points == _allRoundResults.Keys[i - 1].points)
                    Results[_allRoundResults.Values[i]].Rank = Results[_allRoundResults.Values[i - 1]].Rank;
                else
                    Results[_allRoundResults.Values[i]].Rank = i + 1;
        }

        private void RemoveFromAllRoundResults()
        {
            var competitorId = StartList[StartListIndex];
            var subroundNum = RoundIndex * _subRoundsCount + SubroundIndex - 1;
            var bibRoundIndex = SubroundIndex > 0 ? RoundIndex : RoundIndex - 1;
            var bibCode = GetBibCode(Results[competitorId].Bibs[bibRoundIndex]);
            _allRoundResults.Remove((Results[competitorId].TotalPoints, bibCode, subroundNum));
        }

        private void AddSecondKOJumper()
        {
            var id1 = StartList[StartListIndex - 1];
            var id2 = StartList[StartListIndex];
            var bibCode1 = GetBibCode(Results[id1].Bibs[RoundIndex]);
            var bibCode2 = GetBibCode(Results[id2].Bibs[RoundIndex]);
            var result1 = Results[id1].TotalPoints;
            var result2 = Results[id2].TotalPoints;

            _finalResults.Remove((0, result1, bibCode1));

            int loserId = id1, winnerId = id2;
            int loserBib = bibCode1, winnerBib = bibCode2;

            if (result1 > result2)
            {
                winnerId = id1;
                winnerBib = bibCode1;
                loserId = id2;
                loserBib = bibCode2;
            }

            _finalResults.Add((0, Results[winnerId].TotalPoints, winnerBib), winnerId);

            Results[loserId].Results[SubroundIndex].results[RoundIndex].state = JumpResultState.KoLoser;
            _losersResults.Add((Results[loserId].TotalPoints, loserBib), loserId);
            var loserRank = _losersResults.IndexOfKey((Results[loserId].TotalPoints, loserBib));

            //lost
            if (loserRank >= _maxLosers)
            {
                _koState[loserId] = 1;
                _finalResults.Add((1, Results[loserId].TotalPoints, loserBib), loserId);
            }
            //lucky loser
            else
            {
                //remove last lucky loser
                if (_losersResults.Count > _maxLosers)
                {
                    var (lastLoserPoints, lastLoserBib) = _losersResults.Keys[_maxLosers];
                    var lastLoserId = _losersResults.Values[_maxLosers];
                    _koState[lastLoserId] = 1;
                    Results[loserId].Results[SubroundIndex].results[RoundIndex].state = JumpResultState.KoLoser;

                    _finalResults.Remove((0, lastLoserPoints, lastLoserBib));
                    _finalResults.Add((1, lastLoserPoints, lastLoserBib), lastLoserId);
                }

                _finalResults.Add((0, Results[loserId].TotalPoints, loserBib), loserId);
            }
        }

        private readonly Comparison<(int state, decimal points, int bib)> _finalResultsComp = (x, y) =>
            x.state == y.state
                ? x.points == y.points
                    ? x.bib.CompareTo(y.bib)
                    : y.points.CompareTo(x.points)
                : x.state.CompareTo(y.state);

        private readonly Comparison<(decimal points, int bib, int round)> _allRoundResultsComp = (x, y) =>
            x.points == y.points
                ? x.bib == y.bib
                    ? y.round.CompareTo(x.round)
                    : x.bib.CompareTo(y.bib)
                : y.points.CompareTo(x.points);

        private readonly Comparison<(decimal points, int bib)> _losersResultsComp = (x, y) =>
            x.points == y.points
                ? x.bib.CompareTo(y.bib)
                : y.points.CompareTo(x.points);

        private int GetBibCode(int bib) =>
            EventInfo.roundInfos[RoundIndex].reversedBibs ? bib : _competitorsCount - bib;

        private static int KOIndex(int index, int length)
        {
            var halfLen = length / 2;
            var halfIndex = index / 2;

            if (index == 2 * halfLen) return index;
            if (index % 2 == 0)
                return halfLen - 1 - halfIndex;
            return halfLen + halfIndex;
        }

        public IEnumerable<(int, decimal)> GetPoints(ClassificationInfo classificationInfo)
        {
            return classificationInfo.eventType == EventType.Individual
                ? GetIndividualPoints(classificationInfo)
                : GetTeamPoints(classificationInfo);
        }

        public EventInfo EventInfo { get; }

        public EventResults GetEventResults()
        {
            return new()
            {
                competitorIds = OrderedParticipants.Select(it => it.id).ToList(),
                results = Results.ToList(),
                allroundResults = _allRoundResults.Select(it => it.Value).ToList(),
                finalResults = _finalResults.Select(it => it.Value).ToList()
            };
        }

        private IEnumerable<(int, decimal)> GetIndividualPoints(ClassificationInfo classificationInfo)
        {
            IPointsGiver pointsGiver;
            if (classificationInfo.classificationType == ClassificationType.Place)
                pointsGiver = new PlacePointsGiver();
            else
                pointsGiver = new PointsPointsGiver();

            if (EventInfo.eventType == EventType.Individual)
            {
                return Results.Select((val, index) =>
                    (OrderedParticipants[index].id, pointsGiver.GetPoints(classificationInfo, val, 0)));
            }

            //EventInfo.eventType == EventType.Team

            var res = new List<(int, decimal)>();
            for (var i = 0; i < Results.Count; i++)
            {
                res.AddRange(Results[i].TotalResults.Select((t, j) => (OrderedParticipants[i].competitors[j], t)));
            }

            if (classificationInfo.classificationType == ClassificationType.Points)
                return res;
            res = res.OrderByDescending(it => it.Item2).ToList();

            var indRank = new int[res.Count];
            indRank[0] = 1;
            for (var i = 1; i < indRank.Length; i++)
            {
                if (res[i - 1].Item2 == res[i].Item2) indRank[i] = indRank[i - 1];
                else indRank[i] = i + 1;
            }

            return res.Select((it, ind) => (it.Item1, PointsUtils.GetPlacePoints(classificationInfo, ind, 0)));
        }

        private IEnumerable<(int, decimal)> GetTeamPoints(ClassificationInfo classificationInfo)
        {
            IPointsGiver pointsGiver;
            if (classificationInfo.classificationType == ClassificationType.Place)
                pointsGiver = new PlacePointsGiver();
            else
                pointsGiver = new PointsPointsGiver();

            if (EventInfo.eventType == EventType.Team)
            {
                return Results.Select((val, index) =>
                    (OrderedParticipants[index].id, pointsGiver.GetPoints(classificationInfo, val, 1)));
            }

            var competitorsByTeam = OrderedParticipants.Select((it, ind) => (it, ind)).ToLookup(it => it.it.teamId);
            return competitorsByTeam.Select(teamMembers => (
                teamMembers.Key,
                (classificationInfo.teamClassificationLimitType == TeamClassificationLimitType.Best
                    ? teamMembers.Select(it => pointsGiver.GetPoints(classificationInfo, Results[it.ind], 0))
                        .OrderByDescending(it => it).Take(classificationInfo.teamCompetitorsLimit)
                    : teamMembers.Select(it => pointsGiver.GetPoints(classificationInfo, Results[it.ind], 0))).Sum()));
        }
    }
}