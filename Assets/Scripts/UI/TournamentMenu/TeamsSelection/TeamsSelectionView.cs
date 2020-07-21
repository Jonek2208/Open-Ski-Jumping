using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.ListView;
using UnityEngine;

namespace OpenSkiJumping.UI.TournamentMenu.TeamsSelection
{
    public class TeamsSelectionView : MonoBehaviour, ITeamsSelectionView
    {
        private bool initialized;
        private TeamsSelectionPresenter presenter;
        [SerializeField] private TournamentMenuController tournamentMenuController;

        [SerializeField] private TournamentMenuData tournamentMenuData;
        [SerializeField] private FlagsData flagsData;

        [SerializeField] private TeamsSelectionListView listView;
        [SerializeField] private ToggleGroupExtension toggleGroupExtension;


        private List<TeamData> teams;

        public IEnumerable<TeamData> Teams
        {
            set
            {
                teams = value.ToList();
                listView.Items = teams;
                listView.ClampSelectedIndex();
                listView.Refresh();
            }
        }

        public void RefreshTeams()
        {
            listView.RefreshShownValue();
        }

        public event Action<TeamData, bool> OnChangeTeamState;
        public event Action<TeamData> OnEditRequest;
        public event Action OnDataReload;

        private void Start()
        {
            ListViewSetup();
            presenter = new TeamsSelectionPresenter(this, tournamentMenuData, tournamentMenuController);
            initialized = true;
        }

        private void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
            listView.Reset();
        }


        private void ListViewSetup()
        {
            toggleGroupExtension.OnSelectionChanged += HandleSelectionChanged;
            listView.ConfigureItem = ConfigureItem;
            listView.SelectionType = SelectionType.None;
            listView.Initialize(BindListViewItem);
        }

        private void BindListViewItem(int index, TeamsSelectionListItem listItem)
        {
            var item = teams[index];

            var it = 0;
            foreach (var jumper in item.GetTeamMembers())
            {
                listItem.nameText[it].text = $"{jumper.firstName} {jumper.lastName.ToUpper()}";
                it++;
            }

            listItem.countryFlagText.text = item.team.countryCode;
            listItem.countryFlagImage.sprite = flagsData.GetFlag(item.team.countryCode);

            listItem.toggleExtension.SetElementId(index);
            listItem.toggleExtension.Toggle.SetIsOnWithoutNotify(item.registered);
        }

        private void ConfigureItem(TeamsSelectionListItem listItem)
        {
            listItem.editButton.onClick.AddListener(() =>
                OnEditRequest?.Invoke(teams[listItem.toggleExtension.ElementId]));
        }

        private void HandleSelectionChanged(int index, bool value)
        {
            var item = teams[index];
            OnChangeTeamState?.Invoke(item, value);
        }
    }
}