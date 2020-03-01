using UnityEngine;

using CompCal;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class CompetitionRunner : MonoBehaviour
{
    [SerializeField]
    private EventManager eventManager;
    [SerializeField]
    private IntVariable eventId;
    [SerializeField]
    private RuntimeCalendar calendar;
    [SerializeField]
    private RuntimeEventInfo eventInfo;
    [SerializeField]
    private RuntimeResultsContainer resultsContainer;
    [SerializeField]
    private RuntimeParticipantsList startList;
    [SerializeField]
    private RuntimeResultsManager resultsManager;

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
            return;
        }

        OnSubroundFinish();
    }

    public void OnSubroundFinish()
    {
        if (resultsManager.SubroundFinish())
        {
            onSubroundFinish.Invoke();
            return;
        }

        OnRoundFinish();
    }

    public void OnRoundFinish()
    {
        if (resultsManager.RoundFinish())
        {
            onRoundFinish.Invoke();
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
        resultsManager.CompetitionInit();
        OnRoundStart();
        OnSubroundStart();
        OnJumpStart();
    }

    public void OnRoundStart()
    {
        resultsManager.RoundInit();
    }

    public void OnSubroundStart()
    {
        resultsManager.SubroundInit();
    }

    public void OnJumpStart()
    {
        onNewJumper.Invoke();
    }

}