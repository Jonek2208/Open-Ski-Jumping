using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using CompetitionClasses;

public class CalendarCreatorScript : MonoBehaviour
{
    const string competitorsFileName = "competitors.json";
    const string countryDataFileName = "country_data.json";
    public GameObject jumpersContentObject;
    public GameObject jumperPrefab;
    public SpriteAtlas flagsSprite;
    public TMPro.TMP_InputField classificationNameInput;
    public TMPro.TMP_Dropdown classificationDropdown1;
    public TMPro.TMP_Dropdown classificationDropdown2;
    public GameObject classificationsContentObject;
    public GameObject classificationPrefab;

    List<Competitor> allCompetitors;
    List<Classification> classifications;
    public void CreateCompetitorsList()
    {

        Dictionary<string, int> countriesDict = new Dictionary<string, int>();

        string filePath = Path.Combine(Application.streamingAssetsPath, countryDataFileName);
        CountryData countryData = new CountryData();
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            countryData = JsonConvert.DeserializeObject<CountryData>(dataAsJson);
            for (int i = 0; i < countryData.spritesList.Count; i++)
            {
                countriesDict.Add(countryData.spritesList[i], i);
            }

            foreach (var c in countryData.countryList)
            {
                countriesDict.Add(c.ioc, countriesDict[c.alpha2]);
            }
        }

        foreach (var c in countryData.countryList)
        {
            Debug.Log(c.en + " " + countriesDict[c.ioc] + " " + countriesDict[c.alpha2]);
        }

        filePath = Path.Combine(Application.streamingAssetsPath, competitorsFileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            allCompetitors = JsonConvert.DeserializeObject<List<Competitor>>(dataAsJson);
            foreach (var c in allCompetitors)
            {
                GameObject tmp = Instantiate(jumperPrefab);
                tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = c.lastName.ToUpper() + " " + c.firstName;
                tmp.GetComponentsInChildren<Image>()[3].sprite = flagsSprite.GetSprite("flags_responsive_uncompressed_" + countriesDict[c.countryCode].ToString());
                tmp.transform.SetParent(jumpersContentObject.transform);
            }
        }
    }

    public void OnAddClassificationButton()
    {
        int dropdown1val = classificationDropdown1.value;
        int dropdown2val = classificationDropdown2.value;
        ClassificationType ct = (ClassificationType)(2 * dropdown1val + dropdown2val);
        classifications.Add(new Classification(classificationNameInput.text, ct));

        GameObject tmp = Instantiate(classificationPrefab);
        tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = classificationNameInput.text;
        tmp.transform.SetParent(classificationsContentObject.transform);
        tmp.GetComponent<ScrollViewElemScript>().calendarCreatorScript = this;
        tmp.GetComponent<ScrollViewElemScript>().index = classifications.Count - 1;

        tmp.GetComponent<Toggle>().group = classificationsContentObject.GetComponent<ToggleGroup>();
    }

    public void UpdateClassificationsPanel(int index)
    {
        classificationNameInput.text = classifications[index].name;
        classificationDropdown1.value = (int)(classifications[index].classificationType) / 2;
        classificationDropdown2.value = (int)(classifications[index].classificationType) % 2;
    }

    void Start()
    {
        classifications = new List<Classification>();
        CreateCompetitorsList();
    }
}