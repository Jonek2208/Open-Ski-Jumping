using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Variables/BoolVariable")]
    public class BoolVariable : ScriptableObject
    {
        [SerializeField]
        private bool value;
        public bool Value
        {
            get => value;
            set => this.value = value;
        }
    }
}