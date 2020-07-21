using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.CalendarsMenu
{
    public class CalendarsMenuView : MonoBehaviour, ICalendarsMenuView
    {
        private List<Calendar> calendars;
        [SerializeField] private CalendarsRuntime calendarsRuntime;

        [SerializeField] private CalendarsListView listView;
        private CalendarsMenuPresenter presenter;


        public string CurrentCalendarName
        {
            set => nameText.text = value;
        }

        public string NewCalendarName => input.text;


        public event Action OnSelectionChanged;
        public event Action OnAdd;
        public event Action OnRemove;
        public event Action OnSubmit;

        public Calendar SelectedCalendar
        {
            get => listView.SelectedIndex < 0 ? null : calendars[listView.SelectedIndex];
            set => SelectCalendar(value);
        }

        public IEnumerable<Calendar> Calendars
        {
            set
            {
                calendars = value.ToList();
                listView.Items = calendars;
                listView.SelectedIndex = Mathf.Clamp(listView.SelectedIndex, 0, calendars.Count - 1);
                listView.Refresh();
            }
        }

        public void ShowPopUp()
        {
            popUpRoot.SetActive(true);
            promptObj.SetActive(false);
            input.text = "";
        }

        public void HidePopUp()
        {
            popUpRoot.SetActive(false);
        }

        public void ShowPrompt()
        {
            promptObj.SetActive(true);
        }

        public bool CalendarInfoEnabled
        {
            set => calendarInfoObj.SetActive(value);
        }

        private void Start()
        {
            ListViewSetup();
            RegisterCallbacks();
            presenter = new CalendarsMenuPresenter(this, calendarsRuntime);
        }

        private void ListViewSetup()
        {
            listView.OnSelectionChanged += x => { OnSelectionChanged?.Invoke(); };
            listView.SelectionType = SelectionType.Single;
            listView.Initialize(BindListViewItem);
        }

        private void RegisterCallbacks()
        {
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            submitButton.onClick.AddListener(() => OnSubmit?.Invoke());
            cancelButton.onClick.AddListener(HidePopUp);
        }

        private void BindListViewItem(int index, CalendarsListItem item)
        {
            item.valueText.text = calendars[index].name;
        }

        private void SelectCalendar(Calendar calendar)
        {
            var index = calendars.IndexOf(calendar);
            index = Mathf.Clamp(index, 0, calendars.Count - 1);
            listView.SelectedIndex = index;
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        #region CalendarInfoUI

        [SerializeField] private GameObject calendarInfoObj;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

        [SerializeField] private GameObject popUpRoot;
        [SerializeField] private GameObject promptObj;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;

        #endregion
    }
}