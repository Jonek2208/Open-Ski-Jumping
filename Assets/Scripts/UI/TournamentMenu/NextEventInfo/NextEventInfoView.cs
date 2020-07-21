using System;
using OpenSkiJumping.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = OpenSkiJumping.Competition.EventType;

namespace OpenSkiJumping.UI.TournamentMenu.NextEventInfo
{
    public class NextEventInfoView : MonoBehaviour, INextEventInfoView
    {
        private bool initialized;
        private NextEventInfoPresenter presenter;
        [SerializeField] private IconsData iconsData;
        [SerializeField] private TournamentMenuData tournamentMenuData;

        [SerializeField] private TMP_Text hillText;
        [SerializeField] private Image eventTypeImage;

        private void Start()
        {
            presenter = new NextEventInfoPresenter(this, tournamentMenuData);
            initialized = true;
        }

        private void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
        }

        public string Hill
        {
            set => hillText.text = value;
        }

        public EventType EventType
        {
            set => eventTypeImage.sprite = iconsData.GetEventTypeIcon(value);
        }

        public event Action OnDataReload;
    }
}