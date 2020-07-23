using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.CalendarEditor.Classifications;
using OpenSkiJumping.UI.ListView;
using UnityEngine;

namespace OpenSkiJumping.UI.TournamentMenu.ResultsMenu
{
    public class ClassificationsSelectionView : MonoBehaviour, IClassificationsSelectionView
    {
        private List<ClassificationInfo> classifications;
        [SerializeField] private IconsData iconsData;
        private bool initialized;

        [SerializeField] private ClassificationsListView listView;
        private ClassificationsSelectionPresenter presenter;

        [SerializeField] private ResultsListController resultsListController;

        [SerializeField] private TournamentMenuData tournamentMenuData;

        public ClassificationInfo SelectedClassification
        {
            get => listView.SelectedIndex < 0 ? null : classifications[listView.SelectedIndex];
            set => SelectClassification(value);
        }

        public int CurrentClassificationIndex => listView.SelectedIndex;

        public IEnumerable<ClassificationInfo> Classifications
        {
            set
            {
                classifications = value.ToList();
                listView.Items = classifications;
                listView.ClampSelectedIndex();
                listView.Refresh();
            }
        }

        public IResultsListController ResultsListController => resultsListController;

        public event Action OnSelectionChanged;
        public event Action OnDataReload;

        private void SelectClassification(ClassificationInfo classification)
        {
            listView.SelectedIndex =
                classification == null ? listView.SelectedIndex : classifications.IndexOf(classification);

            listView.ClampSelectedIndex();
            listView.ScrollToIndex(listView.SelectedIndex);
            listView.RefreshShownValue();
        }


        private void Start()
        {
            ListViewSetup();
            presenter = new ClassificationsSelectionPresenter(this, tournamentMenuData);
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
            listView.OnSelectionChanged += x => OnSelectionChanged?.Invoke();
            listView.SelectionType = SelectionType.Single;
            listView.Initialize(BindListViewItem);
        }

        private void BindListViewItem(int index, ClassificationsListItem listItem)
        {
            var item = classifications[index];

            listItem.nameText.text = $"{item.name}";
            listItem.bibImage.color = SimpleColorPicker.Hex2Color(item.leaderBibColor);
            listItem.classificationTypeImage.sprite = iconsData.GetClassificationTypeIcon(item.classificationType);
            listItem.eventTypeImage.sprite = iconsData.GetEventTypeIcon(item.eventType);
        }
    }
}