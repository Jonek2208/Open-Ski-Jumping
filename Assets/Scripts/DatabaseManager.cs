using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping
{
    public class DatabaseManager : MonoBehaviour
    {
        public List<RuntimeData> objects;

        private void Awake()
        {
            foreach (var item in objects)
            {
                item.LoadData();
            }
        }

        public void Save()
        {
            foreach (var item in objects)
            {
                item.SaveData();
            }
        }
    }
}