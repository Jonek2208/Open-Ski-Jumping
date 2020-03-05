using System.Collections.Generic;
using UnityEngine;
using CompCal;


[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeParticipantsList")]
public class RuntimeParticipantsList : ScriptableObject
{
    public List<Participant> participants;
    public List<Competitor> competitors;
    public List<Team> teams;
}