using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TournamentMenuController : MonoBehaviour
{
    public DatabaseManager databaseManager;
    public FlagsData flagsData;
    private GameSave gameSave;
    private Competition.ResultsDatabase resultsContainer;
    private Competition.Calendar calendar;
    private List<GameObject> resultsList;
    public TMPro.TMP_Dropdown resultsDropdown;
    public GameObject resultsWrapperPrefab;
    public GameObject resultPrefab;
    public GameObject resultPointsPrefab;
    public GameObject scrollViewObject;
    public TMPro.TMP_Text resultsInfoText;
    public GameObject contentObject;

    private void Start()
    {
        resultsContainer = databaseManager.dbSaveData.savesList[databaseManager.dbSaveData.currentSaveId].resultsContainer;
        calendar = databaseManager.dbSaveData.savesList[databaseManager.dbSaveData.currentSaveId].calendar;
        var dropdownList = calendar.classifications.Select(x => new TMPro.TMP_Dropdown.OptionData(x.name));
        resultsDropdown.options = new List<TMPro.TMP_Dropdown.OptionData>(dropdownList);
        ShowEventResults(resultsContainer, 0);
    }

    public void OnDropdownChange()
    {
        HideResults();
        ShowEventResults(resultsContainer, resultsDropdown.value);
    }

    public void LoadNextEvent()
    {
        if (resultsContainer.eventIt >= calendar.events.Count)
        {
            databaseManager.dbSaveData.savesList.RemoveAt(databaseManager.dbSaveData.currentSaveId);
            databaseManager.dbSaveData.currentSaveId = -1;
            databaseManager.Save();
        }
        else
        {
        }
    }

    public void ShowEventResults(Competition.ResultsDatabase calendarResults, int classificationId)
    {
        resultsList = new List<GameObject>();
        resultsInfoText.text = calendar.classifications[classificationId].name;

        float width = resultPrefab.GetComponent<RectTransform>().rect.width + resultPointsPrefab.GetComponent<RectTransform>().rect.width;
        scrollViewObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, scrollViewObject.GetComponent<RectTransform>().sizeDelta.y);

        for (int i = 0; i < calendarResults.classificationResults[classificationId].totalSortedResults.Count; i++)
        {
            // int x = calendarResults.classificationResults[classificationId].totalSortedResults.Values[i];
            // GameObject wrapper = Instantiate(resultsWrapperPrefab);
            // wrapper.GetComponent<RectTransform>().sizeDelta = new Vector2(width, wrapper.GetComponent<RectTransform>().sizeDelta.y);
            // GameObject tmp = Instantiate(resultPrefab);
            // wrapper.transform.SetParent(contentObject.transform, false);
            // tmp.transform.SetParent(wrapper.transform, false);
            // tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.calendar.competitors[x].firstName + " " + calendarResults.calendar.competitors[x].lastName.ToUpper();
            // tmp.GetComponentsInChildren<Image>()[1].sprite = flagsData.GetFlag(calendarResults.calendar.competitors[x].countryCode);
            // tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = calendarResults.calendar.competitors[x].countryCode;
            // tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = calendarResults.classificationResults[classificationId].rank[x].ToString();

            // tmp = Instantiate(resultPointsPrefab);
            // tmp.transform.SetParent(wrapper.transform, false);
            // tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = calendarResults.classificationResults[classificationId].totalSortedResults.Keys[i].Item1.ToString("#0.0");
            // resultsList.Add(wrapper);
        }
    }

    public void HideResults()
    {
        if (resultsList != null)
        {
            foreach (var x in resultsList)
            {
                Destroy(x);
            }
        }

        resultsList = new List<GameObject>();
    }

}
