using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.ItemsSearch.Hills;
using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace OpenSkiJumping.UI.CalendarEditor.Events
{
    public class CalendarEditorEventsView : MonoBehaviour, ICalendarEditorEventsView
    {
        private bool initialized;
        private List<ClassificationData> classifications;
        private List<EventInfo> events;
        private List<ProfileData> hills;
        private CalendarEditorEventsPresenter presenter;
        private List<EventRoundsInfo> presetsList;
        private HashSet<ClassificationData> selectedClassifications = new HashSet<ClassificationData>();

        [SerializeField] private CalendarFactory calendarFactory;
        [SerializeField] private IconsData iconsData;
        [SerializeField] private HillsRuntime hillsRuntime;
        [SerializeField] private PresetsRuntime presets;

        #region UIFields

        [Header("UI Fields")] [SerializeField] private EventsListView listView;

        [SerializeField] private GameObject eventInfoObj;

        [SerializeField] private SegmentedControl eventTypeSelect;
        [SerializeField] private TMP_Dropdown presetsDropdown;

        [SerializeField] private HillsSearch hillsSearch;

        [SerializeField] private TMP_Dropdown hillsDropdown;

        [SerializeField] private SegmentedControl hillSurfaceSelect;

        [SerializeField] private ToggleGroupExtension classificationToggleGroupExtension;
        [SerializeField] private ClassificationsSelectListView classificationsListView;

        [SerializeField] private SegmentedControl ordRankSelect;
        [SerializeField] private TMP_Dropdown ordRankDropdown;

        [SerializeField] private SegmentedControl qualRankSelect;
        [SerializeField] private TMP_Dropdown qualRankDropdown;

        [SerializeField] private GameObject optionalDataGO;
        [SerializeField] private GameObject preQualLimitGO;

        [SerializeField] private SegmentedControl inLimitSelect;
        [SerializeField] private TMP_InputField inLimitInput;

        [SerializeField] private SegmentedControl preQualRankSelect;
        [SerializeField] private TMP_Dropdown preQualRankDropdown;
        [SerializeField] private SegmentedControl preQualLimitSelect;
        [SerializeField] private TMP_InputField preQualLimitInput;


        [SerializeField] private Button duplicateButton;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;

        #endregion


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

            SetUpRankTypeDropdown(ordRankDropdown, SelectedEvent.ordRankType, SelectedEvent.ordRankId);
            SetUpRankTypeDropdown(qualRankDropdown, SelectedEvent.qualRankType, SelectedEvent.qualRankId);

            if (SelectedEvent.qualRankType == RankType.None)
            {
                optionalDataGO.SetActive(false);
                return;
            }

            optionalDataGO.SetActive(true);
            inLimitInput.gameObject.SetActive(SelectedEvent.inLimitType != LimitType.None);
            SetUpRankTypeDropdown(preQualRankDropdown, SelectedEvent.preQualRankType, SelectedEvent.preQualRankId);
            if (SelectedEvent.preQualRankType == RankType.None)
            {
                preQualLimitGO.SetActive(false);
                return;
            }

            preQualLimitGO.SetActive(true);
            preQualLimitInput.gameObject.SetActive(SelectedEvent.preQualLimitType != LimitType.None);
        }

        private void HideInfo() => eventInfoObj.SetActive(false);

        private void SetUpRankTypeDropdown(TMP_Dropdown dropdown, RankType type, int rankId)
        {
            dropdown.gameObject.SetActive(type != RankType.None);
            if (type == RankType.None) return;
            dropdown.ClearOptions();
            if (type == RankType.Classification)
            {
                dropdown.AddOptions(classifications.Select(item => item.name).ToList());
            }
            else if (type == RankType.Event)
            {
                dropdown.AddOptions(events.Take(SelectedEvent.id)
                    .Select((item, index) => $"{index + 1} {item.hillId}").ToList());
            }

            dropdown.SetValueWithoutNotify(rankId);
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

            RegisterSegmentedControlCallbacks(eventTypeSelect);
            RegisterSegmentedControlCallbacks(qualRankSelect);
            RegisterSegmentedControlCallbacks(inLimitSelect);
            RegisterSegmentedControlCallbacks(preQualLimitSelect);
            RegisterSegmentedControlCallbacks(preQualRankSelect);
            RegisterSegmentedControlCallbacks(ordRankSelect);
            RegisterSegmentedControlCallbacks(hillSurfaceSelect);

            presetsDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            hillsDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            hillsDropdown.onValueChanged.AddListener(arg => ShowInfo());
            inLimitInput.onEndEdit.AddListener(arg => OnCurrentEventChanged?.Invoke());
            qualRankDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            preQualLimitInput.onEndEdit.AddListener(arg => OnCurrentEventChanged?.Invoke());
            preQualRankDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            ordRankDropdown.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());

            hillsSearch.OnValueChanged += data => { OnCurrentEventChanged?.Invoke(); };
        }

        private void RegisterSegmentedControlCallbacks(SegmentedControl item)
        {
            item.onValueChanged.AddListener(arg => OnCurrentEventChanged?.Invoke());
            item.onValueChanged.AddListener(arg => ShowInfo());
        }

        private void BindListViewItem(int index, EventsListItem item)
        {
            var eventInfo = events[index];
            item.idText.text = $"{index + 1}";
            item.nameText.text = $"{eventInfo.hillId}";
            item.eventTypeImage.sprite = iconsData.GetEventTypeIcon(eventInfo.eventType);
        }

        private void BindClassificationsListViewItem(int index, ClassificationsSelectListItem item)
        {
            var classification = classifications[index];
            item.nameText.text = $"{classification.name}";
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

        private void SelectHill(ProfileData value)
        {
            hillsSearch.SetValueWithoutNotify(value);
            // if (value == null) return;
            // var index = hills.IndexOf(value);
            // hillsDropdown.SetValueWithoutNotify(index);
        }


        #region Events

        public event Action OnSelectionChanged;
        public event Action OnCurrentEventChanged;
        public event Action OnAdd;
        public event Action OnRemove;
        public event Action OnMoveUp;
        public event Action OnMoveDown;
        public event Action OnDuplicate;
        public event Action OnDataReload;

        #endregion

        #region Properties

        public EventInfo SelectedEvent
        {
            get => listView.SelectedIndex < 0 ? null : events[listView.SelectedIndex];
            set => listView.SelectItem(value);
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

        public ProfileData SelectedHill
        {
            get => hillsSearch.SelectedItem;
            set => SelectHill(value);
        }

        public IEnumerable<ProfileData> Hills
        {
            set
            {
                hills = value.ToList();
                hillsDropdown.ClearOptions();
                hillsDropdown.AddOptions(value.Select(item => item.name).ToList());
            }
        }

        public int EventType
        {
            get => eventTypeSelect.selectedSegmentIndex;
            set => eventTypeSelect.SetSelectedSegmentWithoutNotify(value);
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

        public int PreQualRankType
        {
            get => preQualRankSelect.selectedSegmentIndex;
            set => preQualRankSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public int PreQualRankId
        {
            get => preQualRankDropdown.value;
            set => preQualRankDropdown.SetValueWithoutNotify(value);
        }

        public int PreQualLimitType
        {
            get => preQualLimitSelect.selectedSegmentIndex;
            set => preQualLimitSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public int PreQualLimit
        {
            get => int.Parse(preQualLimitInput.text);
            set => preQualLimitInput.SetTextWithoutNotify(value.ToString());
        }

        public int HillSurface
        {
            get => hillSurfaceSelect.selectedSegmentIndex;
            set => hillSurfaceSelect.SetSelectedSegmentWithoutNotify(value);
        }


        public bool EventInfoEnabled
        {
            set
            {
                if (value) ShowInfo();
                else HideInfo();
            }
        }

        #endregion
    }
}