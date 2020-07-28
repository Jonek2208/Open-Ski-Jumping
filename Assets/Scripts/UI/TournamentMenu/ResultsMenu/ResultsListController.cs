using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.TVGraphics.SideResults;
using OpenSkiJumping.UI.ListView;
using UnityEngine;

namespace OpenSkiJumping.UI.TournamentMenu.ResultsMenu
{
    [Serializable]
    public class ResultsListItem
    {
        public string countryCode;
        public string name;
        public int rank;
        public decimal value;
    }

    public interface IResultsListController
    {
        IEnumerable<ResultsListItem> Results { set; }
    }

    public class ResultsListController : MonoBehaviour, IResultsListController
    {
        [SerializeField] private FlagsData flagsData;
        [SerializeField] private ResultsListView listView;
        private List<ResultsListItem> results;


        public IEnumerable<ResultsListItem> Results
        {
            set
            {
                results = value.ToList();
                listView.Items = results;
                listView.Refresh();
            }
        }

        private void Start()
        {
            listView.SelectionType = SelectionType.None;
            listView.Initialize(BindListViewItem);
        }

        private void BindListViewItem(int index, SideResultsListItem listItem)
        {
            var item = results[index];
            listItem.rankText.text = $"{item.rank}";
            listItem.nameText.text = item.name;
            listItem.countryFlagText.text = item.countryCode;
            listItem.countryFlagImage.sprite = flagsData.GetFlag(item.countryCode);
            listItem.resultText.text = $"{item.value.ToString("F1", CultureInfo.InvariantCulture)}";
        }
    }
}