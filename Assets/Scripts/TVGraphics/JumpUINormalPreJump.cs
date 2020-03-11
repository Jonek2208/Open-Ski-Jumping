using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class JumpUINormalPreJump : PreJumpUIManager
{
    public TMP_Text bib;
    public TMP_Text jumperName;
    public TMP_Text nextAthleteName;

    public TMP_Text rank;
    public TMP_Text total;
    public TMP_Text[] meters;

    public CountryInfo countryInfo;

    public RawImage jumperImage;
    public GameObject row1;
    public GameObject row2;
    public GameObject nextAthleteObj;
    // public RectTransform jumperImage;

    private RectTransform jumperImageTransform;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;


    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        Hide();
    }

    public override void Show()
    {
        canvasGroup.alpha = 1;
        StartCoroutine(LoadImage());
        SetCountry();

        rectTransform.localScale = new Vector3(0, 1, 1);
        // jumperImageTransform.localScale = new Vector3(1, 0, 1);
        // jumperImageObj.SetActive(jumperImage.texture != null);

        DOTween.Sequence().Append(rectTransform.DOScaleX(1, 0.5f));

        int jumpsCount = resultsManager.roundIndex;
        int competitorId = resultsManager.currentStartList[resultsManager.currentStartListIndex];
        int bib = resultsManager.results[competitorId].Bibs[resultsManager.roundIndex];
        int rank = resultsManager.lastRank[competitorId];
        CompCal.Competitor competitor = competitors.competitors[participants.participants[competitorId].competitors[resultsManager.subroundIndex]];

        this.jumperName.text = $"{competitor.firstName} {competitor.lastName.ToUpper()}";
        this.bib.text = bib.ToString();

        if (resultsManager.currentStartListIndex + 1 < resultsManager.currentStartList.Count)
        {
            nextAthleteObj.SetActive(true);
            int nextCompetitorId = resultsManager.currentStartList[resultsManager.currentStartListIndex + 1];
            CompCal.Competitor nextCompetitor = competitors.competitors[participants.participants[nextCompetitorId].competitors[resultsManager.subroundIndex]];
            nextAthleteName.text = $"Next athlete: {nextCompetitor.firstName} {nextCompetitor.lastName.ToUpper()}";
        }
        else
        {
            nextAthleteObj.SetActive(false);
        }

        if (jumpsCount == 0)
        {
            row2.SetActive(false);
        }
        else
        {
            row2.SetActive(true);
            this.rank.text = rank.ToString();

            CompCal.JumpResults jumpResults = resultsManager.results[competitorId].Results[resultsManager.subroundIndex];
            CompCal.JumpResult jump = jumpResults.results[resultsManager.roundIndex - 1];
            total.text = resultsManager.results[competitorId].TotalPoints.ToString("F1", CultureInfo.InvariantCulture);
            int xx = Mathf.Max(0, jumpsCount - meters.Length);
            foreach (var item in meters)
            {
                if (xx < jumpsCount)
                {
                    item.text = $"{jumpResults.results[xx].distance:F1} m";
                }
                else
                {
                    item.text = "";
                }

                xx++;
            }
        }
    }

    public void SetCountry()
    {
        int competitorId = resultsManager.currentStartList[resultsManager.currentStartListIndex];
        CompCal.Competitor competitor = competitors.competitors[participants.participants[competitorId].competitors[resultsManager.subroundIndex]];
        countryInfo.FlagImage.sprite = flagsData.GetFlag(competitor.countryCode);
        countryInfo.CountryName.text = competitor.countryCode;
    }

    IEnumerator LoadImage()
    {
        int competitorId = resultsManager.currentStartList[resultsManager.currentStartListIndex];
        CompCal.Competitor competitor = competitors.competitors[participants.participants[competitorId].competitors[resultsManager.subroundIndex]];
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(competitor.imagePath);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Image succesfully loaded");
            jumperImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }

    public override void Hide()
    {
        canvasGroup.DOFade(0, 0.5f);
    }

}
