using System;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.ScriptableObjects.Variables;
using OpenSkiJumping.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public interface IJumperEditorView : IViewBase
    {
        Competitor SelectedJumper { get; set; }

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

        void LoadImage(string path);
    }

    public class JumpersEditorPresenter 
    {
        private readonly IJumperEditorView _view;
        private readonly CompetitorsRuntime _jumpers;

        public JumpersEditorPresenter(IJumperEditorView view, CompetitorsRuntime jumpers)
        {
            _view = view;
            _jumpers = jumpers;

            InitEvents();
        }


        private void PresentJumperInfo()
        {
            var jumper = _view.SelectedJumper;
            if (jumper == null) return;
            _view.FirstName = jumper.firstName;
            _view.LastName = jumper.lastName;
            _view.CountryCode = jumper.countryCode;
            _view.Gender = (int) jumper.gender;
            _view.SuitTopFront = jumper.suitTopFrontColor;
            _view.SuitTopBack = jumper.suitTopBackColor;
            _view.SuitBottomFront = jumper.suitBottomFrontColor;
            _view.SuitBottomBack = jumper.suitBottomBackColor;
            _view.Helmet = jumper.helmetColor;
            _view.Skis = jumper.skisColor;
            _view.Skin = jumper.skinColor;
            _view.ImagePath = jumper.imagePath;
            _view.LoadImage(jumper.imagePath);
        }

        private void SaveJumperInfo()
        {
            var jumper = _view.SelectedJumper;
            if (jumper == null)
            {
                return;
            }

            jumper.firstName = _view.FirstName;
            jumper.lastName = _view.LastName;
            jumper.countryCode = _view.CountryCode;
            jumper.gender = (Gender) _view.Gender;
            jumper.suitTopFrontColor = _view.SuitTopFront;
            jumper.suitTopBackColor = _view.SuitTopBack;
            jumper.suitBottomFrontColor = _view.SuitBottomFront;
            jumper.suitBottomBackColor = _view.SuitBottomBack;
            jumper.helmetColor = _view.Helmet;
            jumper.skisColor = _view.Skis;
            jumper.skinColor = _view.Skin;
            jumper.imagePath = _view.ImagePath;
        }

        private void InitEvents()
        {
            _view.OnDataReload += PresentJumperInfo;
            _view.OnDataSave += SaveJumperInfo;
            _view.OnSelectionChanged += PresentJumperInfo;
            _view.OnCurrentJumperChanged += SaveJumperInfo;
        }
    }

    
    public class JumperEditorView : ViewBase, IJumperEditorView
    {
        private JumpersEditorPresenter _presenter;

        [SerializeField] private CompetitorsRuntime jumpersRuntime;
        [SerializeField] private FlagsData flagsData;
        [SerializeField] private IconsData iconsData;
        [SerializeField] private ImageCacher imageCache;
        [SerializeField] private JumperVariable currentJumper;

        protected override void Initialize()
        {
            RegisterCallbacks();
            _presenter = new JumpersEditorPresenter(this, jumpersRuntime);
        }

        public Competitor SelectedJumper
        {
            get => currentJumper.Value;
            set => currentJumper.Value = value;
        }

        public event Action OnSelectionChanged;
        public event Action OnCurrentJumperChanged;
        
        public void LoadImage(string path)
        {
            StartCoroutine(imageCache.GetSpriteAsync(path, SetJumperImage));
        }

        private void RegisterCallbacks()
        {
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
        }

        private void SetJumperImage(Sprite value, bool succeeded)
        {
            if (!succeeded)
            {
                image.enabled = false;
                return;
            }

            image.enabled = true;
            image.sprite = value;
        }

        #region JumperInfoUI

        [SerializeField] private GameObject jumperInfoObj;
        [SerializeField] private TMP_InputField firstNameInput;
        [SerializeField] private TMP_InputField lastNameInput;
        [SerializeField] private TMP_InputField countryCodeInput;
        [SerializeField] private SegmentedControl genderSelect;
        [SerializeField] private SimpleColorPicker helmetColorPicker;
        [SerializeField] private SimpleColorPicker suitTopFrontColorPicker;
        [SerializeField] private SimpleColorPicker suitTopBackColorPicker;
        [SerializeField] private SimpleColorPicker suitBottomFrontColorPicker;
        [SerializeField] private SimpleColorPicker suitBottomBackColorPicker;
        [SerializeField] private SimpleColorPicker skisColorPicker;
        [SerializeField] private SimpleColorPicker skinColorPicker;
        [SerializeField] private TMP_InputField imagePathInput;
        [SerializeField] private Image image;

        #endregion

        #region JumperInfoProps

        public string FirstName
        {
            get => firstNameInput.text;
            set => firstNameInput.SetTextWithoutNotify(value);
        }

        public string LastName
        {
            get => lastNameInput.text;
            set => lastNameInput.SetTextWithoutNotify(value);
        }

        public string CountryCode
        {
            get => countryCodeInput.text;
            set => countryCodeInput.SetTextWithoutNotify(value);
        }

        public int Gender
        {
            get => genderSelect.selectedSegmentIndex;
            set => genderSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public string SuitTopFront
        {
            get => suitTopFrontColorPicker.ToHex;
            set => suitTopFrontColorPicker.SetValueWithoutNotify(value);
        }

        public string SuitTopBack
        {
            get => suitTopBackColorPicker.ToHex;
            set => suitTopBackColorPicker.SetValueWithoutNotify(value);
        }

        public string SuitBottomFront
        {
            get => suitBottomFrontColorPicker.ToHex;
            set => suitBottomFrontColorPicker.SetValueWithoutNotify(value);
        }

        public string SuitBottomBack
        {
            get => suitBottomBackColorPicker.ToHex;
            set => suitBottomBackColorPicker.SetValueWithoutNotify(value);
        }

        public string Helmet
        {
            get => helmetColorPicker.ToHex;
            set => helmetColorPicker.SetValueWithoutNotify(value);
        }

        public string Skis
        {
            get => skisColorPicker.ToHex;
            set => skisColorPicker.SetValueWithoutNotify(value);
        }

        public string Skin
        {
            get => skinColorPicker.ToHex;
            set => skinColorPicker.SetValueWithoutNotify(value);
        }

        public string ImagePath
        {
            get => imagePathInput.text;
            set => imagePathInput.SetTextWithoutNotify(value);
        }

        #endregion
    }
}