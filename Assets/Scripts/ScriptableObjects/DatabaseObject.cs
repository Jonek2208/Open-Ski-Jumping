using UnityEngine;

namespace ScriptableObjects
{
    public abstract class DatabaseObject : ScriptableObject
    {
        public abstract bool LoadData();
        public abstract void SaveData();
    }
}