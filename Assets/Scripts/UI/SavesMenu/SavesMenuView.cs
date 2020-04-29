using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.SavesMenu
{
    public class SavesMenuView : MonoBehaviour, ISavesMenuView
    {
        private List<Calendar> calendars;
        [SerializeField] private CalendarsRuntime calendarsRuntime;

        [SerializeField] private SavesListView listView;
        private SavesMenuPresenter presenter;

        private List<GameSave> saves;
        [SerializeField] private SavesRuntime savesRuntime;
        private GameSave selectedSave;

        public event Action OnSelectionChanged;
        public event Action OnAdd;
        public event Action OnRemove;
        public event Action OnSubmit;

        public IEnumerable<GameSave> Saves
        {
            set
            {
                saves = value.ToList();
                listView.Items = saves;
                listView.SelectedIndex = Mathf.Clamp(listView.SelectedIndex, 0, saves.Count - 1);
                listView.Refresh();
            }
        }

        public IEnumerable<Calendar> Calendars
        {
            set
            {
                calendars = value.ToList();
                dropdown.ClearOptions();
                dropdown.AddOptions(value.Select(item => item.name).ToList());
            }
        }

        public GameSave SelectedSave
        {
            get => listView.SelectedIndex < 0 ? null : saves[listView.SelectedIndex];
            set => SelectSave(value);
        }

        public Calendar SelectedCalendar => calendars[dropdown.value];

        public string CurrentSaveName
        {
            set => nameText.text = value;
        }

        public string CurrentCalendarName
        {
            set => calendarText.text = value;
        }

        public string NewSaveName => input.text;

        public void ShowPopUp()
        {
            popUpRoot.SetActive(true);
            promptObj.SetActive(false);
            input.text = "";
        }

        public void HidePopUp()
        {
            popUpRoot.SetActive(false);
        }

        public void ShowPrompt()
        {
            promptObj.SetActive(true);
        }

        public bool SaveInfoEnabled
        {
            set => saveInfoObj.SetActive(value);
        }

        private void Start()
        {
            ListViewSetup();
            RegisterCallbacks();
            presenter = new SavesMenuPresenter(this, savesRuntime, calendarsRuntime);
        }

        private void ListViewSetup()
        {
            listView.OnSelectionChanged += x => { OnSelectionChanged?.Invoke(); };
            listView.SelectionType = SelectionType.Single;
            listView.Initialize(BindListViewItem);
        }

        private void RegisterCallbacks()
        {
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            submitButton.onClick.AddListener(() => OnSubmit?.Invoke());
            cancelButton.onClick.AddListener(HidePopUp);
        }

        private void BindListViewItem(int index, SavesListItem item)
        {
            item.valueText.text = saves[index].name;
        }

        private void SelectSave(GameSave save)
        {
            var index = saves.IndexOf(save);
            index = Mathf.Clamp(index, 0, saves.Count - 1);
            listView.SelectedIndex = index;
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        #region SaveInfoUI

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

        #endregion
    }
}