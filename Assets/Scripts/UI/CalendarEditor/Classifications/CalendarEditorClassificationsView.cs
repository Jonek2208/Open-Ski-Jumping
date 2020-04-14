using System;
using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;
using ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace UI.CalendarEditor.Classifications
{
    public class CalendarEditorClassificationsView : MonoBehaviour, ICalendarEditorClassificationsView
    {
        private CalendarEditorClassificationsPresenter presenter;

        [SerializeField] private CalendarFactory calendarFactory;
        [SerializeField] private Sprite[] eventTypeIcons;
        [SerializeField] private Sprite[] classificationTypeIcons;

        [SerializeField] private ClassificationsListView listView;
        [SerializeField] private ToggleGroupExtension toggleGroup;

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


        [SerializeField] private GameObject indTableObj;
        [SerializeField] private GameObject teamTableObj;
        [SerializeField] private GameObject limitObj;
        [SerializeField] private GameObject medalsObj;


        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

        #endregion

        public ClassificationInfo SelectedClassification => classifications[toggleGroup.CurrentValue];

        private List<ClassificationInfo> classifications;
        private IEnumerable<PointsTable> pointsTablesIndividual;
        private IEnumerable<PointsTable> pointsTablesTeam;

        public IEnumerable<ClassificationInfo> Classifications
        {
            set
            {
                classifications = value.ToList();
                listView.Items = classifications;
                FixCurrentSelection();
                listView.Refresh();
            }
        }

        public PointsTable SelectedPointsTableIndividual { get; set; }

        public IEnumerable<PointsTable> PointsTablesIndividual
        {
            get => pointsTablesIndividual;
            set => pointsTablesIndividual = value;
        }

        public PointsTable SelectedPointsTableTeam { get; set; }

        public IEnumerable<PointsTable> PointsTablesTeam
        {
            get => pointsTablesTeam;
            set => pointsTablesTeam = value;
        }

        public string Name
        {
            get => nameInput.text;
            set => nameInput.text = value;
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
            set => limitInput.text = value.ToString();
        }

        public int MedalPlaces
        {
            get => int.Parse(medalPlacesInput.text);
            set => medalPlacesInput.text = value.ToString();
        }

        public bool BlockJumperInfoCallbacks { get; set; }
        public bool BlockSelectionCallbacks { get; set; }

        public event Action OnSelectionChanged;
        public event Action OnCurrentClassificationChanged;
        public event Action OnAdd;
        public event Action OnRemove;

        private void Start()
        {
            toggleGroup.OnValueChanged += x => { HandleSelectionChanged(); };
            listView.Initialize(BindListViewItem);
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            RegisterCallbacks();
            presenter = new CalendarEditorClassificationsPresenter(this, calendarFactory);
        }

        private void FixCurrentSelection()
        {
            toggleGroup.HandleSelectionChanged(Mathf.Clamp(toggleGroup.CurrentValue, 0, classifications.Count - 1));
        }


        private void HandleSelectionChanged()
        {
            if (!BlockSelectionCallbacks) OnSelectionChanged?.Invoke();
        }

        private void RegisterCallbacks()
        {
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
        }

        private void OnValueChanged()
        {
            if (!BlockJumperInfoCallbacks) OnCurrentClassificationChanged?.Invoke();
        }

        public void SelectClassification(ClassificationInfo classification)
        {
            int index = (classification == null) ? toggleGroup.CurrentValue : classifications.IndexOf(classification);
            index = Mathf.Clamp(index, 0, classifications.Count - 1);
            toggleGroup.HandleSelectionChanged(index);
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        public void HideClassificationInfo() => classificationInfoObj.SetActive(false);

        public void ShowClassificationInfo()
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
            item.classificationTypeImage.sprite = classificationTypeIcons[(int) classificationInfo.classificationType];
            item.eventTypeImage.sprite = eventTypeIcons[(int) classificationInfo.eventType];
            item.toggleExtension.SetElementId(index);
        }
    }
}