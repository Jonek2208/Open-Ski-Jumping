using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using DG.Tweening;

public class JumpUINormalPostJump : PostJumpUIManager
{
    [SerializeField] private TMP_Text bib;
    [SerializeField] private TMP_Text jumperName;
    [SerializeField] private TMP_Text rank;
    [SerializeField] private TMP_Text total;
    [SerializeField] private TMP_Text[] meters;
    [SerializeField] private CountryInfo countryInfo;
    [SerializeField] private RectTransform judgesMarksTransform;
    [SerializeField] private JudgesUIData judgesUIData;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        InstantHide();
    }


    public void SetCountry()
    {
        int competitorId = resultsManager.currentStartList[resultsManager.startListIndex];
        CompCal.Competitor competitor = competitors.competitors[participants.participants[competitorId].competitors[resultsManager.subroundIndex]];
        countryInfo.FlagImage.sprite = flagsData.GetFlag(competitor.countryCode);
        countryInfo.CountryName.text = competitor.countryCode;
    }

    public override void Show()
    {
        canvasGroup.alpha = 1;
        SetCountry();

        this.rectTransform.localScale = new Vector3(0, 1, 1);
        this.judgesMarksTransform.localScale = new Vector3(1, 0, 1);
        DOTween.Sequence().Append(rectTransform.DOScaleX(1, 0.5f)).Append(judgesMarksTransform.DOScaleY(1, 0.5f));

        int jumpsCount = resultsManager.roundIndex + 1;
        int competitorId = resultsManager.currentStartList[resultsManager.startListIndex];
        int bib = resultsManager.results[competitorId].Bibs[resultsManager.roundIndex];
        int rank = resultsManager.results[competitorId].Rank;
        CompCal.Competitor competitor = competitors.competitors[participants.participants[competitorId].competitors[resultsManager.subroundIndex]];
        CompCal.JumpResults jumpResults = resultsManager.results[competitorId].Results[resultsManager.subroundIndex];

        this.jumperName.text = $"{competitor.firstName} {competitor.lastName.ToUpper()}";
        this.bib.text = bib.ToString();
        this.rank.text = rank.ToString();

        int xx = jumpsCount - meters.Length;
        int offset = Mathf.Max(0, meters.Length - jumpsCount);

        CompCal.JumpResult jump = jumpResults.results[resultsManager.roundIndex];
        total.text = resultsManager.results[competitorId].TotalPoints.ToString("F1", CultureInfo.InvariantCulture);
        wind.SetValues(jump.windPoints);
        gate.SetValues(jump.gatePoints);

        for (int i = 0; i < judgesMarks.Length; i++)
        {
            judgesMarks[i].SetValues(jump.judgesMarks[i], judgesUIData.countries[i], flagsData.GetFlag(judgesUIData.countries[i]), jump.judgesMask[i]);
        }

        foreach (var item in meters)
        {
            if (0 <= xx)
            {
                item.text = $"{jumpResults.results[xx].distance:F1} m";
            }
            else
            {
                item.text = "";
            }
            xx++;
        }
        // listView.AddItem(new ResultData() { firstName = competitor.firstName, lastName = competitor.lastName.ToUpper(), result = (float)resultsManager.results[competitorId].TotalPoints });
        // listView.Items = listView.Items.OrderByDescending(item => item.result).ToList();
    }

    public override void Hide()
    {
        canvasGroup.DOFade(0, 0.5f);
    }

    public override void InstantHide()
    {
        canvasGroup.alpha = 0;
    }


}
