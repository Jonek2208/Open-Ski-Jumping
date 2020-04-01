using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/SavesRuntime")]
public class SavesRuntime : DatabaseObject<SaveData>
{

    public List<GameSave> GetData() => Data.savesList;
    public bool Remove(GameSave item)
    {
        return Data.savesList.Remove(item);
    }

    public void Add(GameSave item)
    {
        Data.savesList.Add(item);
    }
}
