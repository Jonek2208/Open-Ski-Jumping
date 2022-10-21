using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects.Variables
{
    public class ScriptableObjectVariable<T> : ScriptableObject
    {
        [SerializeField] private T value;
        public T Value
        {
            get => value;
            set => this.value = value;
        }
    }
}