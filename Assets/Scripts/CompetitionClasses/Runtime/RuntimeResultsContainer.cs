using System.Collections.Generic;
using CompCal;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Competition/RuntimeResultsContainer")]
public class RuntimeResultsDatabase : ScriptableObject
{
    [SerializeField] private ResultsDatabase resultsDatabase;
    [SerializeField] private Calendar calendar;
    [SerializeField] private RuntimeEventInfo eventInfo;
    [SerializeField] private RuntimeParticipantsList participants;
    [SerializeField] private RuntimeHillInfo hillInfo;

    public ResultsDatabase ResultsDatabase { get => resultsDatabase; set => resultsDatabase = value; }
    public Calendar Calendar { get => calendar; set => calendar = value; }
    public RuntimeEventInfo EventInfo { get => eventInfo; set => eventInfo = value; }
    public RuntimeHillInfo HillInfo { get => hillInfo; set => hillInfo = value; }
    public RuntimeParticipantsList Participants { get => participants; set => participants = value; }

    public void PrepareCompetition(int competitionId)
    {
        eventInfo.value = calendar.events[competitionId];
        List<int> tmp = EventProcessor.GetCompetitors(competitionId, calendar, resultsDatabase);
    }
}