using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.Simulation;
using OpenSkiJumping.UI;
using UnityEngine;
using UnityEngine.Events;

namespace OpenSkiJumping.Competition
{
    public class CompetitionRunner : MonoBehaviour
    {
        [SerializeField] private RuntimeCompetitorsList competitors;
        [SerializeField] private MeshScript hill;
        [SerializeField] private IHillInfo hillInfo;
        [SerializeField] private HillsFactory hillsFactory;
        [SerializeField] private HillsRuntime hillsRepository;
        [SerializeField] private SkiJumperDataController skiJumperDataController;
        [SerializeField] private WindGatePanel windGatePanel;
        [SerializeField] private JumpSimulator compensationsJumpSimulator;
        [SerializeField] private ToBeatLineController toBeatLineController;
        [SerializeField] private RuntimeJumpData jumpData;


        public UnityEvent onCompetitionFinish;

        public UnityEvent onCompetitionStart;
        public UnityEvent onJumpFinish;
        public UnityEvent onJumpStart;
        public UnityEvent onNewJumper;
        public UnityEvent onRoundFinish;
        public UnityEvent onRoundStart;
        public UnityEvent onSubroundFinish;
        public UnityEvent onSubroundStart;
        public UnityEvent onWindGateChanged;
        [SerializeField] private RuntimeResultsManager resultsManager;
        [SerializeField] private SavesRuntime savesRepository;
        [SerializeField] private MainMenuController menuController;
        
        private Dictionary<int, Color> bibColors;


        private void Start()
        {
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
            menuController.LoadTournamentMenu();
        }

        private void UpdateClassifications()
        {
            var save = savesRepository.GetCurrentSave();
            var eventId = save.resultsContainer.eventIndex;
            var eventInfo = save.calendar.events[eventId];
            var eventResults = save.resultsContainer.eventResults[eventId];
            resultsManager.Value.UpdateEventResults(eventResults);

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


            var eventParticipants = EventProcessor.EventParticipants(save, eventId);
            save.resultsContainer.eventResults[eventId] = new EventResults {participants = eventParticipants};

            var participantsDict = eventParticipants.Select(item => item)
                .ToDictionary(item => item.id, item => item);
            var orderedParticipants = EventProcessor.GetCompetitors(save.calendar, save.resultsContainer)
                .Select(it => participantsDict[it]).ToList();
            bibColors = orderedParticipants.ToDictionary(it => it.id, it => Color.white);

            var orderedClassifications = save.classificationsData.Where(it => it.useBib)
                .Select(it => (it.calendarId, it.priority)).Reverse();

            foreach (var (it, ind) in orderedClassifications)
            {
                var classificationResults = save.resultsContainer.classificationResults[it];
                var classificationInfo = save.classificationsData[it].classification;
                foreach (var id in classificationResults.totalSortedResults.TakeWhile(jumperId =>
                    classificationResults.rank[jumperId] <= 1))
                {
                    if (classificationInfo.eventType == EventType.Individual)
                    {
                        bibColors[id] = SimpleColorPicker.Hex2Color(save
                            .classificationsData[ind].classification
                            .leaderBibColor);
                    }
                    else
                    {
                        foreach (var competitor in save.teams[id].competitors)
                        {
                            bibColors[competitor.calendarId] = SimpleColorPicker.Hex2Color(save
                                .classificationsData[ind].classification
                                .leaderBibColor);
                        }
                    }
                }
            }

            var hillId = save.calendar.events[eventId].hillId;

            hillInfo = hillsRepository.GetHillInfo(hillId);
            var (head, tail, gate) = compensationsJumpSimulator.GetCompensations();
            hillInfo.SetCompensations(head, tail, gate);

            resultsManager.Initialize(currentEventInfo, orderedParticipants, hillInfo);
            jumpData.Gate = jumpData.InitGate = 1;
            jumpData.Wind = 0;

            windGatePanel.Initialize(hill.profileData.Value.gates);

            onCompetitionStart.Invoke();
            OnRoundStart();
            OnSubroundStart();
            OnJumpStart();
        }
        

        public void OnRoundStart()
        {
            resultsManager.RoundInit();
            UpdateToBeat();
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
            var id = resultsManager.Value.GetCurrentJumperId();
            onNewJumper.Invoke();
            skiJumperDataController.SetValues(bibColors[id]);
        }

        public void UpdateToBeat()
        {
            if (resultsManager.Value.StartListIndex == 0 && resultsManager.Value.SubroundIndex == 0)
                jumpData.InitGate = jumpData.Gate;
            toBeatLineController.CompensationPoints =
                (float) (hillInfo.GetGatePoints(jumpData.GatesDiff) + hillInfo.GetWindPoints(jumpData.Wind));
            onWindGateChanged.Invoke();
        }
    }
}