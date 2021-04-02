using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.UI.ItemsSearch
{
    public abstract class ItemsSearchView<T, TSearchListItem, TListView> : MonoBehaviour
        where TSearchListItem : ItemsSearchListItem
        where TListView : ListView<T, TSearchListItem>
    {
        private bool _initialized;
        [SerializeField] protected TListView listView;
        [SerializeField] protected TMP_InputField inputField;
        [SerializeField] protected List<T> items;
        [SerializeField] protected T selectedItem;
        public event Action<T> OnValueChanged;

        public T SelectedItem
        {
            get => selectedItem;
            set => SetValue(value);
        }

        public void SetValue(T value, bool notify = true)
        {
            UpdateUI(value);
            selectedItem = value;
            if (!notify) return;
            OnValueChanged?.Invoke(value);
        }

        protected abstract void UpdateUI(T value);

        public void SetValueWithoutNotify(T value) => SetValue(value, false);

        protected void Start()
        {
            inputField.onValueChanged.AddListener(FilterItems);
            listView.SelectionType = SelectionType.None;
            listView.Initialize(BindListViewItem);
            FilterItems("");
            _initialized = true;
        }

        protected void OnEnable()
        {
            if (!_initialized) return;
            ResetSearchField();
        }

        protected void ResetSearchField()
        {
            inputField.SetTextWithoutNotify("");
            FilterItems("");
        }

        private void RefreshList()
        {
            listView.Items = items;
            listView.Refresh();
        }

        private void FilterItems(string data)
        {
            items = GetFilteredItems(data).ToList();
            RefreshList();
        }

        public void HandleValueChanged(int id)
        {
            SetValue(items[id]);
        }

        protected abstract void BindListViewItem(int index, TSearchListItem listItem);

        protected abstract IEnumerable<T> GetFilteredItems(string data);
    }
}