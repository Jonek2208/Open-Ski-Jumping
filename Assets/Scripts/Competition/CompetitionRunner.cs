using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using OpenSkiJumping.TVGraphics;
using UnityEngine;
using UnityEngine.Events;

namespace OpenSkiJumping.Competition
{
    public class CompetitionRunner : MonoBehaviour
    {
        [SerializeField] private SavesRuntime savesRepository;
        [SerializeField] private HillsRuntime hillsRepository;
        [SerializeField] private MeshScript hill;
        [SerializeField] private RuntimeCompetitorsList competitors;
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
            var save = savesRepository.GetCurrentSave();
            var eventId = save.resultsContainer.eventIndex;

            hill.profileData.Value = hillsRepository.GetProfileData(save.calendar.events[eventId].hillId);
            hill.GenerateMesh();
            competitors.competitors = save.competitors.Select(it => it.competitor).ToList();
            competitors.teams = save.teams.Select(it => it.team).ToList();


            var eventParticipants =
                save.calendar.events[eventId].eventType == EventType.Individual
                    ? save.competitors.Where(it => it.registered).Select(it => new Participant
                        {competitors = new List<int> {it.calendarId}, id = it.calendarId}).ToList()
                    : save.teams.Where(it => it.registered && it.competitors.Count >= 4).Select(it => new Participant
                        {
                            competitors = it.competitors.Select(x => x.calendarId).Take(4).ToList(), id = it.calendarId
                        })
                        .ToList();

            save.resultsContainer.eventResults[eventId] = new EventResults {participants = eventParticipants};

            var orderedParticipants = EventProcessor.GetCompetitors(save.calendar, save.resultsContainer).ToList();
            var hillId = save.calendar.events[eventId].hillId;

            hillInfo = hillsRepository.GetHillInfo(hillId);
            resultsManager.Initialize(save.calendar.events[eventId], orderedParticipants, eventParticipants, hillInfo);

            onCompetitionStart.Invoke();
            OnRoundStart();
            OnSubroundStart();
            OnJumpStart();
        }

        public void OnRoundStart()
        {
            resultsManager.RoundInit();
            onRoundStart.Invoke();
            OnSubroundStart();
        }

        public void OnSubroundStart()
        {
            resultsManager.SubroundInit();
            onSubroundStart.Invoke();
            OnJumpStart();
        }

        public void OnJumpStart()
        {
            onNewJumper.Invoke();
        }
    }
}