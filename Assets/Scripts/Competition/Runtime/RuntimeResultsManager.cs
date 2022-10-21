using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeResultsManager")]
    public class RuntimeResultsManager : ScriptableObject, IResultsManager
    {
        [SerializeField] private RuntimeJumpData jumpData;
        private IResultsManager Value { get; set; }
        public IList<int> StartList => Value.StartList;
        public int StartListIndex => Value.StartListIndex;
        public int SubroundIndex => Value.SubroundIndex;
        public int RoundIndex => Value.RoundIndex;
        public IList<Participant> OrderedParticipants => Value.OrderedParticipants;
        public IList<Result> Results => Value.Results;
        public IList<int> LastRank => Value.LastRank;


        public void Initialize(IResultsManager resultsManager)
        {
            Value = resultsManager;
        }

        public void RegisterJump(IJumpData jpData) => Value.RegisterJump(jpData);

        public void SubroundInit() => Value.SubroundInit();
        public void RoundInit() => Value.RoundInit();
        public bool JumpFinish() => Value.JumpFinish();
        public bool SubroundFinish() => Value.SubroundFinish();
        public bool RoundFinish() => Value.RoundFinish();

        public void RegisterJump() => Value.RegisterJump(jumpData);

        public int GetCurrentCompetitorLocalId() => Value.GetCurrentCompetitorLocalId();

        public int GetCurrentJumperId() => Value.GetCurrentJumperId();
        public int CompetitorRank(int id) => Value.CompetitorRank(id);

        public IEnumerable<(int, decimal)> GetPoints(ClassificationInfo classificationInfo) =>
            Value.GetPoints(classificationInfo);

        public EventInfo EventInfo => Value.EventInfo;

        public EventResults GetEventResults() => Value.GetEventResults();

        public Result GetResultByRank(int rank) => Value.GetResultByRank(rank);
        public int GetIdByRank(int rank) => Value.GetIdByRank(rank);

        public JumpResults GetResultById(int primaryId, int secondaryId) => Value.GetResultById(primaryId, secondaryId);

        public int GetCurrentCompetitorId() => Value.GetCurrentCompetitorId();
    }
}