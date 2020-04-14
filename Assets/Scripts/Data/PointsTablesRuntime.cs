using System.Collections.Generic;
using Competition.Persistent;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/PointsTablesRuntime")]
    public class PointsTablesRuntime : DatabaseObject<List<PointsTable>>
    {
        public List<PointsTable> GetData() => Data;

        public bool Remove(PointsTable item)
        {
            return Data.Remove(item);
        }

        public void Add(PointsTable item)
        {
            Data.Add(item);
        }
    }
}