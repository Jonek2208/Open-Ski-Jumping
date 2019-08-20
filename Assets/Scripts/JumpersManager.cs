using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Country
{
    public string full;
    public string alpha3;
    public int numericCode;
}

[System.Serializable]
public class JumperData
{
    public int id;
    public string name, surname;
    public int countryCode;
};

[System.Serializable]
public class CompetitorsData
{
    public Country[] countriesList;
    public List<JumperData> jumpersList;
}

public class JumpersManager : MonoBehaviour
{
    public enum State
    {
        init, edit
    }

    private int selected;
    State state;
    public GameObject contentObject;
    public GameObject jumperButtonPrefab;
    public TMPro.TMP_InputField nameInput;
    public TMPro.TMP_InputField surnameInput;
    public TMPro.TMP_Dropdown countryDropdown;
    public Button changeButton;
    public Button addButton;
    public Button deleteButton;

    public Country[] countries;
    public List<JumperData> jumpers;
    public List<int> deletedJumpers;
    public List<GameObject> jumperButtons;


    public void JumperButtonClick(int x)
    {
        Debug.Log(x);
        nameInput.text = jumpers[x].name;
        surnameInput.text = jumpers[x].surname;
        countryDropdown.value = jumpers[x].countryCode;
        state = State.edit;
        changeButton.interactable = true;
        deleteButton.interactable = true;
        selected = x;
        jumperButtons[selected].GetComponent<Button>();
    }

    public void AddButtonClick()
    {
        GameObject tmp = Instantiate(jumperButtonPrefab);
        tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = countries[countryDropdown.value].alpha3;
        tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = nameInput.text + " " + surnameInput.text;
        tmp.transform.SetParent(contentObject.transform);
        tmp.GetComponent<ButtonScript>().jumpersManager = this;
        JumperData jmp = new JumperData();
        jmp.countryCode = countryDropdown.value;
        jmp.name = nameInput.text;
        jmp.surname = surnameInput.text;
        tmp.GetComponent<ButtonScript>().index = jumpers.Count;
        selected = jumpers.Count;
        jumperButtons.Add(tmp);
        jumpers.Add(jmp);

        changeButton.interactable = true;
        deleteButton.interactable = true;
    }

    public void ChangeButtonClick()
    {
        Debug.Log(selected);
        GameObject tmp = jumperButtons[selected];
        tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = countries[countryDropdown.value].alpha3;
        tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = nameInput.text + " " + surnameInput.text;
        JumperData jmp = jumpers[selected];
        jmp.countryCode = countryDropdown.value;
        jmp.name = nameInput.text;
        jmp.surname = surnameInput.text;
    }

    public void DeleteButtonClick()
    {
        GameObject tmp = jumperButtons[selected];
        deletedJumpers.Add(selected);
        Destroy(tmp);
        selected = -1;
        state = State.init;
        changeButton.interactable = false;
        deleteButton.interactable = false;
    }

    public void GenerateList()
    {
        jumpers = LoadData();
        int i = 0;
        foreach (var x in jumpers)
        {
            // Debug.Log(x.name + " " + x.surname + " " + countries[x.countryCode].alpha3);
            GameObject tmp = Instantiate(jumperButtonPrefab);
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = countries[x.countryCode].alpha3;
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = x.name + " " + x.surname;
            tmp.transform.SetParent(contentObject.transform);
            tmp.GetComponent<ButtonScript>().index = i;
            tmp.GetComponent<ButtonScript>().jumpersManager = this;
            jumperButtons.Add(tmp);
            i++;
        }
    }

    public void GenerateCountries()
    {
        countryDropdown.options = new List<TMPro.TMP_Dropdown.OptionData>();

        foreach (var x in countries)
        {
            TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData();
            option.text = x.full;
            countryDropdown.options.Add(option);
        }
        countryDropdown.value = -1;
        countryDropdown.value = 0;

    }

    public void GenerateCompetitors()
    {
        deletedJumpers.Sort();
        deletedJumpers.Reverse();
        foreach (int it in deletedJumpers)
        {
            jumpers.RemoveAt(it);
            jumperButtons.RemoveAt(it);
        }

        int i = 0;

        foreach (var item in jumperButtons)
        {
            item.GetComponent<ButtonScript>().index = i;
            i++;
        }

        CompetitorsData cd = new CompetitorsData();
        cd.jumpersList = jumpers;
        cd.countriesList = countries;
        string dataAsJson = JsonUtility.ToJson(cd);
        PlayerPrefs.SetString("competitorsData", dataAsJson);
        Debug.Log(dataAsJson);
    }

    private List<JumperData> LoadData()
    {
        if (PlayerPrefs.HasKey("competitorsData"))
        {
            string dataAsJson = PlayerPrefs.GetString("competitorsData");
            CompetitorsData loadedData = JsonUtility.FromJson<CompetitorsData>(dataAsJson);
            return loadedData.jumpersList;
        }
        else
        {
            return jumpers;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateCountries();
        GenerateList();
        state = State.init;
        changeButton.interactable = false;
        deleteButton.interactable = false;
        selected = -1;
        // GenerateCompetitors();
    }    
}
