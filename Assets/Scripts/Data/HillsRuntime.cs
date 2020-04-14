using System.Collections.Generic;
using Competition.Persistent;
using Hills;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/HillsRuntime")]
    public class HillsRuntime : DatabaseObject<List<ProfileData>>
    {
        public HillInfo GetHillInfo(string hillName)
        {
            var hill = Data.Find(it => it.name == hillName);
            return new HillInfo((decimal)hill.w, (decimal)(hill.w + hill.l2), 0, 0, 0, 0);
        }
    }
}