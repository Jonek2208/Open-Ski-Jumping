using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using OpenSkiJumping.ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace OpenSkiJumping.UI.CalendarEditor.Events
{
    public class CalendarEditorEventsView : MonoBehaviour, ICalendarEditorEventsView
    {
        [SerializeField] private CalendarFactory calendarFactory;

        private List<EventInfo> events;

        [SerializeField] private Sprite[] eventTypeIcons;
        [SerializeField] private HillsRuntime hillsRuntime;
        private bool initialized;

        [SerializeField] private EventsListView listView;
        private CalendarEditorEventsPresenter presenter;
        [SerializeField] private PresetsRuntime presets;
        private List<EventRoundsInfo> presetsList;

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

        public EventRoundsInfo SelectedRoundsInfo
        {
            get => presetsList[presetsDropdown.value];
            set => SelectPreset(value);
        }

        public IEnumerable<EventRoundsInfo> RoundsInfos
        {
            set
            {
                presetsList = value.ToList();
                presetsDropdown.ClearOptions();
                presetsDropdown.AddOptions(presetsList.Select(item => item.name).ToList());
            }
        }

        public int EventType
        {
            get => eventTypeSelect.selectedSegmentIndex;
            set => eventTypeSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public int QualRankType
        {
            get => qualRankSelect.selectedSegmentIndex;
            set => qualRankSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public int QualRankId
        {
            get => qualRankDropdown.value;
            set => qualRankDropdown.SetValueWithoutNotify(value);
        }

        public int OrdRankType
        {
            get => ordRankSelect.selectedSegmentIndex;
            set => ordRankSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public int OrdRankId
        {
            get => ordRankDropdown.value;
            set => ordRankDropdown.SetValueWithoutNotify(value);
        }

        public int InLimitType
        {
            get => inLimitSelect.selectedSegmentIndex;
            set => inLimitSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public int InLimit
        {
            get => int.Parse(inLimitInput.text);
            set => inLimitInput.SetTextWithoutNotify(value.ToString());
        }

        public event Action OnSelectionChanged;
        public event Action OnCurrentEventChanged;
        public event Action OnAdd;
        public event Action OnRemove;
        public event Action OnMoveUp;
        public event Action OnMoveDown;
        public event Action OnDuplicate;
        public event Action OnDataReload;

        public bool EventInfoEnabled
        {
            set
            {
                if (value) ShowInfo();
                else HideInfo();
            }
        }

        private void SelectEvent(EventInfo value)
        {
            listView.SelectedIndex = value == null ? listView.SelectedIndex : events.IndexOf(value);
            listView.ClampSelectedIndex();
            listView.ScrollToIndex(listView.SelectedIndex);
            listView.RefreshShownValue();
        }


        private void SelectPreset(EventRoundsInfo value)
        {
            var index = value == null
                ? presetsDropdown.value
                : presetsList.FindIndex(item => item.name == value.name);
            index = Mathf.Clamp(index, 0, presetsList.Count - 1);
            presetsDropdown.SetValueWithoutNotify(index);
        }

        private void ShowInfo()
        {
            eventInfoObj.SetActive(true);
            // if (SelectedClassification.classificationType == Competition.ClassificationType.Place)
            // {
            //     indTableObj.SetActive(true);
            //     teamTableObj.SetActive(SelectedClassification.eventType == Competition.EventType.Team);
            // }
            // else
            // {
            //     indTableObj.SetActive(false);
            //     teamTableObj.SetActive(false);
            // }
            //
            inLimitInput.gameObject.SetActive(SelectedEvent.inLimitType != LimitType.None);

            qualRankDropdown.gameObject.SetActive(SelectedEvent.qualRankType != RankType.None);
            if (SelectedEvent.qualRankType == RankType.Classification)
            {
                qualRankDropdown.ClearOptions();
                qualRankDropdown.AddOptions(classifications.Select(item => item.Name).ToList());
            }

            if (SelectedEvent.qualRankType == RankType.Event)
            {
                qualRankDropdown.ClearOptions();
                qualRankDropdown.AddOptions(events.Select((item, index) => $"{index + 1} {item.hillId}").ToList());
            }

            qualRankDropdown.SetValueWithoutNotify(SelectedEvent.qualRankId);


            ordRankDropdown.gameObject.SetActive(SelectedEvent.ordRankType != RankType.None);
            if (SelectedEvent.ordRankType == RankType.Classification)
            {
                ordRankDropdown.ClearOptions();
                ordRankDropdown.AddOptions(classifications.Select(item => item.Name).ToList());
            }

            if (SelectedEvent.ordRankType == RankType.Event)
            {
                ordRankDropdown.ClearOptions();
                ordRankDropdown.AddOptions(events.Select((item, index) => $"{index + 1} {item.hillId}").ToList());
            }

            ordRankDropdown.SetValueWithoutNotify(SelectedEvent.ordRankId);
        }

        private void HideInfo()
        {
            eventInfoObj.SetActive(false);
        }


        private void Start()
        {
            ListViewSetup();
            RegisterCallbacks();
            presenter = new CalendarEditorEventsPresenter(this, calendarFactory, hillsRuntime, presets);
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

            classificationToggleGroupExtension.OnSelectionChanged += HandleClassificationsSelectionChanged;
            classificationsListView.SelectionType = SelectionType.None;
            classificationsListView.Initialize(BindClassificationsListViewItem);
            // allElementsToggle.onValueChanged.AddListener(HandleAllElementsToggle);
        }

        private void RegisterCallbacks()
        {
            duplicateButton.onClick.AddListener(() => OnDuplicate?.Invoke());
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            moveUpButton.onClick.AddListener(() => OnMoveUp?.Invoke());
            moveDownButton.onClick.AddListener(() => OnMoveDown?.Invoke());

            eventTypeSelect.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            eventTypeSelect.onValueChanged.AddListener(arg => ShowInfo());

            presetsDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            hillsDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());

            inLimitSelect.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            inLimitSelect.onValueChanged.AddListener(arg => ShowInfo());
            inLimitInput.onEndEdit.AddListener(arg => OnCurrentEventChanged?.Invoke());

            qualRankSelect.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            qualRankSelect.onValueChanged.AddListener(arg => ShowInfo());
            qualRankDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());

            ordRankSelect.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            ordRankSelect.onValueChanged.AddListener(arg => ShowInfo());
            ordRankDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
        }

        private void BindListViewItem(int index, EventsListItem item)
        {
            var eventInfo = events[index];
            item.idText.text = $"{index + 1}";
            item.nameText.text = $"{eventInfo.hillId}";
            item.eventTypeImage.sprite = eventTypeIcons[(int) eventInfo.eventType];
        }

        private void BindClassificationsListViewItem(int index, ClassificationsSelectListItem item)
        {
            var classification = classifications[index];
            item.nameText.text = $"{classification.Name}";
            item.toggleExtension.SetElementId(index);
            item.toggleExtension.Toggle.SetIsOnWithoutNotify(selectedClassifications.Contains(classification));
        }

        private void HandleClassificationsSelectionChanged(int index, bool value)
        {
            var item = classifications[index];
            if (value)
            {
                if (!selectedClassifications.Contains(item))
                    selectedClassifications.Add(item);
            }
            else
            {
                selectedClassifications.Remove(item);
            }

            OnCurrentEventChanged?.Invoke();

            // allElementsToggle.SetIsOnWithoutNotify(selectedClassifications.Count > 0);
        }

        #region EventInfoUI

        [SerializeField] private GameObject eventInfoObj;

        [SerializeField] private SegmentedControl eventTypeSelect;
        [SerializeField] private TMP_Dropdown presetsDropdown;
        [SerializeField] private TMP_Dropdown hillsDropdown;

        [SerializeField] private ToggleGroupExtension classificationToggleGroupExtension;
        [SerializeField] private ClassificationsSelectListView classificationsListView;

        [SerializeField] private SegmentedControl inLimitSelect;
        [SerializeField] private TMP_InputField inLimitInput;
        [SerializeField] private SegmentedControl qualRankSelect;
        [SerializeField] private TMP_Dropdown qualRankDropdown;
        [SerializeField] private SegmentedControl ordRankSelect;
        [SerializeField] private TMP_Dropdown ordRankDropdown;

        // [SerializeField] private GameObject indTableObj;
        // [SerializeField] private GameObject teamTableObj;
        // [SerializeField] private GameObject limitObj;
        // [SerializeField] private GameObject medalsObj;


        [SerializeField] private Button duplicateButton;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;

        #endregion


        #region ClassificationsSelectListView

        private List<ClassificationData> classifications;
        private HashSet<ClassificationData> selectedClassifications = new HashSet<ClassificationData>();

        public IEnumerable<ClassificationData> SelectedClassifications
        {
            get => selectedClassifications.ToList();
            set
            {
                selectedClassifications = new HashSet<ClassificationData>(value);
                classificationsListView.Refresh();
            }
        }

        public IEnumerable<ClassificationData> Classifications
        {
            set
            {
                classifications = value.ToList();
                classificationsListView.Items = classifications;
                classificationsListView.Refresh();
            }
        }

        #endregion


        #region Hills

        private List<ProfileData> hills;

        public IEnumerable<ProfileData> Hills
        {
            set
            {
                hills = value.ToList();
                hillsDropdown.ClearOptions();
                hillsDropdown.AddOptions(value.Select(item => item.name).ToList());
            }
        }

        public ProfileData SelectedHill
        {
            get => hillsDropdown.value < 0 ? null : hills[hillsDropdown.value];
            set => SelectHill(value);
        }

        private void SelectHill(ProfileData value)
        {
            if (value == null) return;
            var index = hills.IndexOf(value);
            hillsDropdown.SetValueWithoutNotify(index);
        }

        #endregion
    }
}