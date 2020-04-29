using System;
using System.Collections.Generic;
using System.Linq;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Data;
using OpenSkiJumping.ListView;
using OpenSkiJumping.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public class JumpersMenuView : MonoBehaviour, IJumpersMenuView
    {
        [SerializeField] private FlagsData flagsData;
        [SerializeField] private Sprite[] genderIcons;
        [SerializeField] private ImageCacher imageCacher;
        private List<Competitor> jumpers;

        [SerializeField] private CompetitorsRuntime jumpersRuntime;

        [SerializeField] private JumpersListView listView;
        private JumpersMenuPresenter presenter;

        public UnityEngine.UIElements.ListView xd;

        public Competitor SelectedJumper
        {
            get => listView.SelectedIndex < 0 ? null : jumpers[listView.SelectedIndex];
            set => SelectJumper(value);
        }

        public event Action OnSelectionChanged;
        public event Action OnCurrentJumperChanged;
        public event Action OnAdd;
        public event Action OnRemove;

        public bool JumperInfoEnabled
        {
            set => jumperInfoObj.SetActive(value);
        }

        public IEnumerable<Competitor> Jumpers
        {
            set
            {
                jumpers = value.ToList();
                listView.Items = jumpers;
                listView.SelectedIndex = Mathf.Clamp(listView.SelectedIndex, 0, jumpers.Count - 1);
                listView.Refresh();
            }
        }

        public void LoadImage(string path)
        {
            StartCoroutine(imageCacher.GetSpriteAsync(path, SetJumperImage));
        }

        private void Start()
        {
            ListViewSetup();
            RegisterCallbacks();
            presenter = new JumpersMenuPresenter(this, jumpersRuntime, flagsData);
        }

        private void ListViewSetup()
        {
            listView.OnSelectionChanged += x => OnSelectionChanged?.Invoke();
            listView.SelectionType = SelectionType.Single;
            listView.Initialize(BindListViewItem);
        }

        private void RegisterCallbacks()
        {
            addButton.onClick.AddListener(() => OnAdd?.Invoke());
            removeButton.onClick.AddListener(() => OnRemove?.Invoke());
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

        private void SelectJumper(Competitor jumper)
        {
            var index = jumper == null ? listView.SelectedIndex : jumpers.IndexOf(jumper);
            index = Mathf.Clamp(index, 0, jumpers.Count - 1);
            listView.SelectedIndex = index;
            listView.ScrollToIndex(index);
            listView.RefreshShownValue();
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


        private void BindListViewItem(int index, JumpersListItem item)
        {
            var jumper = jumpers[index];
            item.nameText.text = $"{jumper.firstName} {jumper.lastName.ToUpper()}";
            item.countryFlagText.text = jumper.countryCode;
            item.countryFlagImage.sprite = flagsData.GetFlag(jumper.countryCode);
            item.genderIconImage.sprite = genderIcons[(int) jumper.gender];
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
        [SerializeField] private TMP_InputField imagePathInput;
        [SerializeField] private Image image;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;

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

        public string ImagePath
        {
            get => imagePathInput.text;
            set => imagePathInput.SetTextWithoutNotify(value);
        }

        #endregion
    }
}