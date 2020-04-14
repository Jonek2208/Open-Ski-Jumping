using System;
using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;
using Data;
using ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SavesMenu
{
    public class SavesMenuView : MonoBehaviour, ISavesMenuView
    {
        private SavesMenuPresenter presenter;
        [SerializeField] private SavesRuntime savesRuntime;
        [SerializeField] private CalendarsRuntime calendarsRuntime;

        [SerializeField] private SavesListView listView;
        [SerializeField] private ToggleGroupExtension toggleGroup;

        [SerializeField] private GameObject saveInfoObj;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text calendarText;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

        [SerializeField] private GameObject popUpRoot;
        [SerializeField] private GameObject promptObj;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;

        public event Action OnSelectionChanged;
        public event Action OnAdd;
        public event Action OnRemove;
        public event Action OnSubmit;

        private List<GameSave> saves;
        public IEnumerable<GameSave> Saves
        {
            set
            {
                saves = value.ToList();
                listView.Items = saves;
                FixCurrentSelection();
                listView.Refresh();
                OnSelectionChanged?.Invoke();
            }
        }
        private List<Calendar> calendars;
        private GameSave selectedSave;

        public IEnumerable<Calendar> Calendars
        {
            set
            {
                calendars = value.ToList();
                dropdown.ClearOptions();
                dropdown.AddOptions(value.Select(item => item.name).ToList());
            }
        }

        public GameSave SelectedSave => saves[toggleGroup.CurrentValue];
        public Calendar         SelectedCalendar => calendars[dropdown.value];


        private void FixCurrentSelection()
        {
            toggleGroup.HandleSelectionChanged(Mathf.Clamp(toggleGroup.CurrentValue, 0, saves.Count - 1));
        }

        public string CurrentSaveName { set => nameText.text = value; }
        public string CurrentCalendarName { set => calendarText.text = value; }
        public string NewSaveName => input.text;

        private void Start()
        {
            toggleGroup.OnValueChanged += val => { OnSelectionChanged?.Invoke(); };
            listView.Initialize(BindListViewItem);
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            submitButton.onClick.AddListener(() => OnSubmit?.Invoke());
            cancelButton.onClick.AddListener(HidePopUp);
            presenter = new SavesMenuPresenter(this, savesRuntime, calendarsRuntime);
        }

        public void ShowSaveInfo() => saveInfoObj.SetActive(true);
        public void HideSaveInfo() => saveInfoObj.SetActive(false);
        public void ShowPopUp()
        {
            popUpRoot.SetActive(true);
            promptObj.SetActive(false);
            input.text = "";
        }
        public void ShowPrompt() => promptObj.SetActive(true);
        public void HidePopUp() => popUpRoot.SetActive(false);

        private void BindListViewItem(int index, SavesListItem item)
        {
            item.valueText.text = saves[index].name;
            item.toggleExtension.SetElementId(index);
        }

        public void SelectSave(GameSave save)
        {
            int index = saves.IndexOf(save);
            index = Mathf.Clamp(index, 0, saves.Count - 1);
            toggleGroup.HandleSelectionChanged(index);
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }
    }
}