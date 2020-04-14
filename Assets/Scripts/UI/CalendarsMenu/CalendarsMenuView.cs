using System;
using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;
using Data;
using ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CalendarsMenu
{
    public class CalendarsMenuView : MonoBehaviour, ICalendarsMenuView
    {
        private CalendarsMenuPresenter presenter;
        [SerializeField] private CalendarsRuntime calendarsRuntime;

        [SerializeField] private CalendarsListView listView;
        [SerializeField] private ToggleGroupExtension toggleGroup;

        [SerializeField] private GameObject calendarInfoObj;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

        [SerializeField] private GameObject popUpRoot;
        [SerializeField] private GameObject promptObj;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;


        public string CurrentCalendarName { set { nameText.text = value; } }
        public string NewCalendarName { get { return input.text; } }


        public event Action OnSelectionChanged;
        public event Action OnAdd;
        public event Action OnRemove;
        public event Action OnSubmit;

        private List<Calendar> calendars;
        public Calendar SelectedCalendar { get => calendars[toggleGroup.CurrentValue]; }

        private void Start()
        {
            toggleGroup.OnValueChanged += val => { OnSelectionChanged?.Invoke(); };
            listView.Initialize(BindListViewItem);
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            submitButton.onClick.AddListener(() => OnSubmit?.Invoke());
            cancelButton.onClick.AddListener(HidePopUp);
            presenter = new CalendarsMenuPresenter(this, calendarsRuntime);
        }

        private void BindListViewItem(int index, CalendarsListItem item)
        {
            item.valueText.text = calendars[index].name;
            item.toggleExtension.SetElementId(index);
        }

        public IEnumerable<Calendar> Calendars
        {
            set
            {
                calendars = value.ToList();
                listView.Items = calendars;
                FixCurrentSelection();
                listView.Refresh();
                OnSelectionChanged?.Invoke();
            }
        }

        private void FixCurrentSelection()
        {
            toggleGroup.HandleSelectionChanged(Mathf.Clamp(toggleGroup.CurrentValue, 0, calendars.Count - 1));
        }

        public void SelectCalendar(Calendar calendar)
        {
            int index = calendars.IndexOf(calendar);
            index = Mathf.Clamp(index, 0, calendars.Count - 1);
            toggleGroup.HandleSelectionChanged(index);
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        public void ShowCalendarInfo() => calendarInfoObj.SetActive(true);
        public void HideCalendarInfo() => calendarInfoObj.SetActive(false);
        public void ShowPopUp()
        {
            popUpRoot.SetActive(true);
            promptObj.SetActive(false);
            input.text = "";
        }
        public void ShowPrompt() => promptObj.SetActive(true);
        public void HidePopUp() => popUpRoot.SetActive(false);
    }
}