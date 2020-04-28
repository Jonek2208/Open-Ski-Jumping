using OpenSkiJumping.ScriptableObjects.Variables;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
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