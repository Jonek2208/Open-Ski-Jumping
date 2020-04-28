using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ListView;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.JumpersMenu;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.CalendarEditor.Competitors
{
    public class CalendarEditorJumpersView : MonoBehaviour, ICalendarEditorJumpersView
    {
        private bool initialized;
        private CalendarEditorJumpersPresenter presenter;
        [SerializeField] private CompetitorsRuntime jumpersRuntime;
        [SerializeField] private CalendarFactory calendarFactory;

        [SerializeField] private FlagsData flagsData;
        [SerializeField] private Sprite[] genderIcons;

        [SerializeField] private JumpersListView listView;
        [SerializeField] private ToggleGroupExtension toggleGroup;
        [SerializeField] private Toggle allElementsToggle;

        private List<Competitor> jumpers;

        private HashSet<Competitor> selectedJumpers = new HashSet<Competitor>();

        public IEnumerable<Competitor> SelectedJumpers
        {
            get => selectedJumpers.ToList();
            set => selectedJumpers = new HashSet<Competitor>(value);
        }

        public IEnumerable<Competitor> Jumpers
        {
            set
            {
                jumpers = value.ToList();
                listView.Items = jumpers;
                listView.Refresh();
            }
        }

        private void OnDisable()
        {
            OnDataSave?.Invoke();
        }

        private void Start()
        {
            toggleGroup.OnSelectionChanged += HandleSelectionChanged;
            listView.Initialize(BindListViewItem);
            allElementsToggle.onValueChanged.AddListener(HandleAllElementsToggle);
            presenter = new CalendarEditorJumpersPresenter(this, jumpersRuntime, flagsData, calendarFactory);
            allElementsToggle.SetIsOnWithoutNotify(selectedJumpers.Count > 0);
            initialized = true;
        }

        public event Action OnDataSave;
        public event Action OnDataReload;
        public void SelectionSave() => OnDataSave?.Invoke();

        private void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
            listView.Reset();
            allElementsToggle.SetIsOnWithoutNotify(selectedJumpers.Count > 0);
        }


        private void BindListViewItem(int index, JumpersListItem item)
        {
            var competitor = jumpers[index];
            item.nameText.text = $"{competitor.firstName} {competitor.lastName.ToUpper()}";
            item.countryFlagText.text = competitor.countryCode;
            item.countryFlagImage.sprite = flagsData.GetFlag(competitor.countryCode);
            item.genderIconImage.sprite = genderIcons[(int) competitor.gender];
            item.toggleExtension.SetElementId(index);
            item.toggleExtension.Toggle.isOn = selectedJumpers.Contains(competitor);
        }

        private void HandleSelectionChanged(int index, bool value)
        {
            var jumper = jumpers[index];
            if (value)
            {
                if (!selectedJumpers.Contains(jumper))
                    selectedJumpers.Add(jumper);
            }
            else
            {
                selectedJumpers.Remove(jumper);
            }

            allElementsToggle.SetIsOnWithoutNotify(selectedJumpers.Count > 0);
        }

        private void HandleAllElementsToggle(bool value)
        {
            if (value)
            {
                foreach (var jumper in jumpers) selectedJumpers.Add(jumper);
            }
            else
            {
                selectedJumpers.Clear();
            }

            listView.RefreshShownValue();
        }
    }
}