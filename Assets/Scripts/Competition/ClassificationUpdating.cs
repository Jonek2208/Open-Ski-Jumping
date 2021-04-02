using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.Competition
{
    #region PointsGivers

    public static class PointsUtils
    {
        public static decimal GetPlacePoints(ClassificationInfo classificationInfo, int rank, int tableIndex)
        {
            return 1 <= rank && rank <= classificationInfo.pointsTables[tableIndex].value.Length
                ? classificationInfo.pointsTables[tableIndex].value[rank - 1]
                : 0m;
        }

        public static decimal GetClassificationPoints(ClassificationInfo classificationInfo, Result result,
            int tableIndex = 0)
        {
            return classificationInfo.classificationType switch
            {
                ClassificationType.Place => GetPlacePoints(classificationInfo, result.Rank, tableIndex),
                ClassificationType.Points => result.TotalPoints,
                _ => result.Rank
            };
        }

        public static void UpdateClassificationResults(ClassificationInfo classificationInfo,
            ClassificationResults classificationResults, IEnumerable<(int, decimal)> resultsUpdate)
        {
            foreach (var (id, result) in resultsUpdate)
            {
                classificationResults.totalResults[id] += result;
            }

            classificationResults.totalSortedResults = classificationResults.totalResults
                .Select((item, ind) => (item, ind)).OrderByDescending(x => x.item).Select(xx => xx.ind).ToList();
            classificationResults.rank[classificationResults.totalSortedResults[0]] = 1;
            for (var i = 1; i < classificationResults.totalSortedResults.Count; i++)
            {
                var last = classificationResults.totalSortedResults[i - 1];
                var curr = classificationResults.totalSortedResults[i];
                if (classificationResults.totalResults[curr] == classificationResults.totalResults[last])
                    classificationResults.rank[curr] = classificationResults.rank[last];
                else classificationResults.rank[curr] = i + 1;
            }
        }
    }

    public interface IPointsGiver
    {
        decimal GetPoints(ClassificationInfo classificationInfo, Result result, int tableIndex);
    }

    public class PlacePointsGiver : IPointsGiver
    {
        public decimal GetPoints(ClassificationInfo classificationInfo, Result result, int tableIndex)
        {
            return PointsUtils.GetPlacePoints(classificationInfo, result.Rank, tableIndex);
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