using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.TVGraphics;
using OpenSkiJumping.UI.JumpersMenu;
using OpenSkiJumping.UI.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.CalendarEditor.Competitors
{
    public class CalendarEditorJumpersView : MonoBehaviour, ICalendarEditorJumpersView
    {
        [SerializeField] private Toggle allElementsToggle;
        [SerializeField] private CalendarFactory calendarFactory;

        [SerializeField] private FlagsData flagsData;
        [SerializeField] private IconsData iconsData;
        [SerializeField] private ImageCacher imageCache;
        
        private bool initialized;

        private List<Competitor> jumpers;
        [SerializeField] private CompetitorsRuntime jumpersRuntime;

        [SerializeField] private CalendarEditorJumpersListView listView;
        private CalendarEditorJumpersPresenter presenter;

        private HashSet<Competitor> selectedJumpers = new HashSet<Competitor>();
        [SerializeField] private ToggleGroupExtension toggleGroup;

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

        public event Action OnDataSave;
        public event Action OnDataReload;

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

        public void SelectionSave()
        {
            OnDataSave?.Invoke();
        }

        private void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
            listView.Reset();
            allElementsToggle.SetIsOnWithoutNotify(selectedJumpers.Count > 0);
        }
        
        private static Action<Sprite, bool> SetJumperImage(Image img)
        {
            return (value, succeeded) =>
            {
                if (!succeeded)
                {
                    img.enabled = false;
                    return;
                }

                img.enabled = true;
                img.sprite = value;
            };
        }


        private void BindListViewItem(int index, CalendarEditorJumpersListItem item)
        {
            var competitor = jumpers[index];
            item.nameText.text = TvGraphicsUtils.JumperNameText(competitor);
            item.countryFlagText.text = competitor.countryCode;
            item.countryFlagImage.sprite = flagsData.GetFlag(competitor.countryCode);
            item.genderIconImage.sprite = iconsData.GetGenderIcon(competitor.gender);
            item.jumperImage.enabled = false;
            StartCoroutine(imageCache.GetSpriteAsync(competitor.imagePath, SetJumperImage(item.jumperImage)));
            item.toggleExtension.SetElementId(index);
            item.toggleExtension.Toggle.SetIsOnWithoutNotify(selectedJumpers.Contains(competitor));
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
                foreach (var jumper in jumpers)
                    selectedJumpers.Add(jumper);
            }
            else
            {
                selectedJumpers.Clear();
            }

            listView.RefreshShownValue();
        }
    }
}