using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.CalendarEditor.Events;
using OpenSkiJumping.UI.ListView;
using OpenSkiJumping.UI.TournamentMenu.CalendarEventInfo;
using UnityEngine;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEvents
{
    public class CalendarEventsView : MonoBehaviour, ICalendarEventsView
    {
        private bool initialized;
        private CalendarEventsPresenter presenter;
        [SerializeField] private CalendarEventInfoView eventInfoView;
        [SerializeField] private TournamentMenuData tournamentMenuData;
        [SerializeField] private IconsData iconsData;
        [SerializeField] private EventsListView listView;
        private List<EventInfo> events;

        public ICalendarEventInfoView EventInfoView => eventInfoView;

        public EventInfo SelectedEvent
        {
            get => listView.SelectedIndex < 0 ? null : events[listView.SelectedIndex];
            set => SelectEvent(value);
        }

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
        private void SelectEvent(EventInfo item)
        {
            listView.SelectedIndex =
                item == null ? listView.SelectedIndex : events.IndexOf(item);

            listView.ClampSelectedIndex();
            listView.ScrollToIndex(listView.SelectedIndex);
            listView.RefreshShownValue();
        }

        public event Action OnSelectionChanged;
        public event Action OnDataReload;

        private void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
        }

        private void Start()
        {
            ListViewSetup();
            eventInfoView.Initialize();
            presenter = new CalendarEventsPresenter(this, tournamentMenuData);
            initialized = true;
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
            listItem.nameText.text = item.hillId;
            listItem.eventTypeImage.sprite = iconsData.GetEventTypeIcon(item.eventType);
        }
    }
}