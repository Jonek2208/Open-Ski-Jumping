using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.UI.ListView;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.TournamentMenu.ClassificationsHierarchy
{
    public class ClassificationsHierarchyView : MonoBehaviour, IClassificationsHierarchyView
    {
        private ClassificationsHierarchyPresenter presenter;
        private bool initialized;

        [SerializeField] private TournamentMenuData tournamentMenuData;
        
        [SerializeField] private Sprite[] classificationTypeIcons;
        [SerializeField] private Sprite[] eventTypeIcons;

        [SerializeField] private ClassificationsHierarchyListView listView;
        [SerializeField] private ToggleGroupExtension toggleGroupExtension;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;


        private List<ClassificationData> classifications;

        public ClassificationData SelectedClassification
        {
            get => listView.SelectedIndex < 0 ? null : classifications[listView.SelectedIndex];
            set => listView.SelectItem(value);
        }

        public IEnumerable<ClassificationData> Classifications
        {
            set
            {
                classifications = value.ToList();
                listView.Items = classifications;
                listView.ClampSelectedIndex();
                listView.Refresh();
            }
        }

        public event Action OnSelectionChanged;
        public event Action<ClassificationData, bool> OnChangeBibState;
        public event Action OnMoveUp;
        public event Action OnMoveDown;
        public event Action OnDataReload;

        private void Start()
        {
            ListViewSetup();
            RegisterCallbacks();
            presenter = new ClassificationsHierarchyPresenter(this, tournamentMenuData);
            initialized = true;
        }
        
        private void OnEnable()
        {
            if (!initialized) return;
            OnDataReload?.Invoke();
            listView.Reset();
        }

        private void RegisterCallbacks()
        {
            moveUpButton.onClick.AddListener(() => OnMoveUp?.Invoke());
            moveDownButton.onClick.AddListener(() => OnMoveDown?.Invoke());
        }

        private void ListViewSetup()
        {
            toggleGroupExtension.OnSelectionChanged += HandleSelectionChanged;
            
            listView.OnSelectionChanged += x => OnSelectionChanged?.Invoke();
            listView.SelectionType = SelectionType.Single;
            listView.Initialize(BindListViewItem);
        }

        private void BindListViewItem(int index, ClassificationsHierarchyItem listItem)
        {
            var item = classifications[index];
            listItem.nameText.text = item.classification.name;
            listItem.bibImage.color = SimpleColorPicker.Hex2Color(item.classification.leaderBibColor);
            listItem.eventTypeImage.sprite = eventTypeIcons[(int) item.classification.eventType];
            listItem.classificationTypeImage.sprite =
                classificationTypeIcons[(int) item.classification.classificationType];

            listItem.toggleExtension.SetElementId(index);
            listItem.toggleExtension.Toggle.SetIsOnWithoutNotify(item.useBib);
        }

        private void HandleSelectionChanged(int index, bool value)
        {
            var item = classifications[index];
            OnChangeBibState?.Invoke(item, value);
        }
    }
}