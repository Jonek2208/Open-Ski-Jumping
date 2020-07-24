using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using OpenSkiJumping.UI.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.HillsMenu
{
    public class HillsMenuView : MonoBehaviour, IHillsMenuView
    {
        private bool initialized;
        private HillsMenuPresenter presenter;

        [SerializeField] private HillsRuntime hillsRuntime;

        [Header("UI Fields")] 
        [SerializeField] private HillInfoView hillInfoView;
        [SerializeField] private HillsListView listView;
        [SerializeField] private Button duplicateButton;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;
        
        private List<ProfileData> hills;
        
        public ProfileData SelectedHill
        {
            get => listView.SelectedIndex < 0 ? null : hills[listView.SelectedIndex];
            set => SelectHill(value);
        }

        public event Action OnSelectionChanged;
        public event Action OnDataReload;
        public event Action OnAdd;
        public event Action OnRemove;
        public event Action OnDuplicate;

        private void Start()
        {
            ListViewSetup();
            RegisterCallbacks();
            hillInfoView.Initialize();
            presenter = new HillsMenuPresenter(this, hillsRuntime);
            initialized = true;
        }

        private void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
        }

        private void ListViewSetup()
        {
            listView.OnSelectionChanged += x => OnSelectionChanged?.Invoke();
            listView.SelectionType = SelectionType.Single;
            listView.Initialize(BindListViewItem);
        }

        private void RegisterCallbacks()
        {
            duplicateButton.onClick.AddListener(() => OnDuplicate?.Invoke());
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
        }

        private void SelectHill(ProfileData item)
        {
            var index = item == null ? listView.SelectedIndex : hills.IndexOf(item);
            index = Mathf.Clamp(index, 0, hills.Count - 1);
            listView.SelectedIndex = index;
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        public IEnumerable<ProfileData> Hills
        {
            set
            {
                hills = value.ToList();
                listView.Items = hills;
                listView.SelectedIndex = Mathf.Clamp(listView.SelectedIndex, 0, hills.Count - 1);
                listView.Refresh();
            }
        }

        private void BindListViewItem(int index, HillsListItem listItem)
        {
            var item = hills[index];
            listItem.nameText.text = item.name;
        }


        public IHillInfoView HillInfoView => hillInfoView;
    }
}