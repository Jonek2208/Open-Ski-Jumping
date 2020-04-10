using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/HillsRuntime")]
public class HillsRuntime : DatabaseObject<List<HillProfile.ProfileData>>
{
    public Competition.HillInfo GetHillInfo(string hillName)
    {
        var hill = Data.Find(it => it.name == hillName);
        return new Competition.HillInfo((decimal)hill.w, (decimal)(hill.w + hill.l2), 0, 0, 0, 0);
    }
}