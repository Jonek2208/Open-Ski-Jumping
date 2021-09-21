using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/CalendarsRuntime")]
    public class CalendarsRuntime : DatabaseObject<List<Calendar>>
    {
        public IEnumerable<Calendar> GetData() => Data;

        public bool Remove(Calendar item)
        {
            return Data.Remove(item);
        }

        public void Add(Calendar item)
        {
            Data.Add(item);
        }
    }
}