using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumperResultUI : MonoBehaviour
{
    public Image background;
    [Header("Jumper Info")]
    public GameObject jumperInfoObject;
    public TMPro.TMP_Text bibText;
    public TMPro.TMP_Text nameText;

    [Header("Country Info")]
    public GameObject countryInfoObject;
    public TMPro.TMP_Text countryText;
    public Image countryFlagImage;

    [Header("Result Info")]
    public GameObject resultInfoObject;
    public TMPro.TMP_Text resultText;
    [Header("Rank Info")]
    public GameObject rankInfoObject;
    public TMPro.TMP_Text rankText;

    [Header("Other")]
    public FlagsData flagsData;
    public JumperResultInfoData jumperResultInfo;

    private void SetJumperInfo(bool value)
    {
        jumperInfoObject.SetActive(value);

        if (value)
        {
            bibText.text = jumperResultInfo.competitor.bib.Value.ToString();
            bibText.color = jumperResultInfo.fontColor;
            nameText.text = jumperResultInfo.competitor.firstName.Value + " " + jumperResultInfo.competitor.lastName.Value.ToUpper();
            nameText.color = jumperResultInfo.fontColor;
        }
    }

    private void SetCountryInfo(bool value)
    {
        countryInfoObject.SetActive(value);

        if (value)
        {
            countryText.text = jumperResultInfo.competitor.countryCode.Value;
            countryText.color = jumperResultInfo.fontColor;
            countryFlagImage.sprite = flagsData.GetFlag(jumperResultInfo.competitor.countryCode.Value);
        }
    }

    private void SetResultInfo(bool value)
    {
        resultInfoObject.SetActive(value);

        if (value)
        {
            resultText.text = jumperResultInfo.competitor.result.Value.ToString("#0.0");
            resultText.color = jumperResultInfo.fontColor;
        }
    }

    private void SetRankInfo(bool value)
    {
        rankInfoObject.SetActive(value);

        if (value)
        {
            rankText.text = jumperResultInfo.competitor.rank.Value.ToString();
            rankText.color = jumperResultInfo.fontColor;
        }
    }

    public void SetUI()
    {
        background.color = jumperResultInfo.backgroundColor;
        SetJumperInfo(jumperResultInfo.showNameInfo);
        SetCountryInfo(jumperResultInfo.showCountryInfo);
        SetResultInfo(jumperResultInfo.showResultInfo);
        SetRankInfo(jumperResultInfo.showRankInfo);
    }

}