using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/HillsRuntime")]
    public class HillsRuntime : DatabaseObject<List<ProfileData>>
    {
        public HillInfo GetHillInfo(string hillName)
        {
            var hill = Data.Find(it => it.name == hillName);
            return new HillInfo((decimal)hill.w, (decimal)(hill.w + hill.l2), 0, 0, 0, 0);
        }

        public ProfileData GetProfileData(string hillName)
        {
            var hill = Data.Find(it => it.name == hillName);
            return hill;
        }

        public IEnumerable<ProfileData> GetData()
        {
            return Data;
        }
    }
}