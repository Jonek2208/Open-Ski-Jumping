using System.Collections.Generic;
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
            if (!dict.ContainsKey(id)) { return null; }
            return data[dict[id]];
        }
        public List<Competitor> GetData() => Data;

        public void Add(Competitor competitor)
        {
            data.Add(competitor);
            AddToDict(competitor, data.Count - 1);
        }

        public bool Remove(Competitor competitor)
        {
            string jumperId = competitor.id;
            if (!dict.ContainsKey(jumperId)) { return false; }
            int index = dict[jumperId];
            data[index] = data[data.Count - 1];
            dict[data[index].id] = index;
            data.RemoveAt(data.Count - 1);
            dict.Remove(jumperId);
            return true;
        }

        private void AddToDict(Competitor competitor, int index)
        {
            string idWithoutNum = GetJumperIdWithoutNum(competitor);
            int num = 0;
            if (IsJumperIdValid(competitor, competitor.id))
            {
                GetJumperIdNum(competitor.id, out num);
            }
            string jumperId = $"{idWithoutNum}#{num}";
            if (dict.ContainsKey(jumperId)) { num = 0; }
            while (dict.ContainsKey(jumperId))
            {
                num++;
                jumperId = $"{idWithoutNum}#{num}";
            }
            competitor.id = jumperId;
            dict.Add(jumperId, index);
        }

        private string GetJumperIdWithoutNum(Competitor comp)
        {
            string gender = (comp.gender == Gender.Male ? "M" : "F");
            string firstName = comp.firstName.Replace(' ', '_');
            string lastName = comp.lastName.Replace(' ', '_');
            return $"{comp.countryCode}-{gender}-{lastName}-{firstName}";
        }

        public void Recalculate(Competitor competitor)
        {
            int index = dict[competitor.id];
            dict.Remove(competitor.id);
            AddToDict(competitor, index);
        }

        private bool GetJumperIdNum(string value, out int result)
        {
            var numMatch = Regex.Match(value, "(#)([0-9]+)$").Groups[2].Value;
            bool tmp = int.TryParse(numMatch, out result);
            if (!tmp) { return false; }
            return true;
        }
        private bool IsJumperIdValid(Competitor competitor, string jumperId)
        {
            int num;
            bool tmp = GetJumperIdNum(jumperId, out num);
            if (!tmp) { return false; }

            var idMatch = Regex.Match(jumperId, "(.*)(#)([0-9]+)").Groups[1].Value;
            string properJumperId = GetJumperIdWithoutNum(competitor);
            return string.Equals(idMatch, properJumperId);
        }

        public override bool LoadData()
        {
            bool tmp = base.LoadData();
            if (!tmp) { return false; }
            dict = new Dictionary<string, int>();
            for (int i = 0; i < data.Count; i++)
            {
                AddToDict(data[i], i);
            }
            return tmp;
        }
    }
}