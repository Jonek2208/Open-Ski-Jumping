using System;
using System.Collections.Generic;
using System.Linq;

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
        private EventResults eventResults;
        private List<Competitor> competitors;
        private List<int> teamIds;
        private List<int>[] teamResults;
        private Dictionary<int, int> idMapInd;
        private Dictionary<int, int> idMapTeam;
        public IndividualEventFinalResults(EventResults eventResults, List<Competitor> competitors)
        {
            this.eventResults = eventResults;
            this.competitors = competitors;

            this.idMapInd = this.eventResults.competitorIds.Select((val, index) => (val, index)).
                            ToDictionary(it => this.eventResults.participants[it.val].id, it => it.index);
            this.teamIds = this.eventResults.participants.Select(it => competitors[it.id].teamId).Distinct().ToList();
            this.idMapTeam = this.teamIds.Select((val, index) => (val, index)).ToDictionary(it => it.val, it => it.index);

            this.teamResults = new List<int>[teamIds.Count];
            for (int i = 0; i < this.eventResults.competitorIds.Count; i++)
            {
                int globalTeamId = this.competitors[GlobalId(i)].teamId;
                int localTeamId = this.idMapTeam[globalTeamId];
                this.teamResults[localTeamId].Add(i);
            }

            this.teamResults = this.teamResults.Select(it => it.OrderBy(x => this.eventResults.results[x].Rank).ToList()).ToArray();
        }

        public List<(int, decimal)> GetPoints(ClassificationInfo classificationInfo)
        {
            if (classificationInfo.eventType == EventType.Individual)
            { return GetIndividualPoints(classificationInfo); }
            else
            { return GetTeamPoints(classificationInfo); }
        }

        public List<(int, decimal)> GetIndividualPoints(ClassificationInfo classificationInfo)
        {
            IPointsGiver pointsGiver;
            if (classificationInfo.classificationType == ClassificationType.Place)
            { pointsGiver = new PlacePointsGiver(); }
            else
            { pointsGiver = new PointsPointsGiver(); }

            return this.eventResults.results.Select((val, index) => (GlobalId(index), pointsGiver.GetPoints(classificationInfo, val))).ToList();
        }

        public List<(int, decimal)> GetTeamPoints(ClassificationInfo classificationInfo)
        {
            IPointsGiver pointsGiver;
            if (classificationInfo.classificationType == ClassificationType.Place)
            { pointsGiver = new PlacePointsGiver(); }
            else
            { pointsGiver = new PointsPointsGiver(); }

            return this.teamResults.Select((val, index) => (this.teamIds[index], (classificationInfo.teamCompetitorsLimit > 0 ? val.Take(classificationInfo.teamCompetitorsLimit) : val).
                Select(it => pointsGiver.GetPoints(classificationInfo, this.eventResults.results[it])).Sum())).ToList();
        }

        private int GlobalId(int localId) => this.eventResults.participants[this.eventResults.competitorIds[localId]].id;
    }

    public class TeamEventFinalResults : IEventFinalResults
    {
        private EventResults eventResults;
        public TeamEventFinalResults(EventResults eventResults)
        {
            this.eventResults = eventResults;
            // ToDo
        }

        public List<(int, decimal)> GetPoints(ClassificationInfo classificationInfo)
        {
            if (classificationInfo.eventType == EventType.Individual)
            { return GetIndividualPoints(classificationInfo); }
            else
            { return GetTeamPoints(classificationInfo); }
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

            return this.eventResults.results.Select((val, index) => (GlobalId(index), pointsGiver.GetPoints(classificationInfo, val))).ToList();
        }

        private int GlobalId(int localId) => this.eventResults.participants[this.eventResults.competitorIds[localId]].id;
    }
    #endregion
}
