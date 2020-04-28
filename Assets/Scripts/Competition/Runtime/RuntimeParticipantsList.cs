using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.Competition.Runtime
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeParticipantsList")]
    public class RuntimeParticipantsList : ScriptableObject
    {
        public List<Participant> participants;
    }
}
