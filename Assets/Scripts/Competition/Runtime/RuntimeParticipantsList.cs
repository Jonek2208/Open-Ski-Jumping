using System.Collections.Generic;
using Competition.Persistent;
using UnityEngine;

namespace Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeParticipantsList")]
    public class RuntimeParticipantsList : ScriptableObject
    {
        public List<Participant> participants;
    }
}
