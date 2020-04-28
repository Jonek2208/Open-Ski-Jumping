using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace OpenSkiJumping.UI.CalendarEditor.Classifications
{
    public class CalendarEditorClassificationsView : MonoBehaviour, ICalendarEditorClassificationsView
    {
        private bool initialized;
        private CalendarEditorClassificationsPresenter presenter;

        [SerializeField] private CalendarFactory calendarFactory;
        [SerializeField] private PointsTablesRuntime pointsTablesData;

        [SerializeField] private Sprite[] eventTypeIcons;
        [SerializeField] private Sprite[] classificationTypeIcons;

        [SerializeField] private ClassificationsListView listView;

        #region ClassificationInfoUI

        [SerializeField] private GameObject classificationInfoObj;
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private SegmentedControl eventTypeSelect;
        [SerializeField] private SegmentedControl classificationTypeSelect;
        [SerializeField] private TMP_Dropdown indPointsTableDropdown;
        [SerializeField] private TMP_Dropdown teamPointsTableDropdown;
        [SerializeField] private TMP_InputField limitInput;
        [SerializeField] private SegmentedControl limitTypeSelect;
        [SerializeField] private TMP_InputField medalPlacesInput;
        [SerializeField] private SimpleColorPicker bibColor;


        [SerializeField] private GameObject indTableObj;
        [SerializeField] private GameObject teamTableObj;
        [SerializeField] private GameObject limitObj;
        [SerializeField] private GameObject medalsObj;


        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

        #endregion

        private List<ClassificationInfo> classifications;

        public ClassificationInfo SelectedClassification
        {
            get => classifications[listView.SelectedIndex];
            set => SelectClassification(value);
        }

        public IEnumerable<ClassificationInfo> Classifications
        {
            set
            {
                classifications = value.ToList();
                listView.Items = classifications;
                listView.SelectedIndex = Mathf.Clamp(listView.SelectedIndex, 0, classifications.Count - 1);
                listView.Refresh();
            }
        }

        private List<PointsTable> pointsTables;

        public IEnumerable<PointsTable> PointsTables
        {
            set
            {
                pointsTables = value.ToList();
                teamPointsTableDropdown.ClearOptions();
                teamPointsTableDropdown.AddOptions(pointsTables.Select(item => item.name).ToList());
                indPointsTableDropdown.ClearOptions();
                indPointsTableDropdown.AddOptions(pointsTables.Select(item => item.name).ToList());
            }
        }

        public PointsTable SelectedPointsTableIndividual
        {
            get => pointsTables[indPointsTableDropdown.value];
            set => SelectPointsTable(value, indPointsTableDropdown);
        }

        public PointsTable SelectedPointsTableTeam
        {
            get => pointsTables[teamPointsTableDropdown.value];
            set => SelectPointsTable(value, teamPointsTableDropdown);
        }

        private void SelectPointsTable(PointsTable value, TMP_Dropdown dropdown)
        {
            int index = (value == null) ? dropdown.value : pointsTables.FindIndex(item => item.name == value.name);
            index = Mathf.Clamp(index, 0, pointsTables.Count - 1);
            dropdown.SetValueWithoutNotify(index);
        }

        public string Name
        {
            get => nameInput.text;
            set => nameInput.SetTextWithoutNotify(value);
        }

        public int EventType
        {
            get => eventTypeSelect.selectedSegmentIndex;
            set => eventTypeSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public int ClassificationType
        {
            get => classificationTypeSelect.selectedSegmentIndex;
            set => classificationTypeSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public int TeamClassificationLimitType
        {
            get => limitTypeSelect.selectedSegmentIndex;
            set
            {
                if (limitTypeSelect.gameObject.activeSelf) limitTypeSelect.SetSelectedSegmentWithoutNotify(value);
            }
        }

        public int TeamClassificationLimit
        {
            get => int.Parse(limitInput.text);
            set => limitInput.SetTextWithoutNotify(value.ToString());
        }

        public int MedalPlaces
        {
            get => int.Parse(medalPlacesInput.text);
            set => medalPlacesInput.SetTextWithoutNotify(value.ToString());
        }

        public string BibColor
        {
            get => bibColor.ToHex;
            set => bibColor.SetValueWithoutNotify(value);
        }

        public event Action OnSelectionChanged;
        public event Action OnCurrentClassificationChanged;
        public event Action OnAdd;
        public event Action OnRemove;
        public event Action OnDataSave;
        public event Action OnDataReload;

        private void Start()
        {
            ListViewSetup();
            RegisterCallbacks();
            presenter = new CalendarEditorClassificationsPresenter(this, calendarFactory, pointsTablesData);
            initialized = true;
        }

        private void OnDisable()
        {
            OnDataSave?.Invoke();
        }

        private void ListViewSetup()
        {
            listView.OnSelectionChanged += x => OnSelectionChanged?.Invoke();
            listView.SelectionType = SelectionType.Single;
            listView.Initialize(BindListViewItem);
        }

        private void RegisterCallbacks()
        {
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            nameInput.onEndEdit.AddListener(arg => OnValueChanged());
            eventTypeSelect.onValueChanged.AddListener(arg => OnValueChanged());
            eventTypeSelect.onValueChanged.AddListener(arg => ShowClassificationInfo());
            classificationTypeSelect.onValueChanged.AddListener(arg => OnValueChanged());
            classificationTypeSelect.onValueChanged.AddListener(arg => ShowClassificationInfo());
            limitTypeSelect.onValueChanged.AddListener(arg => OnValueChanged());
            limitTypeSelect.onValueChanged.AddListener(arg => ShowClassificationInfo());
            limitInput.onEndEdit.AddListener(arg => OnValueChanged());
            indPointsTableDropdown.onValueChanged.AddListener(arg => OnValueChanged());
            teamPointsTableDropdown.onValueChanged.AddListener(arg => OnValueChanged());
            bibColor.OnColorChange += OnValueChanged;
        }

        private void OnValueChanged() => OnCurrentClassificationChanged?.Invoke();

        private void SelectClassification(ClassificationInfo classification)
        {
            int index = (classification == null) ? listView.SelectedIndex : classifications.IndexOf(classification);
            index = Mathf.Clamp(index, 0, classifications.Count - 1);
            listView.SelectedIndex = index;
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        public bool ClassificationInfoEnabled
        {
            set
            {
                if (value) ShowClassificationInfo();
                else HideClassificationInfo();
            }
        }

        private void HideClassificationInfo() => classificationInfoObj.SetActive(false);

        private void ShowClassificationInfo()
        {
            classificationInfoObj.SetActive(true);
            if (SelectedClassification.classificationType == Competition.ClassificationType.Place)
            {
                indTableObj.SetActive(true);
                teamTableObj.SetActive(SelectedClassification.eventType == Competition.EventType.Team);
            }
            else
            {
                indTableObj.SetActive(false);
                teamTableObj.SetActive(false);
            }

            limitInput.gameObject.SetActive(SelectedClassification.teamClassificationLimitType ==
                                            Competition.Persistent.TeamClassificationLimitType.Best);
            limitObj.SetActive(SelectedClassification.eventType == Competition.EventType.Team);
            medalsObj.SetActive(SelectedClassification.classificationType == Competition.ClassificationType.Medal);
        }

        private void BindListViewItem(int index, ClassificationsListItem item)
        {
            var classificationInfo = classifications[index];
            item.nameText.text = $"{classificationInfo.name}";
            item.bibImage.color = SimpleColorPicker.Hex2Color(classificationInfo.leaderBibColor);
            item.classificationTypeImage.sprite = classificationTypeIcons[(int) classificationInfo.classificationType];
            item.eventTypeImage.sprite = eventTypeIcons[(int) classificationInfo.eventType];
        }

        public void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
            listView.Reset();
        }
    }
}