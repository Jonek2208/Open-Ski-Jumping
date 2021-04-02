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
        private IHillInfo _hillInfo;
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

        private Dictionary<int, Color> _bibColors;


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
                PointsUtils.UpdateClassificationResults(classificationInfo, classificationResults, resultsUpdate);
            }
        }

        public void OnCompetitionStart()
        {
            var save = savesRepository.GetCurrentSave();
            var eventId = save.resultsContainer.eventIndex;
            var currentEventInfo = save.calendar.events[eventId];

            competitors.competitors = save.competitors.Select(it => it.competitor).ToList();
            competitors.teams = save.teams.Select(it => it.team).ToList();

            var eventParticipants = EventProcessor.EventParticipants(save, eventId);
            save.resultsContainer.eventResults[eventId] = new EventResults {participants = eventParticipants};
            var orderedParticipants = GetOrderedParticipants(eventParticipants, save);

            CalculateBibs(save, orderedParticipants);
            HillSetUp(save, eventId, currentEventInfo);

            resultsManager.Initialize(currentEventInfo, orderedParticipants, _hillInfo);
            SetDefaultJumpData();
            windGatePanel.Initialize(hill.profileData.Value.gates);
            onCompetitionStart.Invoke();
            OnRoundStart();
            OnSubroundStart();
            OnJumpStart();
        }

        private static List<Participant> GetOrderedParticipants(IEnumerable<Participant> eventParticipants,
            GameSave save)
        {
            var participantsDict = eventParticipants.Select(item => item)
                .ToDictionary(item => item.id, item => item);
            var orderedParticipants = EventProcessor.GetCompetitors(save.calendar, save.resultsContainer)
                .Select(it => participantsDict[it]).ToList();
            return orderedParticipants;
        }

        private void SetDefaultJumpData()
        {
            jumpData.Gate = jumpData.InitGate = 1;
            jumpData.Wind = 0;
        }

        private void CalculateBibs(GameSave save, IEnumerable<Participant> orderedParticipants)
        {
            _bibColors = orderedParticipants.SelectMany(it => it.competitors).ToDictionary(it => it, it => Color.white);
            var orderedClassifications = save.classificationsData.Where(it => it.useBib)
                .Select(it => (it.calendarId, it.priority)).Reverse();

            foreach (var (it, ind) in orderedClassifications)
            {
                var classificationResults = save.resultsContainer.classificationResults[it];
                var classificationInfo = save.classificationsData[it].classification;
                foreach (var id in classificationResults.totalSortedResults.TakeWhile(jumperId =>
                    classificationResults.rank[jumperId] <= 1))
                {
                    var bibColor =
                        SimpleColorPicker.Hex2Color(save.classificationsData[ind].classification.leaderBibColor);

                    if (classificationInfo.eventType == EventType.Individual)
                    {
                        _bibColors[id] = bibColor;
                    }
                    else
                    {
                        foreach (var competitor in save.teams[id].competitors)
                            _bibColors[competitor.calendarId] = bibColor;
                    }
                }
            }
        }

        private void HillSetUp(GameSave save, int eventId, EventInfo currentEventInfo)
        {
            var hillId = save.calendar.events[eventId].hillId;
            hill.profileData.Value = hillsRepository.GetProfileData(hillId);
            hill.landingAreaSO = hillsFactory.landingAreas[(int) currentEventInfo.hillSurface].Value;
            var track = currentEventInfo.hillSurface == HillSurface.Matting
                ? hill.profileData.Value.inrunData.summerTrack
                : hill.profileData.Value.inrunData.winterTrack;
            hill.inrunTrackSO = hillsFactory.inrunTracks[(int) track].Value;
            hill.GenerateMesh();

            _hillInfo = hillsRepository.GetHillInfo(hillId);
            var (head, tail, gate) = compensationsJumpSimulator.GetCompensations();
            _hillInfo.SetCompensations(head, tail, gate);
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
            skiJumperDataController.SetValues(_bibColors[id]);
        }

        public void UpdateToBeat()
        {
            if (resultsManager.Value.StartListIndex == 0 && resultsManager.Value.SubroundIndex == 0)
                jumpData.InitGate = jumpData.Gate;
            toBeatLineController.CompensationPoints =
                (float) (_hillInfo.GetGatePoints(jumpData.GatesDiff) + _hillInfo.GetWindPoints(jumpData.Wind));
            onWindGateChanged.Invoke();
        }
    }
}