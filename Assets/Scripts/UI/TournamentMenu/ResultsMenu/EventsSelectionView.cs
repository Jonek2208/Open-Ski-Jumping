using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.CalendarEditor.Events;
using OpenSkiJumping.UI.ListView;
using UnityEngine;

namespace OpenSkiJumping.UI.TournamentMenu.ResultsMenu
{
    public class EventsSelectionView : MonoBehaviour, IEventsSelectionView
    {
        private List<EventInfo> events;
        [SerializeField] private IconsData iconsData;
        private bool initialized;

        [SerializeField] private EventsListView listView;
        private EventsSelectionPresenter presenter;

        [SerializeField] private ResultsListController resultsListController;

        [SerializeField] private TournamentMenuData tournamentMenuData;

        public EventInfo SelectedEvent
        {
            get => listView.SelectedIndex < 0 ? null : events[listView.SelectedIndex];
            set => SelectEvent(value);
        }

        public int CurrentEventIndex => listView.SelectedIndex;

        public IEnumerable<EventInfo> Events
        {
            set
            {
                events = value.ToList();
                listView.Items = events;
                listView.ClampSelectedIndex();
                listView.Refresh();
            }
        }

        public IResultsListController ResultsListController => resultsListController;

        public event Action OnSelectionChanged;
        public event Action OnDataReload;

        private void SelectEvent(EventInfo item)
        {
            listView.SelectedIndex =
                item == null ? listView.SelectedIndex : events.IndexOf(item);

            listView.ClampSelectedIndex();
            listView.ScrollToIndex(listView.SelectedIndex);
            listView.RefreshShownValue();
        }


        private void Start()
        {
            ListViewSetup();
            resultsListController.Initialize();
            presenter = new EventsSelectionPresenter(this, tournamentMenuData);
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

        private void BindListViewItem(int index, EventsListItem listItem)
        {
            var item = events[index];

            listItem.idText.text = $"{index + 1}";
            listItem.nameText.text = $"{item.hillId}";
            listItem.eventTypeImage.sprite = iconsData.GetEventTypeIcon(item.eventType);
        }
    }
}