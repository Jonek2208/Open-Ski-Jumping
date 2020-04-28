using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeEventInfo")]
    public class RuntimeEventInfo : ScriptableObject
    {
        public EventInfo value;
    }
}