using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.ItemsSearch.Jumpers
{
    public class JumpersSearch : ItemsSearchView<Competitor, ItemsSearchListItem, JumpersSearchListView>
    {
        [SerializeField] private CompetitorsRuntime competitorsRuntime;
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
            listItem.valueText.text = $"{items[index].firstName} {items[index].lastName} {items[index].countryCode}";
        }

        protected override IEnumerable<Competitor> GetFilteredItems(string data)
        {
            return competitorsRuntime.Data.Where(
                it => $"{it.firstName} {it.lastName}".IndexOf(data, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        protected override void UpdateUI(Competitor value)
        {
            buttonText.text = $"{value.firstName} {value.lastName} {value.countryCode}";
        }

        public void HandleItemSelected(Competitor data)
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