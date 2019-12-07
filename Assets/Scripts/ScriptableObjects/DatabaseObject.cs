using UnityEngine;
public abstract class DatabaseObject : ScriptableObject
{
    public abstract bool LoadData();
    public abstract void SaveData();
}