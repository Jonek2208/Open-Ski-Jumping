using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects.Variables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Variables/FloatVariable")]
    public class FloatVariable : ScriptableObject
    {
        [SerializeField]
        private float value;
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
            }
        }
    }
}