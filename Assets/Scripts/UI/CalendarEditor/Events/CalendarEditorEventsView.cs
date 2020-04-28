using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using OpenSkiJumping.ListView;
using OpenSkiJumping.UI.CalendarEditor.Classifications;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace OpenSkiJumping.UI.CalendarEditor.Events
{
    public class CalendarEditorEventsView : MonoBehaviour, ICalendarEditorEventsView
    {
        private bool initialized;
        private CalendarEditorEventsPresenter presenter;

        [SerializeField] private CalendarFactory calendarFactory;
        [SerializeField] private HillsRuntime hillsRuntime;
        [SerializeField] private PresetsRuntime presets;

        [SerializeField] private Sprite[] eventTypeIcons;

        [SerializeField] private EventsListView listView;

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


        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;

        #endregion

        private List<EventInfo> events;
        private List<EventRoundsInfo> presetsList;

        public EventInfo SelectedEvent
        {
            get => events[listView.SelectedIndex];
            set => SelectEvent(value);
        }

        private void SelectEvent(EventInfo value)
        {
            int index = (value == null) ? listView.SelectedIndex : events.IndexOf(value);
            index = Mathf.Clamp(index, 0, events.Count - 1);
            listView.SelectedIndex = index;
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        public IEnumerable<EventInfo> Events
        {
            set
            {
                events = value.ToList();
                listView.Items = events;
                listView.SelectedIndex = Mathf.Clamp(listView.SelectedIndex, 0, events.Count - 1);
                listView.Refresh();
            }
        }

        public EventRoundsInfo SelectedRoundsInfo
        {
            get => presetsList[presetsDropdown.value];
            set => SelectPreset(value);
        }


        private void SelectPreset(EventRoundsInfo value)
        {
            int index = (value == null)
                ? presetsDropdown.value
                : presetsList.FindIndex(item => item.name == value.name);
            index = Mathf.Clamp(index, 0, presetsList.Count - 1);
            presetsDropdown.SetValueWithoutNotify(index);
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


        #region ClassificationsSelectListView

        private List<ClassificationInfo> classifications;
        private HashSet<ClassificationInfo> selectedClassifications = new HashSet<ClassificationInfo>();

        public IEnumerable<ClassificationInfo> SelectedClassifications
        {
            get => selectedClassifications.ToList();
            set => selectedClassifications = new HashSet<ClassificationInfo>(value);
        }

        public IEnumerable<ClassificationInfo> Classifications
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
            get => hills[hillsDropdown.value];
            set => SelectHill(value);
        }

        private void SelectHill(ProfileData value)
        {
            if (value == null) return;
            int index = hills.IndexOf(value);
            hillsDropdown.SetValueWithoutNotify(index);
        }

        #endregion

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

        public string QualRankId
        {
            get => qualRankDropdown.options[qualRankDropdown.value].text;
            set => SelectRank(value, qualRankDropdown);
        }

        public int OrdRankType
        {
            get => ordRankSelect.selectedSegmentIndex;
            set => ordRankSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public string OrdRankId
        {
            get => ordRankDropdown.options[ordRankDropdown.value].text;
            set => SelectRank(value, ordRankDropdown);
        }

        private void SelectRank(string value, TMP_Dropdown dropdown)
        {
            if (value == null) return;
            int index = dropdown.options.FindIndex(item => item.text == value);

            index = Mathf.Clamp(index, 0, dropdown.options.Count - 1);
            dropdown.SetValueWithoutNotify(index);
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
        public event Action OnDataReload;

        public bool EventInfoEnabled
        {
            set
            {
                if (value) ShowClassificationInfo();
                else HideClassificationInfo();
            }
        }

        private void ShowClassificationInfo()
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
                qualRankDropdown.AddOptions(classifications.Select(item => item.name).ToList());
            }

            if (SelectedEvent.qualRankType == RankType.Event)
            {
                qualRankDropdown.ClearOptions();
                qualRankDropdown.AddOptions(events.Select(item => item.name).ToList());
            }
            
            SelectRank(SelectedEvent.qualRankId, qualRankDropdown);


            ordRankDropdown.gameObject.SetActive(SelectedEvent.ordRankType != RankType.None);
            if (SelectedEvent.ordRankType == RankType.Classification)
            {
                ordRankDropdown.ClearOptions();
                ordRankDropdown.AddOptions(classifications.Select(item => item.name).ToList());
            }

            if (SelectedEvent.ordRankType == RankType.Event)
            {
                ordRankDropdown.ClearOptions();
                ordRankDropdown.AddOptions(events.Select(item => item.name).ToList());
            }
            
            SelectRank(SelectedEvent.ordRankId, ordRankDropdown);

        }

        private void HideClassificationInfo() => eventInfoObj.SetActive(false);


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
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            moveUpButton.onClick.AddListener(() => OnMoveUp?.Invoke());
            moveDownButton.onClick.AddListener(() => OnMoveDown?.Invoke());

            eventTypeSelect.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            eventTypeSelect.onValueChanged.AddListener(arg => ShowClassificationInfo());

            presetsDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            hillsDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());

            inLimitSelect.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            inLimitSelect.onValueChanged.AddListener(arg => ShowClassificationInfo());
            inLimitInput.onEndEdit.AddListener(arg => OnCurrentEventChanged?.Invoke());

            qualRankSelect.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            qualRankSelect.onValueChanged.AddListener(arg => ShowClassificationInfo());
            qualRankDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());

            ordRankSelect.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            ordRankSelect.onValueChanged.AddListener(arg => ShowClassificationInfo());
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
            item.nameText.text = $"{classification.name}";
            item.toggleExtension.SetElementId(index);
            item.toggleExtension.Toggle.isOn = selectedClassifications.Contains(classification);
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
    }
}