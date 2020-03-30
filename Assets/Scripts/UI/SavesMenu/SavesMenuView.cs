using System;
using System.Collections.Generic;
using System.Linq;
using CompCal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ISavesMenuView
{
    List<string> Items { set; }
    List<string> Calendars { set; }
    int CurrentSelection { get; set; }
    void SetCurrentSelectionToToggleGroup();
    int PopUpValue { get; }
    string CurrentSaveName { set; }
    string NewSaveName { get; set; }

    event Action OnSelectionChanged;
    event Action OnAdd;
    event Action OnRemove;
    event Action OnSubmit;

    void HidePopUp();
    void HideSaveInfo();
    void ShowPopUp();
    void ShowPrompt();
    void ShowSaveInfo();
}

public class SavesMenuPresenter
{
    private ISavesMenuView view;
    private SavesRuntime saves;
    private CalendarsRuntime calendars;
    public SavesMenuPresenter(ISavesMenuView view, SavesRuntime saves, CalendarsRuntime calendars)
    {
        this.view = view;
        this.saves = saves;
        this.calendars = calendars;

        InitEvents();
        SetInitValues();
    }

    private GameSave CreateGameSaveFromCalendar(string name, Calendar calendar)
    {
        GameSave gameSave = new GameSave();
        gameSave.name = name;
        gameSave.resultsContainer = new ResultsDatabase();
        gameSave.calendar = calendar;

        gameSave.resultsContainer.eventResults = new EventResults[gameSave.calendar.events.Count];
        gameSave.resultsContainer.classificationResults = new ClassificationResults[gameSave.calendar.classifications.Count];

        for (int i = 0; i < gameSave.resultsContainer.classificationResults.Length; i++)
        {
            gameSave.resultsContainer.classificationResults[i] = new ClassificationResults();
        }

        return gameSave;
    }

    private void CreateNewSave()
    {
        GameSave save = CreateGameSaveFromCalendar(this.view.NewSaveName, this.calendars.Data[this.view.PopUpValue]);
        this.saves.Data.savesList.Add(save);
        PresentList();
    }

    private void RemoveSave()
    {
        if (this.view.CurrentSelection < 0 || this.view.CurrentSelection >= this.saves.Data.savesList.Count) { return; }
        this.saves.Data.savesList.RemoveAt(this.view.CurrentSelection);
        this.view.CurrentSelection = Mathf.Min(this.view.CurrentSelection, this.saves.Data.savesList.Count - 1);
        this.view.SetCurrentSelectionToToggleGroup();
        PresentList();
        PresentSaveInfo();
    }

    private void PresentList()
    {
        this.view.Items = this.saves.Data.savesList.Select(item => item.name).ToList();
    }

    private void PresentSaveInfo()
    {
        if (this.view.CurrentSelection < 0 || this.view.CurrentSelection >= this.saves.Data.savesList.Count)
        {
            this.view.HideSaveInfo();
            return;
        }
        this.view.ShowSaveInfo();
        this.view.CurrentSaveName = this.saves.Data.savesList[this.view.CurrentSelection].name;
    }

    private void OnAdd()
    {
        this.view.ShowPopUp();
    }

    private void OnSubmit()
    {
        if (this.view.NewSaveName.Length > 0)
        {
            CreateNewSave();
            this.view.HidePopUp();
        }
        else
        {
            this.view.ShowPrompt();
        }
    }

    private void InitEvents()
    {
        this.view.OnSelectionChanged += PresentSaveInfo;
        this.view.OnAdd += OnAdd;
        this.view.OnRemove += RemoveSave;
        this.view.OnSubmit += OnSubmit;
    }

    private void SetInitValues()
    {
        PresentList();
        PresentSaveInfo();
        this.view.Calendars = this.calendars.Data.Select(item => item.name).ToList();
    }

}

public class SavesMenuView : MonoBehaviour, ISavesMenuView
{
    private SavesMenuPresenter presenter;
    [SerializeField] private SavesRuntime savesRuntime;
    [SerializeField] private CalendarsRuntime calendarsRuntime;

    [SerializeField] private SavesListView listView;
    [SerializeField] private ToggleGroupExtension toggleGroup;

    [SerializeField] private GameObject saveInfoObj;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;

    [SerializeField] private GameObject popUpRoot;
    [SerializeField] private GameObject promptObj;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button cancelButton;

    public event Action OnSelectionChanged;
    public event Action OnAdd;
    public event Action OnRemove;
    public event Action OnSubmit;

    public List<string> Items { set { this.listView.Items = value; this.listView.Refresh(); } }
    public List<string> Calendars { set { this.dropdown.ClearOptions(); this.dropdown.AddOptions(value); } }

    private int currentSelection;
    public int CurrentSelection { get; set; }
    public void SetCurrentSelectionToToggleGroup()
    {
        toggleGroup.SetCurrentId(CurrentSelection);
        listView.RefreshShownValue();
    }

    public int PopUpValue { get; }

    public string CurrentSaveName { set { this.nameText.text = value; } }
    public string NewSaveName { get { return input.text; } set { input.text = value; } }

    private void Start()
    {
        this.toggleGroup.onValueChanged += (val) => { CurrentSelection = val; OnSelectionChanged?.Invoke(); };
        this.addButton.onClick.AddListener(() => OnAdd?.Invoke());
        this.removeButton.onClick.AddListener(() => OnRemove?.Invoke());
        this.submitButton.onClick.AddListener(() => OnSubmit?.Invoke());
        this.cancelButton.onClick.AddListener(HidePopUp);
        this.presenter = new SavesMenuPresenter(this, savesRuntime, calendarsRuntime);
    }

    public void ShowSaveInfo() => saveInfoObj.SetActive(true);
    public void HideSaveInfo() => saveInfoObj.SetActive(false);
    public void ShowPopUp()
    {
        popUpRoot.SetActive(true);
        promptObj.SetActive(false);
        input.text = "";
    }
    public void ShowPrompt() => promptObj.SetActive(true);
    public void HidePopUp() => popUpRoot.SetActive(false);
}