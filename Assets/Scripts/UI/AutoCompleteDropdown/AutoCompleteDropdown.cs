using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.UI.AutoCompleteDropdown
{
    public class AutoCompleteDropdown<T, TItem, TListView> : MonoBehaviour where TItem : ListItemBehaviour
        where TListView : ListView<T, TItem>
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TListView listView;
        [SerializeField] private ListView<T, TItem> type;
        [SerializeField] private T selectedItem;
        [field: SerializeField] public List<T> Items { get; set; }
        private List<T> currentItems;
        private GameObject listViewGO;

        public event Action<T> OnValueChanged;

        private void Awake()
        {
            listViewGO = listView.gameObject;
        }

        public T SelectedItem
        {
            get => selectedItem;
            set => SelectItem(value);
        }

        public void SelectItemWithoutNotify(T item) => SelectItem(item, false);
        public Action<int, TItem> BindItem { get; set; }
        public Func<T, string> GetName { get; set; }

        private void SelectItem(T item, bool notify = true)
        {
            selectedItem = item;
            inputField.SetTextWithoutNotify(GetName(item));

            if (notify) OnValueChanged?.Invoke(selectedItem);
        }

        private void UpdateList(string value)
        {
            currentItems = Items.FindAll(item => GetName(item).Contains(value)).ToList();
            listView.Items = currentItems;
            listView.SelectedIndex = 0;
            listView.Refresh();
        }

        private void ShowList()
        {
            listViewGO.SetActive(true);
        }

        private void HideList()
        {
            listViewGO.SetActive(false);
        }

        public void Initialize(Func<T, string> getName, Action<int, TItem> bindItem)
        {
            GetName = getName;
            BindItem = bindItem;
            listView.Initialize(BindItem);
            listView.SelectionType = SelectionType.Single;
            listView.Items = currentItems;
            listView.OnSelectionChanged += index => SelectItem(currentItems[index]);

            inputField.onValueChanged.AddListener(UpdateList);
            inputField.onSelect.AddListener(_ => ShowList());
            inputField.onDeselect.AddListener(_ => HideList());
        }
    }
}