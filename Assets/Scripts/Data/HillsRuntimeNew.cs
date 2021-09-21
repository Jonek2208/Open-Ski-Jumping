using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/HillsRuntimeNew")]
    public class HillsRuntimeNew : DatabaseObjectMultipleFiles<HillsMap>
    {
        private Dictionary<string, (int, int)> _profileMap = new Dictionary<string, (int, int)>();

        public HillInfo GetHillInfo(string hillName)
        {
            var hill = GetProfileData(hillName);

            return new HillInfo((decimal) hill.w, (decimal) (hill.hS), 0, 0, 0,
                (decimal) hill.es / (hill.gates - 1));
        }

        public override bool LoadData()
        {
            var tmp = base.LoadData();
            _profileMap.Clear();
            for (var i = 0; i < data.Count; i++)
            {
                var it = data[i];
                for (var j = 0; j < it.value.profiles.Count; j++)
                {
                    var p = it.value.profiles[j];
                    _profileMap[p.profileData.name] = (i, j);
                }
            }

            return tmp;
        }

        public ProfileData GetProfileData(string hillName)
        {
            _profileMap.TryGetValue(hillName, out var index);
            var hill = data[index.Item1].value.profiles[index.Item2].profileData;
            return hill;
        }

        public IEnumerable<ProfileData> GetSortedData()
        {
            return data.SelectMany(it => it.value.profiles).Select(it => it.profileData)
                .OrderBy(it => it.name, StringComparer.InvariantCulture);
        }
    }
}