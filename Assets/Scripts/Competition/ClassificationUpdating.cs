using System;
using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;

namespace Competition
{
    #region PointsGivers
    public interface IPointsGiver
    {
        decimal GetPoints(ClassificationInfo classificationInfo, Result result);
        decimal GetPoints(ClassificationInfo classificationInfo, int rank, decimal points);
    }

    public class PlacePointsGiver : IPointsGiver
    {
        public decimal GetPoints(ClassificationInfo classificationInfo, Result result)
        {
            return ((0 < result.Rank && result.Rank < classificationInfo.pointsTables[0].value.Length) ? classificationInfo.pointsTables[0].value[result.Rank - 1] : 0m);
        }

        public decimal GetPoints(ClassificationInfo classificationInfo, int rank, decimal points)
        {
            return ((0 < rank && rank < classificationInfo.pointsTables[0].value.Length) ? classificationInfo.pointsTables[0].value[rank - 1] : 0m);
        }
    }

    public class PointsPointsGiver : IPointsGiver
    {
        public decimal GetPoints(ClassificationInfo classificationInfo, Result result)
        {
            return result.TotalPoints;
        }

        public decimal GetPoints(ClassificationInfo classificationInfo, int rank, decimal points)
        {
            return points;
        }
    }
    #endregion

    #region EventFinalResults
    public interface IEventFinalResults
    {
        List<(int, decimal)> GetPoints(ClassificationInfo classificationInfo);
        List<(int, decimal)> GetIndividualPoints(ClassificationInfo classificationInfo);
        List<(int, decimal)> GetTeamPoints(ClassificationInfo classificationInfo);
    }

    public class IndividualEventFinalResults : IEventFinalResults
    {
        private readonly EventResults eventResults;
        private readonly List<Competitor> competitors;
        private readonly List<int> teamIds;
        private readonly List<int>[] teamResults;
        private Dictionary<int, int> idMapInd;
        private readonly Dictionary<int, int> idMapTeam;
        public IndividualEventFinalResults(EventResults eventResults, List<Competitor> competitors)
        {
            this.eventResults = eventResults;
            this.competitors = competitors;

            idMapInd = this.eventResults.competitorIds.Select((val, index) => (val, index)).
                            ToDictionary(it => this.eventResults.participants[it.val].id, it => it.index);
            teamIds = this.eventResults.participants.Select(it => competitors[it.id].teamId).Distinct().ToList();
            idMapTeam = teamIds.Select((val, index) => (val, index)).ToDictionary(it => it.val, it => it.index);

            teamResults = new List<int>[teamIds.Count];
            for (int i = 0; i < this.eventResults.competitorIds.Count; i++)
            {
                int globalTeamId = this.competitors[GlobalId(i)].teamId;
                int localTeamId = idMapTeam[globalTeamId];
                teamResults[localTeamId].Add(i);
            }

            teamResults = teamResults.Select(it => it.OrderBy(x => this.eventResults.results[x].Rank).ToList()).ToArray();
        }

        public List<(int, decimal)> GetPoints(ClassificationInfo classificationInfo)
        {
            if (classificationInfo.eventType == EventType.Individual)
            { return GetIndividualPoints(classificationInfo); }

            return GetTeamPoints(classificationInfo);
        }

        public List<(int, decimal)> GetIndividualPoints(ClassificationInfo classificationInfo)
        {
            IPointsGiver pointsGiver;
            if (classificationInfo.classificationType == ClassificationType.Place)
            { pointsGiver = new PlacePointsGiver(); }
            else
            { pointsGiver = new PointsPointsGiver(); }

            return eventResults.results.Select((val, index) => (GlobalId(index), pointsGiver.GetPoints(classificationInfo, val))).ToList();
        }

        public List<(int, decimal)> GetTeamPoints(ClassificationInfo classificationInfo)
        {
            IPointsGiver pointsGiver;
            if (classificationInfo.classificationType == ClassificationType.Place)
            { pointsGiver = new PlacePointsGiver(); }
            else
            { pointsGiver = new PointsPointsGiver(); }

            return teamResults.Select((val, index) => (teamIds[index], (classificationInfo.teamCompetitorsLimit > 0 ? val.Take(classificationInfo.teamCompetitorsLimit) : val).
                Select(it => pointsGiver.GetPoints(classificationInfo, eventResults.results[it])).Sum())).ToList();
        }

        private int GlobalId(int localId) => eventResults.participants[eventResults.competitorIds[localId]].id;
    }

    public class TeamEventFinalResults : IEventFinalResults
    {
        private readonly EventResults eventResults;
        public TeamEventFinalResults(EventResults eventResults)
        {
            this.eventResults = eventResults;
            // ToDo
        }

        public List<(int, decimal)> GetPoints(ClassificationInfo classificationInfo)
        {
            if (classificationInfo.eventType == EventType.Individual)
            { return GetIndividualPoints(classificationInfo); }

            return GetTeamPoints(classificationInfo);
        }

        public List<(int, decimal)> GetIndividualPoints(ClassificationInfo classificationInfo)
        {
            // ToDo
            throw new NotImplementedException();
        }

        public List<(int, decimal)> GetTeamPoints(ClassificationInfo classificationInfo)
        {
            IPointsGiver pointsGiver;
            if (classificationInfo.classificationType == ClassificationType.Place)
            { pointsGiver = new PlacePointsGiver(); }
            else
            { pointsGiver = new PointsPointsGiver(); }

            return eventResults.results.Select((val, index) => (GlobalId(index), pointsGiver.GetPoints(classificationInfo, val))).ToList();
        }

        private int GlobalId(int localId) => eventResults.participants[eventResults.competitorIds[localId]].id;
    }
    #endregion
}
