using System;
using OpenSkiJumping.Data;
using UnityEngine;
using EventType = OpenSkiJumping.Competition.EventType;

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
        [SerializeField] private MainMenuController menuController;

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
            menuController.LoadTournament();
        }

        public void LoadMainMenu()
        {
            menuController.LoadMainMenu();
        }
    }
}