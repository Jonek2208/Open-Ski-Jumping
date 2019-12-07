using System;
using System.Collections.Generic;

namespace CompCal
{
    public interface IRoundRunner
    {

    }

    public interface IClassificationUpdater
    {
        decimal GetPoints(ClassificationInfo classificationInfo, Result result);
    }

    public class ClassificationUpdaterIndividual : IClassificationUpdater
    {
        public decimal GetPoints(ClassificationInfo classificationInfo, Result result)
        {
            if (classificationInfo.eventType == EventType.Team)
            {
                //Team event to individual classification ToDo
                return 0m;
            }

            if (classificationInfo.classificationType == ClassificationType.Points)
            { return result.totalPoints; }

            if (0 < result.rank && result.rank < classificationInfo.pointsTable.Length)
            { return classificationInfo.pointsTable[result.rank - 1]; }

            return 0m;
        }
    }

    public class ClassificationUpdaterTeam : IClassificationUpdater
    {
        public decimal GetPoints(ClassificationInfo classificationInfo, Result result)
        {
            if (classificationInfo.eventType == EventType.Individual)
            {
                //Individual event to team classification ToDo
                return 0m;
            }

            if (classificationInfo.classificationType == ClassificationType.Points)
            { return result.totalPoints; }

            if (0 < result.rank && result.rank < classificationInfo.pointsTable.Length)
            { return classificationInfo.pointsTable[result.rank - 1]; }

            return 0m;
        }

    }
}
