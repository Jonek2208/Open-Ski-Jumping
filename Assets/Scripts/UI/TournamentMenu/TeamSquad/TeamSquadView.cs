using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.TournamentMenu.TeamSquad
{
    public class TeamSquadView : MonoBehaviour, ITeamSquadView
    {
        private TeamSquadPresenter presenter;
        private bool initialized;
        [SerializeField] private TournamentMenuController tournamentMenuController;
        [SerializeField] private TournamentMenuData tournamentMenuData;
        [SerializeField] private IconsData iconsData;
        [SerializeField] private TeamSquadListView listView;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;
        [SerializeField] private Button backButton;

        [SerializeField] private Color[] bibColors;


        private List<CompetitorData> competitors;

        public CompetitorData SelectedCompetitorData
        {
            get => listView.SelectedIndex < 0 ? null : competitors[listView.SelectedIndex];
            set => listView.SelectItem(value);
        }

        public IEnumerable<CompetitorData> Competitors
        {
            set
            {
                competitors = value.ToList();
                listView.Items = competitors;
                listView.ClampSelectedIndex();
                listView.Refresh();
            }
        }

        public event Action OnSelectionChanged;
        public event Action OnSubmit;
        public event Action OnMoveUp;
        public event Action OnMoveDown;
        public event Action OnDataReload;

        private void Start()
        {
            ListViewSetup();
            RegisterCallbacks();
            presenter = new TeamSquadPresenter(this, tournamentMenuData, tournamentMenuController);
            initialized = true;
        }

        private void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
            listView.Reset();
        }

        private void RegisterCallbacks()
        {
            moveUpButton.onClick.AddListener(() => OnMoveUp?.Invoke());
            moveDownButton.onClick.AddListener(() => OnMoveDown?.Invoke());
            backButton.onClick.AddListener(() => OnSubmit?.Invoke());
        }

        private void ListViewSetup()
        {
            listView.OnSelectionChanged += x => OnSelectionChanged?.Invoke();
            listView.SelectionType = SelectionType.Single;
            listView.Initialize(BindListViewItem);
        }

        private void BindListViewItem(int index, TeamSquadListItem listItem)
        {
            var item = competitors[index];
            listItem.nameText.text = $"{item.competitor.firstName} {item.competitor.lastName.ToUpper()}";
            if (index < 4)
            {
                listItem.bibImage.sprite = iconsData.GetBibIcon(1);
                listItem.bibImage.color = bibColors[index];
            }
            else
            {
                listItem.bibImage.sprite = iconsData.GetBibIcon(0);
                listItem.bibImage.color = Color.white;
            }

            listItem.genderIconImage.sprite = iconsData.GetGenderIcon(item.competitor.gender);
        }
    }
}