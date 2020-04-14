using System.Collections.Generic;
using Competition.Persistent;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/CalendarsRuntime")]
    public class CalendarsRuntime : DatabaseObject<List<Calendar>>
    {
        public List<Calendar> GetData() => Data;

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