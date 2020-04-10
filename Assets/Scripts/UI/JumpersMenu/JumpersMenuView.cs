using System;
using System.Collections.Generic;
using System.Linq;
using Competition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IJumpersMenuView
{
    Competitor SelectedJumper { get; }
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
    string ImagePath { get; set; }

    bool BlockJumperInfoCallbacks { get; set; }
    bool BlockSelectionCallbacks { get; set; }

    event Action OnSelectionChanged;
    event Action OnCurrentJumperChanged;
    event Action OnAdd;
    event Action OnRemove;

    void SelectJumper(Competitor jumper);
    void HideJumperInfo();
    void ShowJumperInfo();
    void LoadImage(string path);
}
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
    [SerializeField] private TMP_Dropdown genderDropdown;
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
    public Competitor SelectedJumper { get => jumpers[toggleGroup.CurrentValue]; }

    private List<Competitor> jumpers;

    #region JumperInfoProps
    public string FirstName { get => firstNameInput.text; set => firstNameInput.text = value; }
    public string LastName { get => lastNameInput.text; set => lastNameInput.text = value; }
    public string CountryCode { get => countryCodeInput.text; set => countryCodeInput.text = value; }
    public int Gender { get => genderDropdown.value; set => genderDropdown.value = value; }
    public string SuitTopFront { get => suitTopFrontColorPicker.ToHex; set => suitTopFrontColorPicker.Set(value); }
    public string SuitTopBack { get => suitTopBackColorPicker.ToHex; set => suitTopBackColorPicker.Set(value); }
    public string SuitBottomFront { get => suitBottomFrontColorPicker.ToHex; set => suitBottomFrontColorPicker.Set(value); }
    public string SuitBottomBack { get => suitBottomBackColorPicker.ToHex; set => suitBottomBackColorPicker.Set(value); }
    public string Helmet { get => helmetColorPicker.ToHex; set => helmetColorPicker.Set(value); }
    public string Skis { get => skisColorPicker.ToHex; set => skisColorPicker.Set(value); }
    public string ImagePath { get => imagePathInput.text; set => imagePathInput.text = value; }

    #endregion
    public bool BlockJumperInfoCallbacks { get; set; }
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
            listView.ScrollToIndex(toggleGroup.CurrentValue);
            HandleSelectionChanged();
        }
    }

    private void Start()
    {
        toggleGroup.onValueChanged += (x) => { HandleSelectionChanged(); };
        listView.Initialize(BindListViewItem);
        addButton.onClick.AddListener(() => OnAdd?.Invoke());
        removeButton.onClick.AddListener(() => OnRemove?.Invoke());
        RegisterCallbacks();
        presenter = new JumpersMenuPresenter(this, jumpersRuntime, flagsData);
    }

    private void RegisterCallbacks()
    {
        firstNameInput.onEndEdit.AddListener((x) => OnValueChanged());
        lastNameInput.onEndEdit.AddListener((x) => OnValueChanged());
        countryCodeInput.onEndEdit.AddListener((x) => OnValueChanged());
        imagePathInput.onEndEdit.AddListener((x) => OnValueChanged());
        genderDropdown.onValueChanged.AddListener((x) => OnValueChanged());
        helmetColorPicker.OnColorChange += OnValueChanged;
        suitTopFrontColorPicker.OnColorChange += OnValueChanged;
        suitTopBackColorPicker.OnColorChange += OnValueChanged;
        suitBottomFrontColorPicker.OnColorChange += OnValueChanged;
        suitBottomBackColorPicker.OnColorChange += OnValueChanged;
        skisColorPicker.OnColorChange += OnValueChanged;
    }
    private void OnValueChanged()
    {
        if (!BlockJumperInfoCallbacks) { OnCurrentJumperChanged?.Invoke(); }
    }

    private void HandleSelectionChanged()
    {
        if (!BlockSelectionCallbacks) { OnSelectionChanged?.Invoke(); }
    }
    public void SelectJumper(Competitor jumper)
    {
        int index = jumpers.IndexOf(jumper);
        index = Mathf.Clamp(index, 0, jumpers.Count - 1);
        toggleGroup.SetCurrentId(index);
        listView.ScrollToIndex(index);
        listView.RefreshShownValue();
    }
    public void HideJumperInfo() => jumperInfoObj.SetActive(false);
    public void ShowJumperInfo() => jumperInfoObj.SetActive(true);

    private void SetJumperImage(Sprite value)
    {
        if (value == null) { image.enabled = false; }
        image.enabled = true;
        image.sprite = value;
    }

    public void LoadImage(string path) { Debug.Log($"LoadingImage {path}"); StartCoroutine(imageCacher.GetSpriteAsync(path, SetJumperImage)); }
    private void FixCurrentSelection() => toggleGroup.SetCurrentId(Mathf.Clamp(toggleGroup.CurrentValue, 0, jumpers.Count - 1));

    private void BindListViewItem(int index, JumpersListItem item)
    {
        item.nameText.text = $"{jumpers[index].firstName} {jumpers[index].lastName.ToUpper()}";
        item.countryFlagText.text = jumpers[index].countryCode;
        item.countryFlagImage.sprite = flagsData.GetFlag(jumpers[index].countryCode);
        item.genderIconImage.sprite = genderIcons[(int)jumpers[index].gender];
        item.toggleExtension.SetElementId(index);
    }
}