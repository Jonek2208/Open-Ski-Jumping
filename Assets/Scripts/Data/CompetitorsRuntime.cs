using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Competition;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/CompetitorsRuntime")]
public class CompetitorsRuntime : DatabaseObject<List<Competition.Competitor>>
{
    private Dictionary<string, int> dict;
    public Competition.Competitor GetJumperById(string id)
    {
        if (!this.dict.ContainsKey(id)) { return null; }
        return this.data[dict[id]];
    }
    public List<Competitor> GetData() => Data;

    public void Add(Competition.Competitor competitor)
    {
        this.data.Add(competitor);
        AddToDict(competitor, data.Count - 1);
    }

    public bool Remove(Competition.Competitor competitor)
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

    private void AddToDict(Competition.Competitor competitor, int index)
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

    private string GetJumperIdWithoutNum(Competition.Competitor comp)
    {
        string gender = (comp.gender == Competition.Gender.Male ? "M" : "F");
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
    private bool IsJumperIdValid(Competition.Competitor competitor, string jumperId)
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
        this.dict = new Dictionary<string, int>();
        for (int i = 0; i < this.data.Count; i++)
        {
            AddToDict(this.data[i], i);
        }
        return tmp;
    }
}