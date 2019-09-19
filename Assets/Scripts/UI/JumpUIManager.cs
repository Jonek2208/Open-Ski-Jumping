using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JumpUIManager : MonoBehaviour
{
    public JudgesController judgesController;

    public Slider gateSlider;
    public TMPro.TMP_Text gateText;
    public Slider windSlider;
    public TMPro.TMP_Text windText;
    public TMPro.TMP_Text distText;
    public TMPro.TMP_Text speedText;

    public TMPro.TMP_Text[] stylePtsText;

    public TMPro.TMP_Text totalPtsText;
    public GameObject pointsPanel;

    public void SetSpeed(float val)
    {
        speedText.text = (int)val + "." + (int)(val * 10 % 100) + " km/h";
        Debug.Log(val);
    }

    public void SetDistance(float val)
    {
        distText.text = (int)val + "." + (int)(val * 10 % 10) + " m";
    }

    public void SetPoints(float[] points, int lo, int hi, float total)
    {
        pointsPanel.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            stylePtsText[i].text = ""+points[i];
        }
        stylePtsText[lo].fontStyle = TMPro.FontStyles.Strikethrough;
        stylePtsText[hi].fontStyle = TMPro.FontStyles.Strikethrough;
        totalPtsText.text = "" + total;
    }

    public void UIReset()
    {
        speedText.text = "";
        distText.text = "";
        pointsPanel.SetActive(false);
        for (int i = 0; i < 5; i++)
        {
            stylePtsText[i].fontStyle = TMPro.FontStyles.Normal;
        }
    }

    public void GateSliderChange()
    {
        judgesController.SetGate((int)gateSlider.value);
        gateText.text = "Gate: " + (int)gateSlider.value;
    }

    public void SetGateSlider(int gate)
    {
        gateSlider.value = gate;
        gateText.text = "Gate: " + (int)gateSlider.value;
    }

    public void WindSliderChange()
    {
        judgesController.SetWind(windSlider.value);
        windText.text = "Wind: " + windSlider.value;
    }

    public void SetGateSliderRange(int gates)
    {
        gateSlider.minValue = 1;
        gateSlider.maxValue = gates;
        GateSliderChange();
    }


    private List<GameObject> resultsList;
    public GameObject resultPrefab;
    public GameObject resultsObject;
    public GameObject contentObject;

    public GameObject jumperInfo;
    public void ShowJumperInfo(string name, string surname, string country)
    {
        jumperInfo.SetActive(true);
        jumperInfo.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = country;
        jumperInfo.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = name + " " + surname;
    }

    public void ShowResults(List<JumperData> jumpers, Country[] countries, float[] totals)
    {
        resultsList = new List<GameObject>();
        resultsObject.SetActive(true);

        List<Vector2> unsortedRes = new List<Vector2>();
        for (int i = 0; i < totals.Length; i++)
        {
            unsortedRes.Add(new Vector2(totals[i], i));
        }
        var sortedRes = unsortedRes.OrderBy(v => v.x).ToList();
        for (int x = sortedRes.Count - 1; x >= 0; x--)
        {
            int i = Mathf.RoundToInt(sortedRes[x].y);
            GameObject tmp = Instantiate(resultPrefab);
            tmp.transform.SetParent(contentObject.transform);
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = jumpers[i].name + " " + jumpers[i].surname;
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = countries[jumpers[i].countryCode].alpha3;
            tmp.GetComponentsInChildren<TMPro.TMP_Text>()[2].text = totals[i].ToString();
            resultsList.Add(tmp);
        }
    }

    public void HideResults()
    {
        if(resultsList != null)
        foreach (var x in resultsList)
        {
            Destroy(x);
        }
        resultsList = new List<GameObject>();

        resultsObject.SetActive(false);
    }

    void Start()
    {

    }
}
