using Competition.Persistent;
using UnityEngine;

namespace Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeEventInfo")]
    public class RuntimeEventInfo : ScriptableObject
    {
        public EventInfo value;
    }
}