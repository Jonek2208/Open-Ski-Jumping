using UnityEngine;

using Competition;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class CompetitionRunner : MonoBehaviour
{
    [SerializeField] private SavesRuntime savesRepository;
    [SerializeField] private HillsRuntime hillsRepository;
    [SerializeField] private IntVariable eventId;
    [SerializeField] private RuntimeCalendar calendar;
    [SerializeField] private RuntimeEventInfo eventInfo;
    [SerializeField] private RuntimeResultsDatabase resultsDatabase;
    [SerializeField] private RuntimeParticipantsList startList;
    [SerializeField] private RuntimeResultsManager resultsManager;
    [SerializeField] private IHillInfo hillInfo;
    public UnityEvent onCompetitionStart;
    public UnityEvent onCompetitionFinish;
    public UnityEvent onRoundStart;
    public UnityEvent onRoundFinish;
    public UnityEvent onSubroundStart;
    public UnityEvent onSubroundFinish;
    public UnityEvent onNewJumper;
    public UnityEvent onJumpStart;
    public UnityEvent onJumpFinish;

    private void Start()
    {
        // eventManager = new EventManager(eventId.Value, calendar.Value, resultsContainer.Value);
        OnCompetitionStart();
    }

    public void OnJumpFinish()
    {
        if (resultsManager.JumpFinish())
        {
            onJumpFinish.Invoke();
            OnJumpStart();
            return;
        }

        OnSubroundFinish();
    }

    public void OnSubroundFinish()
    {
        if (resultsManager.SubroundFinish())
        {
            onSubroundFinish.Invoke();
            OnSubroundStart();
            return;
        }

        OnRoundFinish();

    }

    public void OnRoundFinish()
    {
        if (resultsManager.RoundFinish())
        {
            onRoundFinish.Invoke();
            OnRoundStart();
            return;
        }

        OnCompetitionFinish();
    }

    public void OnCompetitionFinish()
    {
        onCompetitionFinish.Invoke();
    }

    public void OnCompetitionStart()
    {
        onCompetitionStart.Invoke();
        GameSave save = savesRepository.GetCurrentSave();
        var participants = EventProcessor.GetCompetitors(save.calendar, save.resultsContainer);
        int eventId = save.resultsContainer.eventIndex;
        string hillId = save.calendar.events[eventId].hillId;
        
        hillInfo = hillsRepository.GetHillInfo(hillId);
        resultsManager.Initialize(save.calendar.events[eventId], participants, hillInfo);
        OnRoundStart();
        OnSubroundStart();
        OnJumpStart();
    }

    public void OnRoundStart()
    {
        onRoundStart.Invoke();
        resultsManager.RoundInit();
        OnSubroundStart();
    }

    public void OnSubroundStart()
    {
        onSubroundStart.Invoke();
        resultsManager.SubroundInit();
        OnJumpStart();
    }

    public void OnJumpStart()
    {
        onNewJumper.Invoke();
    }



}