using System;
using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;
using Data;
using ListView;
using ScriptableObjects;
using UI.JumpersMenu;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CalendarEditor.Competitors
{
    public class CalendarEditorJumpersView : MonoBehaviour, ICalendarEditorJumpersView
    {
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
            set
            {
                selectedJumpers = new HashSet<Competitor>(value);
                listView.RefreshShownValue();
            }
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
            OnSelectionSave?.Invoke();
        }

        private void Start()
        {
            toggleGroup.OnSelectionChanged += HandleSelectionChanged;
            listView.Initialize(BindListViewItem);
            allElementsToggle.onValueChanged.AddListener(HandleAllElementsToggle);
            presenter = new CalendarEditorJumpersPresenter(this, jumpersRuntime, flagsData, calendarFactory);
        }

        public event Action OnSelectionSave;

        private void BindListViewItem(int index, JumpersListItem item)
        {
            item.nameText.text = $"{jumpers[index].firstName} {jumpers[index].lastName.ToUpper()}";
            item.countryFlagText.text = jumpers[index].countryCode;
            item.countryFlagImage.sprite = flagsData.GetFlag(jumpers[index].countryCode);
            item.genderIconImage.sprite = genderIcons[(int) jumpers[index].gender];
            item.toggleExtension.SetElementId(index);
            item.toggleExtension.Toggle.isOn = selectedJumpers.Contains(jumpers[index]);
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