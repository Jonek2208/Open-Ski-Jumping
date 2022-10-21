using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ScriptableObjects;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public interface IJumpersMenuViewNew
    {
        Competitor SelectedJumper { get; set; }
        IEnumerable<Competitor> Jumpers { set; }

        string FirstName { get; set; }
        string LastName { get; set; }
        string CountryCode { get; set; }
        int Gender { get; set; }
        string SuitTopFront { get; set; }
        string SuitTopBack { get; set; }
        string SuitBottomFront { get; set; }
        string SuitBottomBack { get; set; }
        string Helmet { get; set; }
        string Skis { get; set; }
        string Skin { get; set; }
        string ImagePath { get; set; }

        event Action OnSelectionChanged;
        event Action OnCurrentJumperChanged;
        event Action OnAdd;
        event Action OnRemove;

        bool JumperInfoEnabled { set; }
        void LoadImage(string path);
    }

    public class JumpersMenuPresenterNew
    {
        private readonly IJumpersMenuViewNew view;
        private readonly CompetitorsRuntime jumpers;
        private FlagsData flagsData;

        public JumpersMenuPresenterNew(IJumpersMenuViewNew view, CompetitorsRuntime jumpers, FlagsData flagsData)
        {
            this.view = view;
            this.jumpers = jumpers;
            this.flagsData = flagsData;

            InitEvents();
            SetInitValues();
        }

        private void CreateNewJumper()
        {
            var jumper = new Competitor();
            jumpers.Add(jumper);
            PresentList();
            view.SelectedJumper = jumper;
            PresentJumperInfo();
        }

        private void RemoveJumper()
        {
            Competitor jumper = view.SelectedJumper;
            if (jumper == null)
            {
                return;
            }

            bool val = jumpers.Remove(jumper);

            PresentList();
            view.SelectedJumper = null;
            PresentJumperInfo();
        }

        private void PresentList()
        {
            view.Jumpers = jumpers.GetData().OrderBy(item => item.countryCode);
        }

        private void PresentJumperInfo()
        {
            var jumper = view.SelectedJumper;
            if (jumper == null)
            {
                view.JumperInfoEnabled = false;
                return;
            }

            view.JumperInfoEnabled = true;

            view.FirstName = jumper.firstName;
            view.LastName = jumper.lastName;
            view.CountryCode = jumper.countryCode;
            view.Gender = (int) jumper.gender;
            view.SuitTopFront = jumper.suitTopFrontColor;
            view.SuitTopBack = jumper.suitTopBackColor;
            view.SuitBottomFront = jumper.suitBottomFrontColor;
            view.SuitBottomBack = jumper.suitBottomBackColor;
            view.Helmet = jumper.helmetColor;
            view.Skis = jumper.skisColor;
            view.Skin = jumper.skinColor;
            view.ImagePath = jumper.imagePath;
            view.LoadImage(jumper.imagePath);
        }

        private void SaveJumperInfo()
        {
            var jumper = view.SelectedJumper;
            if (jumper == null)
            {
                return;
            }

            jumper.firstName = view.FirstName;
            jumper.lastName = view.LastName;
            jumper.countryCode = view.CountryCode;
            jumper.gender = (Gender) view.Gender;
            jumper.suitTopFrontColor = view.SuitTopFront;
            jumper.suitTopBackColor = view.SuitTopBack;
            jumper.suitBottomFrontColor = view.SuitBottomFront;
            jumper.suitBottomBackColor = view.SuitBottomBack;
            jumper.helmetColor = view.Helmet;
            jumper.skisColor = view.Skis;
            jumper.skinColor = view.Skin;
            jumper.imagePath = view.ImagePath;

            view.LoadImage(jumper.imagePath);
            jumpers.Recalculate(jumper);
            PresentList();
            view.SelectedJumper = jumper;
        }

        private void InitEvents()
        {
            view.OnSelectionChanged += PresentJumperInfo;
            view.OnAdd += CreateNewJumper;
            view.OnRemove += RemoveJumper;
            view.OnCurrentJumperChanged += SaveJumperInfo;
        }

        private void SetInitValues()
        {
            PresentList();
            view.SelectedJumper = jumpers.GetData().OrderBy(item => item.countryCode).First();
            PresentJumperInfo();
        }
    }

    public class JumpersMenuViewNew : MonoBehaviour, IJumpersMenuViewNew
    {
        [SerializeField] private FlagsData flagsData;
        [SerializeField] private Sprite[] genderIcons;
        [SerializeField] private ImageCacher imageCache;
        private List<Competitor> _jumpers;

        [SerializeField] private UIDocument vTreeAsset;
        [SerializeField] private VisualTreeAsset jumperListItem;

        [SerializeField] private CompetitorsRuntime jumpersRuntime;

        private JumpersMenuPresenterNew _presenter;
        private VisualElement _rootElement;

        private UnityEngine.UIElements.ListView _listView;

        public Competitor SelectedJumper
        {
            get => _listView.selectedIndex < 0 ? null : _jumpers[_listView.selectedIndex];
            set => SelectJumper(value);
        }

        public event Action OnSelectionChanged;
        public event Action OnCurrentJumperChanged;
        public event Action OnAdd;
        public event Action OnRemove;

        public bool JumperInfoEnabled
        {
            set { }
        }

        public IEnumerable<Competitor> Jumpers
        {
            set
            {
                _jumpers = value.ToList();
                _listView.itemsSource = _jumpers;
                _listView.selectedIndex = Mathf.Clamp(_listView.selectedIndex, 0, _jumpers.Count - 1);
                _listView.Rebuild();
            }
        }

        public void LoadImage(string path)
        {
            StartCoroutine(imageCache.GetSpriteAsync(path, SetJumperImage));
        }

        private void Start()
        {
            _rootElement = vTreeAsset.rootVisualElement;
            ListViewSetup();
            RegisterCallbacks();
            _presenter = new JumpersMenuPresenterNew(this, jumpersRuntime, flagsData);
        }

        private void ListViewSetup()
        {
            _listView = _rootElement.Q<UnityEngine.UIElements.ListView>();
            _listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            _listView.onSelectionChange += _ => OnSelectionChanged?.Invoke();
            _listView.selectionType = SelectionType.None;
            _listView.makeItem = () => jumperListItem.CloneTree();
            _listView.bindItem = BindListViewItem;
            _listView.Rebuild();
        }

        private void RegisterCallbacks()
        {
            // addButton.onClick.AddListener(() => OnAdd?.Invoke());
            // removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            // firstNameInput.onEndEdit.AddListener(x => OnValueChanged());
            // lastNameInput.onEndEdit.AddListener(x => OnValueChanged());
            // countryCodeInput.onEndEdit.AddListener(x => OnValueChanged());
            // imagePathInput.onEndEdit.AddListener(x => OnValueChanged());
            // genderSelect.onValueChanged.AddListener(x => OnValueChanged());
            // helmetColorPicker.OnColorChange += OnValueChanged;
            // suitTopFrontColorPicker.OnColorChange += OnValueChanged;
            // suitTopBackColorPicker.OnColorChange += OnValueChanged;
            // suitBottomFrontColorPicker.OnColorChange += OnValueChanged;
            // suitBottomBackColorPicker.OnColorChange += OnValueChanged;
            // skisColorPicker.OnColorChange += OnValueChanged;
        }

        private void OnValueChanged()
        {
            OnCurrentJumperChanged?.Invoke();
        }

        private void SelectJumper(Competitor jumper)
        {
            var index = jumper == null ? _listView.selectedIndex : _jumpers.IndexOf(jumper);
            index = Mathf.Clamp(index, 0, _jumpers.Count - 1);
            _listView.selectedIndex = index;
            _listView.ScrollToItem(index);
        }

        private void SetJumperImage(Sprite value, bool succeeded)
        {
            // if (!succeeded)
            // {
            //     image.enabled = false;
            //     return;
            // }
            //
            // image.enabled = true;
            // image.sprite = value;
        }


        private void BindListViewItem(VisualElement item, int index)
        {
            var jumper = _jumpers[index];
            item.Q<UnityEngine.UIElements.Label>("name").text =
                $"{jumper.firstName.ToUpper()} <b>{jumper.lastName.ToUpper()}</b>";
            item.Q<UnityEngine.UIElements.Label>("country-code").text = jumper.countryCode;
            item.Q("flag").style.backgroundImage = Background.FromSprite(flagsData.GetFlag(jumper.countryCode));
            StartCoroutine(imageCache.GetSpriteAsync(jumper.imagePath,
                (sprite, b) =>
                {
                    item.Q("image").style.backgroundImage = b ? Background.FromSprite(sprite) : new Background();
                }));
            // item.countryFlagImage.sprite = flagsData.GetFlag(jumper.countryCode);
            // item.genderIconImage.sprite = genderIcons[(int) jumper.gender];
        }

        #region JumperInfoProps

        public string FirstName
        {
            get => default;
            set { }
        }

        public string LastName
        {
            get => default;
            set { }
        }

        public string CountryCode
        {
            get => default;
            set { }
        }

        public int Gender
        {
            get => default;
            set { }
        }

        public string SuitTopFront
        {
            get => default;
            set { }
        }

        public string SuitTopBack
        {
            get => default;
            set { }
        }

        public string SuitBottomFront
        {
            get => default;
            set { }
        }

        public string SuitBottomBack
        {
            get => default;
            set { }
        }

        public string Helmet
        {
            get => default;
            set { }
        }

        public string Skis
        {
            get => default;
            set { }
        }

        public string Skin
        {
            get => default;
            set { }
        }

        public string ImagePath
        {
            get => default;
            set { }
        }

        #endregion
    }
}