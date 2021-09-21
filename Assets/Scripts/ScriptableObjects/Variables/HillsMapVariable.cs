using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects.Variables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Variables/HillsMapVariable")]
    public class HillsMapVariable : ScriptableObject
    {
        [SerializeField] private HillsMap value;

        public HillsMap Value
        {
            get => value;
            set => this.value = value;
        }
    }
}