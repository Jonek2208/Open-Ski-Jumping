using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;
using OpenSkiJumping.Simulation;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/HillsRuntime")]
    public class HillsRuntime : DatabaseObject<List<ProfileData>>
    {
        public HillInfo GetHillInfo(string hillName)
        {
            var hill = Data.Find(it => it.name == hillName);

            return new HillInfo((decimal) hill.w, (decimal) (hill.w + hill.l2), 0, 0, 0,
                (decimal) hill.es / (hill.gates - 1));
        }

        public ProfileData GetProfileData(string hillName)
        {
            var hill = Data.Find(it => it.name == hillName);
            return hill;
        }

        public IEnumerable<ProfileData> GetSortedData()
        {
            return Data.OrderBy(it => it.name);
        }

        public void Add(ProfileData item)
        {
            data.Add(item);
        }

        public bool Remove(ProfileData item)
        {
            return data.Remove(item);
        }

        public override void SaveData()
        {
            data.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal));
            base.SaveData();
        }
    }
}