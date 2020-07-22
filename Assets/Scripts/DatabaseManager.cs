using System.Collections.Generic;
using OpenSkiJumping.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace OpenSkiJumping
{
    public class DatabaseManager : MonoBehaviour
    {
        [FormerlySerializedAs("objects")] public List<RuntimeData> objectsToLoad;
        public List<RuntimeData> objectsToSave;

        private void Awake()
        {
            Load();
        }

        private void OnDestroy()
        {
            Save();
        }

        private void Load()
        {
            foreach (var item in objectsToLoad)
            {
                item.LoadData();
            }
        }

        private void Save()
        {
            foreach (var item in objectsToSave)
            {
                item.SaveData();
            }
        }
    }
}