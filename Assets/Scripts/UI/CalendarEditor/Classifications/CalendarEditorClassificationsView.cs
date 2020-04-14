using System;
using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;
using ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CalendarEditor.Classifications
{
    public class CalendarEditorClassificationsView : MonoBehaviour, ICalendarEditorClassificationsView
    {
        private CalendarEditorClassificationsPresenter presenter;

        [SerializeField] private CalendarFactory calendarFactory;

        [SerializeField] private ClassificationsListView listView;
        [SerializeField] private ToggleGroupExtension toggleGroup;

        [SerializeField] private GameObject jumperInfoObj;

        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private RadioGroup.RadioGroup eventTypeRadioGroup;
        [SerializeField] private RadioGroup.RadioGroup classificationTypeRadioGroup;
        [SerializeField] private TMP_Dropdown indPointsTableDropdown;
        [SerializeField] private TMP_Dropdown teamPointsTableDropdown;


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
                listView.ScrollToIndex(toggleGroup.CurrentValue);
                HandleSelectionChanged();
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
            get => eventTypeRadioGroup.Value;
            set => eventTypeRadioGroup.Value = value;
        }

        public int ClassificationType
        {
            get => classificationTypeRadioGroup.Value;
            set => classificationTypeRadioGroup.Value = value;
        }

        public bool BlockJumperInfoCallbacks { get; set; }
        public bool BlockSelectionCallbacks { get; set; }

        public event Action OnSelectionChanged;
        public event Action OnCurrentClassificationChanged;
        public event Action OnAdd;
        public event Action OnRemove;

        public void SelectClassification(ClassificationInfo classification)
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            // TODO
            presenter = new CalendarEditorClassificationsPresenter(this, calendarFactory);
        }

        private void FixCurrentSelection() =>
            toggleGroup.HandleSelectionChanged(Mathf.Clamp(toggleGroup.CurrentValue, 0, classifications.Count - 1));


        private void HandleSelectionChanged()
        {
            if (!BlockSelectionCallbacks)
            {
                OnSelectionChanged?.Invoke();
            }
        }

        private void RegisterCallbacks()
        {
            nameInput.onEndEdit.AddListener(x => OnValueChanged());
            //TODO
        }

        private void OnValueChanged()
        {
            if (!BlockJumperInfoCallbacks)
            {
                OnCurrentClassificationChanged?.Invoke();
            }
        }

        public void SelectJumper(ClassificationInfo classification)
        {
            int index = classifications.IndexOf(classification);
            index = Mathf.Clamp(index, 0, classifications.Count - 1);
            toggleGroup.HandleSelectionChanged(index);
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        public void HideJumperInfo() => jumperInfoObj.SetActive(false);
        public void ShowJumperInfo() => jumperInfoObj.SetActive(true);

        private void BindListViewItem(int index, ClassificationsListItem item)
        {
            item.nameText.text = $"{classifications[index].name}";
            //TODO
            // item.eventTypeImage.sprite = flagsData.GetFlag(classifications[index].countryCode);
            // item.classificationTypeImage.sprite = genderIcons[(int) classifications[index].gender];
            item.toggleExtension.SetElementId(index);
        }
    }
}