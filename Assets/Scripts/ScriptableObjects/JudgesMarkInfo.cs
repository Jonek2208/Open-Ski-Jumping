using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
{
    [CreateAssetMenu]
    public class JudgesMarkInfo : ScriptableObject
    {
        [SerializeField]
        private string countryCode;
        [SerializeField]
        private float markValue;
        [SerializeField]
        private bool isCounted;

        public string CountryCode { get => countryCode; set => countryCode = value; }
        public float MarkValue { get => markValue; set => markValue = value; }
        public bool IsCounted { get => isCounted; set => isCounted = value; }
    }
}