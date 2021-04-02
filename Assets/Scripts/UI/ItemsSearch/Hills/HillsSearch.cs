using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.ItemsSearch.Hills
{
    public class HillsSearch : ItemsSearchView<ProfileData, ItemsSearchListItem, HillsSearchListView>
    {
        [SerializeField] private HillsRuntime hillsRuntime;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject contentGO;


        protected new void Start()
        {
            base.Start();
            OnValueChanged += HandleItemSelected;
            openButton.onClick.AddListener(ShowHillsSearch);
            closeButton.onClick.AddListener(HideHillsSearch);
        }

        protected override void BindListViewItem(int index, ItemsSearchListItem listItem)
        {
            listItem.valueText.text = items[index].name;
        }

        protected override IEnumerable<ProfileData> GetFilteredItems(string data)
        {
            return hillsRuntime.Data.Where(
                it => it.name.IndexOf(data, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        protected override void UpdateUI(ProfileData value)
        {
            buttonText.text = value.name;
        }

        public void HandleItemSelected(ProfileData data)
        {
            UpdateUI(data);
            HideHillsSearch();
        }


        public void ShowHillsSearch()
        {
            ResetSearchField();
            contentGO.gameObject.SetActive(true);
            inputField.Select();
        }

        private void HideHillsSearch()
        {
            contentGO.gameObject.SetActive(false);
        }
    }
}