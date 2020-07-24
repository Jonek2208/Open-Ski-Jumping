using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.Competition
{
    #region PointsGivers

    public interface IPointsGiver
    {
        decimal GetPoints(ClassificationInfo classificationInfo, Result result, int tableIndex);
    }

    public class PlacePointsGiver : IPointsGiver
    {
        public decimal GetPoints(ClassificationInfo classificationInfo, Result result, int tableIndex)
        {
            return (0 < result.Rank && result.Rank < classificationInfo.pointsTables[tableIndex].value.Length)
                ? classificationInfo.pointsTables[tableIndex].value[result.Rank - 1]
                : 0m;
        }
    }

    public class PointsPointsGiver : IPointsGiver
    {
        public decimal GetPoints(ClassificationInfo classificationInfo, Result result, int tableIndex = 0)
        {
            return result.TotalPoints;
        }
    }

    #endregion
}