using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.CalendarEditor.Classifications;
using OpenSkiJumping.UI.CalendarEditor.Events;
using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using EventType = OpenSkiJumping.Competition.EventType;

namespace OpenSkiJumping.UI.TournamentMenu.CalendarEventInfo
{
    public class CalendarEventInfoView : MonoBehaviour, ICalendarEventInfoView
    {
        private CalendarEventInfoPresenter presenter;

        [SerializeField] private TournamentMenuData tournamentMenuData;

        [SerializeField] private IconsData iconsData;

        #region UIFields

        [Header("UI Fields")] [SerializeField] private Image eventTypeImage;
        [SerializeField] private TMP_Text presetName;
        [SerializeField] private TMP_Text hillName;
        [SerializeField] private TMP_Text ordRankText;
        [SerializeField] private TMP_Text qualRankText;
        [SerializeField] private TMP_Text qualLimitText;
        [SerializeField] private TMP_Text preQualRankText;
        [SerializeField] private TMP_Text preQualLimitText;
        [SerializeField] private TMP_Text hillSurfaceText;

        [SerializeField] private GameObject eventInfoObj;

        [SerializeField] private ClassificationsListView classificationsListView;

        #endregion


        public void DataReload()
        {
            OnDataReload?.Invoke();
        }

        public event Action OnDataReload;

        private List<ClassificationInfo> classifications;

        public EventRoundsInfo RoundsInfo
        {
            set => presetName.text = value.name;
        }

        public IEnumerable<ClassificationInfo> Classifications
        {
            set
            {
                classifications = value.ToList();
                classificationsListView.Items = classifications;
                classificationsListView.Refresh();
            }
        }

        public string Hill
        {
            set => hillName.text = value;
        }

        public EventType EventType
        {
            set => eventTypeImage.sprite = iconsData.GetEventTypeIcon(value);
        }

        public string OrdRank
        {
            set => ordRankText.text = value;
        }

        public string QualRank
        {
            set => qualRankText.text = value;
        }

        public string QualLimit
        {
            set => qualLimitText.text = value;
        }

        public string PreQualRank
        {
            set => preQualRankText.text = value;
        }

        public string PreQualLimit
        {
            set => preQualLimitText.text = value;
        }

        public string HillSurface
        {
            set => hillSurfaceText.text = value;
        }


        public void Initialize()
        {
            ListViewSetup();
            presenter = new CalendarEventInfoPresenter(this, tournamentMenuData);
        }

        private void ListViewSetup()
        {
            classificationsListView.SelectionType = SelectionType.None;
            classificationsListView.Initialize(BindClassificationsListViewItem);
        }

        private void BindClassificationsListViewItem(int index, ClassificationsListItem listItem)
        {
            var item = classifications[index];
            
            listItem.nameText.text = item.name;
            listItem.classificationTypeImage.sprite = iconsData.GetClassificationTypeIcon(item.classificationType);
            listItem.eventTypeImage.sprite = iconsData.GetEventTypeIcon(item.eventType);
            listItem.bibImage.color = SimpleColorPicker.Hex2Color(item.leaderBibColor);
        }
    }
}