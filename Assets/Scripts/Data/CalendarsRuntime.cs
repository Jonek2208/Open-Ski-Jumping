using System.Collections.Generic;
using UnityEngine;
using Competition;
using System;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/CalendarsRuntime")]
public class CalendarsRuntime : DatabaseObject<List<Calendar>>
{
    public List<Calendar> GetData() => Data;

    public bool Remove(Calendar item)
    {
        return Data.Remove(item);
    }

    public void Add(Calendar item)
    {
        Data.Add(item);
    }
}