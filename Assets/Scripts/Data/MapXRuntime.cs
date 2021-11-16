using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;
using OpenSkiJumping.ScriptableObjects;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/MapXRuntime")]
    public class MapXRuntime : DatabaseObjectMultipleFiles<MapX>
    {
        [SerializeField] private List<DatabaseObjectFileData<HillsMap>> hillsMap;

        private Dictionary<string, DatabaseObjectFileData<HillsMap>> _hillsMapping =
            new Dictionary<string, DatabaseObjectFileData<HillsMap>>();

        public void SetData(List<DatabaseObjectFileData<MapX>> value) => data = value;
        public IEnumerable<DatabaseObjectFileData<MapX>> GetData() => data;

        public override bool LoadData()
        {
            var tmp = base.LoadData();
            hillsMap = data.Select(it =>
                new DatabaseObjectFileData<HillsMap>(HillsMapFactory.GetHillsMap(it.value), it.fileName)).ToList();
            
            _hillsMapping.Clear();
            foreach (var it in hillsMap)
            {
                foreach (var hill in it.value.profiles)
                {
                    _hillsMapping[hill.profileData.name] = it;
                }
            }

            return tmp;
        }

        public bool GetHillInfo(string hillName, out HillInfo result)
        {
            result = default;
            var tmp = GetProfileData(hillName, out var hill);
            if (!tmp) return false;

            result = new HillInfo((decimal) hill.w, (decimal) (hill.w + hill.l2), 0, 0, 0,
                (decimal) hill.es / (hill.gates - 1));
            return true;
        }

        public bool GetProfileData(string hillName, out ProfileData result)
        {
            result = default;
            if (!_hillsMapping.ContainsKey(hillName)) return false;

            result = _hillsMapping[hillName].value.profiles.Find(it => it.profileData.name == hillName).profileData;
            return true;
        }

        public IEnumerable<ProfileData> GetSortedData()
        {
            return hillsMap.SelectMany(it => it.value.profiles).Select(it => it.profileData)
                .OrderBy(it => it.name, StringComparer.InvariantCulture);
        }
    }
}