using System.Collections.Generic;
using UnityEngine;
using Competition;


[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeParticipantsList")]
public class RuntimeParticipantsList : ScriptableObject
{
    public List<Participant> participants;
}
