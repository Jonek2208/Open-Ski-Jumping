using System;
using System.Collections.Generic;
using System.Linq;
using Competition.Persistent;
using Data;
using JetBrains.Annotations;
using ListView;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace UI.JumpersMenu
{
    public class JumpersMenuView : MonoBehaviour, IJumpersMenuView
    {
        private JumpersMenuPresenter presenter;

        [SerializeField] private CompetitorsRuntime jumpersRuntime;
        [SerializeField] private FlagsData flagsData;
        [SerializeField] private ImageCacher imageCacher;
        [SerializeField] private Sprite[] genderIcons;

        [SerializeField] private JumpersListView listView;
        [SerializeField] private ToggleGroupExtension toggleGroup;

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
        [SerializeField] private TMP_InputField imagePathInput;
        [SerializeField] private Image image;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

        #endregion

        private Competitor selectedJumper;
        public Competitor SelectedJumper => jumpers[toggleGroup.CurrentValue];

        private List<Competitor> jumpers;

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
            set => suitTopFrontColorPicker.Set(value);
        }

        public string SuitTopBack
        {
            get => suitTopBackColorPicker.ToHex;
            set => suitTopBackColorPicker.Set(value);
        }

        public string SuitBottomFront
        {
            get => suitBottomFrontColorPicker.ToHex;
            set => suitBottomFrontColorPicker.Set(value);
        }

        public string SuitBottomBack
        {
            get => suitBottomBackColorPicker.ToHex;
            set => suitBottomBackColorPicker.Set(value);
        }

        public string Helmet
        {
            get => helmetColorPicker.ToHex;
            set => helmetColorPicker.Set(value);
        }

        public string Skis
        {
            get => skisColorPicker.ToHex;
            set => skisColorPicker.Set(value);
        }

        public string ImagePath
        {
            get => imagePathInput.text;
            set => imagePathInput.SetTextWithoutNotify(value);
        }

        #endregion

        public bool BlockSelectionCallbacks { get; set; }
        public event Action OnSelectionChanged;
        public event Action OnCurrentJumperChanged;
        public event Action OnAdd;
        public event Action OnRemove;

        public IEnumerable<Competitor> Jumpers
        {
            set
            {
                jumpers = value.ToList();
                listView.Items = jumpers;
                FixCurrentSelection();
                listView.Refresh();
            }
        }

        public void SetJumpersWithSelection(IEnumerable<Competitor> value, Competitor jumper)
        {
            jumpers = value.ToList();
            listView.Items = jumpers;
            SelectJumper(jumper);
            listView.Refresh();
            listView.ScrollToIndex(toggleGroup.CurrentValue);
        }

        private void Start()
        {
            toggleGroup.OnValueChanged += x => { HandleSelectionChanged(); };
            listView.Initialize(BindListViewItem);
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
            RegisterCallbacks();
            presenter = new JumpersMenuPresenter(this, jumpersRuntime, flagsData);
        }

        private void RegisterCallbacks()
        {
            firstNameInput.onEndEdit.AddListener(x => OnValueChanged());
            lastNameInput.onEndEdit.AddListener(x => OnValueChanged());
            countryCodeInput.onEndEdit.AddListener(x => OnValueChanged());
            imagePathInput.onEndEdit.AddListener(x => OnValueChanged());
            genderSelect.onValueChanged.AddListener(x => OnValueChanged());
            helmetColorPicker.OnColorChange += OnValueChanged;
            suitTopFrontColorPicker.OnColorChange += OnValueChanged;
            suitTopBackColorPicker.OnColorChange += OnValueChanged;
            suitBottomFrontColorPicker.OnColorChange += OnValueChanged;
            suitBottomBackColorPicker.OnColorChange += OnValueChanged;
            skisColorPicker.OnColorChange += OnValueChanged;
        }

        private void OnValueChanged()
        {
            OnCurrentJumperChanged?.Invoke();
        }

        private void HandleSelectionChanged()
        {
            if (!BlockSelectionCallbacks) OnSelectionChanged?.Invoke();
        }

        public void SelectJumper(Competitor jumper)
        {
            int index = (jumper == null) ? toggleGroup.CurrentValue : jumpers.IndexOf(jumper);
            index = Mathf.Clamp(index, 0, jumpers.Count - 1);
            toggleGroup.HandleSelectionChanged(index, true, false);

            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
        }

        public void HideJumperInfo() => jumperInfoObj.SetActive(false);
        public void ShowJumperInfo() => jumperInfoObj.SetActive(true);

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

        public void LoadImage(string path)
        {
            Debug.Log($"LoadingImage {path}");
            StartCoroutine(imageCacher.GetSpriteAsync(path, SetJumperImage));
        }

        private void FixCurrentSelection()
        {
            toggleGroup.HandleSelectionChanged(Mathf.Clamp(toggleGroup.CurrentValue, 0, jumpers.Count - 1), true,
                false);
        }

        private void BindListViewItem(int index, JumpersListItem item)
        {
            var jumper = jumpers[index];
            item.nameText.text = $"{jumper.firstName} {jumper.lastName.ToUpper()}";
            item.countryFlagText.text = jumper.countryCode;
            item.countryFlagImage.sprite = flagsData.GetFlag(jumper.countryCode);
            if ((int) jumper.gender < 0 || (int) jumper.gender >= genderIcons.Length)
                Debug.LogError($"Index out of range {(int) jumper.gender}");
            item.genderIconImage.sprite = genderIcons[(int) jumper.gender];
            item.toggleExtension.SetElementId(index);
        }
    }
}