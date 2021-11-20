using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/CompetitorsRuntime")]
    public class CompetitorsRuntime : DatabaseObject<List<Competitor>>
    {
        private Dictionary<string, int> dict;

        public Competitor GetJumperById(string id)
        {
            return ContainsJumper(id) ? data[dict[id]] : null;
        }

        public bool ContainsJumper(string id) => dict.ContainsKey(id);
        public IEnumerable<Competitor> GetData() => Data;

        public void Add(Competitor competitor)
        {
            data.Add(competitor);
            AddToDict(competitor, data.Count - 1);
        }

        public IEnumerable<Competitor> GetJumpersById(IEnumerable<string> competitorsIds)
        {
            return competitorsIds.Where(ContainsJumper).Select(GetJumperById);
        }

        public bool Remove(Competitor competitor)
        {
            var jumperId = competitor.id;
            if (!dict.ContainsKey(jumperId))
            {
                return false;
            }

            var index = dict[jumperId];
            data[index] = data[^1];
            dict[data[index].id] = index;
            data.RemoveAt(data.Count - 1);
            dict.Remove(jumperId);
            return true;
        }

        private void AddToDict(Competitor competitor, int index)
        {
            var idWithoutNum = GetJumperIdWithoutNum(competitor);
            var num = 0;
            if (IsJumperIdValid(competitor, competitor.id))
            {
                GetJumperIdNum(competitor.id, out num);
            }

            var jumperId = $"{idWithoutNum}#{num}";
            if (dict.ContainsKey(jumperId))
            {
                num = 0;
            }

            while (dict.ContainsKey(jumperId))
            {
                num++;
                jumperId = $"{idWithoutNum}#{num}";
            }

            competitor.id = jumperId;
            dict.Add(jumperId, index);
        }

        private static string GetJumperIdWithoutNum(Competitor comp)
        {
            var gender = (comp.gender == Gender.Male ? "M" : "F");
            var firstName = comp.firstName.Replace(' ', '_');
            var lastName = comp.lastName.Replace(' ', '_');
            return $"{comp.countryCode}-{gender}-{lastName}-{firstName}";
        }

        public void Recalculate(Competitor competitor)
        {
            var index = dict[competitor.id];
            dict.Remove(competitor.id);
            AddToDict(competitor, index);
        }

        private static bool GetJumperIdNum(string value, out int result)
        { 
            var numMatch = Regex.Match(value, "(#)([0-9]+)$").Groups[2].Value;
            var tmp = int.TryParse(numMatch, out result);
            return tmp;
        }

        private bool IsJumperIdValid(Competitor competitor, string jumperId)
        {
            var tmp = GetJumperIdNum(jumperId, out _);
            if (!tmp)
            {
                return false;
            }

            var idMatch = Regex.Match(jumperId, "(.*)(#)([0-9]+)").Groups[1].Value;
            var properJumperId = GetJumperIdWithoutNum(competitor);
            return string.Equals(idMatch, properJumperId);
        }

        public override bool LoadData()
        {
            var tmp = base.LoadData();
            if (!tmp)
            {
                return false;
            }

            dict = new Dictionary<string, int>();
            for (var i = 0; i < data.Count; i++)
            {
                AddToDict(data[i], i);
            }

            return true;
        }
    }
}