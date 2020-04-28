using OpenSkiJumping.Hills;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects.Variables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Variables/HillProfileVariable")]

    public class HillProfileVariable : ScriptableObject
    {
        [SerializeField]
        private ProfileData value;

        public ProfileData Value { get => value; set => this.value = value; } 
    }
}