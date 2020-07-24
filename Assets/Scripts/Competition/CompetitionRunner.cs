using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace OpenSkiJumping.Competition
{
    public class CompetitionRunner : MonoBehaviour
    {
        [SerializeField] private RuntimeCompetitorsList competitors;
        [SerializeField] private MeshScript hill;
        [SerializeField] private IHillInfo hillInfo;
        [SerializeField] private HillsFactory hillsFactory;
        [SerializeField] private HillsRuntime hillsRepository;
        public UnityEvent onCompetitionFinish;

        public UnityEvent onCompetitionStart;
        public UnityEvent onJumpFinish;
        public UnityEvent onJumpStart;
        public UnityEvent onNewJumper;
        public UnityEvent onRoundFinish;
        public UnityEvent onRoundStart;
        public UnityEvent onSubroundFinish;
        public UnityEvent onSubroundStart;
        [SerializeField] private RuntimeResultsManager resultsManager;
        [SerializeField] private SavesRuntime savesRepository;
        [SerializeField] private int tournamentMenuSceneIndex;


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
            UpdateClassifications();
            var save = savesRepository.GetCurrentSave();
            save.resultsContainer.eventIndex++;
            savesRepository.SaveData();
            SceneManager.LoadScene(tournamentMenuSceneIndex);
        }

        private void UpdateClassifications()
        {
            var save = savesRepository.GetCurrentSave();
            var eventId = save.resultsContainer.eventIndex;
            var eventInfo = save.calendar.events[eventId];

            foreach (var it in eventInfo.classifications)
            {
                var classificationInfo = save.calendar.classifications[it];
                var resultsUpdate = resultsManager.Value.GetPoints(classificationInfo);
                var classificationResults = save.resultsContainer.classificationResults[it];

                foreach (var (id, result) in resultsUpdate)
                {
                    classificationResults.totalResults[id] += result;
                }

                classificationResults.totalSortedResults = classificationResults.totalResults
                    .Select((item, ind) => (item, ind)).OrderByDescending(x => x.item).Select(xx => xx.ind).ToList();
                classificationResults.rank[classificationResults.totalSortedResults[0]] = 1;
                for (var i = 1; i < classificationResults.totalSortedResults.Count; i++)
                {
                    var last = classificationResults.totalSortedResults[i - 1];
                    var curr = classificationResults.totalSortedResults[i];
                    if (classificationResults.totalResults[curr] == classificationResults.totalResults[last])
                        classificationResults.rank[curr] = classificationResults.rank[last];
                    else classificationResults.rank[curr] = i + 1;
                }
            }
        }

        public void OnCompetitionStart()
        {
            var save = savesRepository.GetCurrentSave();
            var eventId = save.resultsContainer.eventIndex;
            var currentEventInfo = save.calendar.events[eventId];

            hill.profileData.Value = hillsRepository.GetProfileData(save.calendar.events[eventId].hillId);
            hill.landingAreaSO = hillsFactory.landingAreas[(int) currentEventInfo.hillSurface].Value;
            hill.GenerateMesh();
            competitors.competitors = save.competitors.Select(it => it.competitor).ToList();
            competitors.teams = save.teams.Select(it => it.team).ToList();


            var eventParticipants =
                save.calendar.events[eventId].eventType == EventType.Individual
                    ? save.competitors.Where(it => it.registered).Select(it => new Participant
                    {
                        competitors = new List<int> {it.calendarId}, id = it.calendarId,
                        teamId = competitors.competitors[it.calendarId].teamId
                    }).ToList()
                    : save.teams.Where(it => it.registered && it.competitors.Count >= 4).Select(it => new Participant
                        {
                            competitors = it.competitors.Select(x => x.calendarId).Take(4).ToList(), id = it.calendarId,
                            teamId = it.calendarId
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