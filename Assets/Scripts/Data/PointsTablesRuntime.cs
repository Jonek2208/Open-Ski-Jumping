using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/PointsTablesRuntime")]
    public class PointsTablesRuntime : DatabaseObject<List<List<PointsTable>>>
    {
        public IEnumerable<PointsTable> GetData(int tableIndex) => Data[tableIndex];

        public bool Remove(int tableIndex, PointsTable item)
        {
            return Data[tableIndex].Remove(item);
        }

        public void Add(int tableIndex, PointsTable item)
        {
            Data[tableIndex].Add(item);
        }
    }
}