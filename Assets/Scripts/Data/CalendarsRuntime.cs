using System.Collections.Generic;
using UnityEngine;
using Competition;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/CalendarsRuntime")]
public class CalendarsRuntime : DatabaseObject<List<Calendar>>
{
    public List<Calendar> GetData() => Data;
}