using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using CompetitionClasses;

public class CalendarCreatorScript : MonoBehaviour
{
    const string competitorsFileName = "competitors.json";
    public GameObject jumperPrefab;
    public TMPro.TMP_InputField lastNameInput;
    public TMPro.TMP_InputField firstNameInput;
    public TMPro.TMP_InputField countryInput;
    public TMPro.TMP_Dropdown genderDropdown;

    List<Competitor> allCompetitors;

    public void CreateList()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, competitorsFileName);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            allCompetitors = JsonConvert.DeserializeObject<List<Competitor>>(dataAsJson);
            foreach (var c in allCompetitors)
            {
                Debug.Log(c.firstName + " " + c.lastName);
            }
        }
    }

    void Start()
    {
        CreateList();
    }
}
