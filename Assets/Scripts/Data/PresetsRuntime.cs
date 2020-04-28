using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/PresetsRuntime")]
    public class PresetsRuntime : DatabaseObject<List<EventRoundsInfo>>
    {
        public List<EventRoundsInfo> GetData() => Data;
        public bool Remove(EventRoundsInfo item)
        {
            return Data.Remove(item);
        }

        public void Add(EventRoundsInfo item)
        {
            Data.Add(item);
        }
    }
}