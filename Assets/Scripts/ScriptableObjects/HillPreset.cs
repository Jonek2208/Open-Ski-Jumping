using ScriptableObjects.Variables;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class HillPreset : ScriptableObject
    {
        public HillProfileVariable profile;
        public Material inrunMaterial;
        public Material landingHillMaterial;
        public Material redLinesMaterial;
        public Material blueLinesMaterial;
        public Material secondaryLinesMaterial;
    }
}