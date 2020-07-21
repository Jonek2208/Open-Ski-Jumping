using System;
using OpenSkiJumping.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = OpenSkiJumping.Competition.EventType;
using Object = UnityEngine.Object;

namespace OpenSkiJumping.UI.TournamentMenu
{
    public class TournamentMenuController : MonoBehaviour, ITournamentMenuController
    {
        [SerializeField] private TournamentMenuData tournamentMenuData;
        [SerializeField] private GameObject classificationsHierarchyGO;
        [SerializeField] private SavesRuntime saves;
        [SerializeField] private GameObject teamSquadGO;
        [SerializeField] private GameObject jumpersListGO;
        [SerializeField] private GameObject teamsListGO;
        [SerializeField] private int competitionSceneIndex;

        private void Awake()
        {
            var currentSave = saves.GetCurrentSave();
            tournamentMenuData.GameSave = currentSave;
            tournamentMenuData.Classifications = currentSave.classificationsData;
            tournamentMenuData.Competitors = currentSave.competitors;
            tournamentMenuData.Teams = currentSave.teams;
        }

        private void Start()
        {
            classificationsHierarchyGO.SetActive(true);
            jumpersListGO.SetActive(tournamentMenuData.GetCurrentEvent().eventType == EventType.Individual);
            teamsListGO.SetActive(tournamentMenuData.GetCurrentEvent().eventType == EventType.Team);
        }

        public event Action OnReloadTeamsList;

        public void ShowTeamSquad()
        {
            teamSquadGO.SetActive(true);
        }

        public void HideTeamSquad()
        {
            teamSquadGO.SetActive(false);
            OnReloadTeamsList?.Invoke();
        }

        public void LoadCompetition()
        {
            SceneManager.LoadScene(competitionSceneIndex);
        }
    }
}