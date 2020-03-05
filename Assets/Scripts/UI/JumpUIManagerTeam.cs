using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JumpUIManagerTeam : JumpUIManager
{
    public Label bib;
    public Label jumperName;
    public Label nextAthleteName;

    public Label rankPreJump;
    public Label totalPreJump;
    public Label[] metersPreJump;

    public Label rankPostJump;
    public Label totalPostJump;
    public Label[] metersPostJump;
    public GameObject[] metersPostJumpObj;
    public RectTransform row1;
    public RectTransform row2;
    private float row2height;
    private float nextAthleteY;
    public RectTransform nextAthleteTransform;
    public RectTransform judgesMarksTransform;
    public GameObject preJumpDataObj;
    public GameObject postJumpDataObj;
    public GameObject nextAthleteObj;
    public GameObject jumperImageObj;

    private RectTransform jumperImageTransform;
    private RectTransform rectTransform;

    private RawImage jumperImage;

    private void Start()
    {
        jumperImageTransform = jumperImageObj.GetComponent<RectTransform>();
        jumperImage = jumperImageObj.GetComponent<RawImage>();
        row2height = row2.rect.height;
        nextAthleteY = nextAthleteTransform.localPosition.y;
        this.rectTransform = GetComponent<RectTransform>();
        SetPreJumpUI();
    }

    public override void SetPreJumpUI()
    {
        postJumpDataObj.SetActive(false);
        nextAthleteObj.SetActive(true);
        preJumpDataObj.SetActive(true);

        rectTransform.localScale = new Vector3(0, 1, 1);
        // jumperImageTransform.localScale = new Vector3(1, 0, 1);
        jumperImageObj.SetActive(jumperImage.texture != null);

        jumperImageObj.SetActive(true);
        DOTween.Sequence().Append(rectTransform.DOScaleX(1, 0.5f));

        int jumpsCount = resultsManager.roundIndex;
        int competitorId = resultsManager.currentStartList[resultsManager.currentStartListIndex];
        int bib = resultsManager.results[competitorId].Bibs[resultsManager.roundIndex];
        int rank = resultsManager.results[competitorId].Rank;
        CompCal.Competitor competitor = participants.competitors[participants.participants[competitorId].competitors[0]];

        this.jumperName.Text.text = competitor.firstName + " " + competitor.lastName.ToUpper();
        this.bib.Text.text = bib.ToString();

        if (resultsManager.currentStartListIndex + 1 < resultsManager.currentStartList.Count)
        {
            int nextCompetitorId = resultsManager.currentStartList[resultsManager.currentStartListIndex + 1];
            CompCal.Competitor nextCompetitor = participants.competitors[participants.participants[nextCompetitorId].competitors[0]];
            nextAthleteName.Text.text = "Next athlete: " + nextCompetitor.firstName + " " + nextCompetitor.lastName.ToUpper();
        }
        else
        {
            nextAthleteObj.SetActive(false);

        }

        if (jumpsCount == 0)
        {
            DisableRow2();
            nextAthleteTransform.localPosition = new Vector3(nextAthleteTransform.localPosition.x, nextAthleteY + row2height);
        }
        else
        {
            EnableRow2();
            this.rankPreJump.Text.text = rank.ToString();
            nextAthleteTransform.localPosition = new Vector3(nextAthleteTransform.localPosition.x, nextAthleteY);

            int xx = Mathf.Max(0, jumpsCount - metersPreJump.Length);
            foreach (var item in metersPreJump)
            {
                if (xx < jumpsCount)
                {
                    item.Text.text = ((xx + 1) * 10).ToString("F1", CultureInfo.InvariantCulture) + " m";
                }
                else
                {
                    item.Text.text = "";
                }

                xx++;
            }
        }
    }

    public override void SetPostJumpUI()
    {
        nextAthleteObj.SetActive(false);
        preJumpDataObj.SetActive(false);
        jumperImageObj.SetActive(false);
        postJumpDataObj.SetActive(true);


        this.rectTransform.localScale = new Vector3(0, 1, 1);
        this.judgesMarksTransform.localScale = new Vector3(1, 0, 1);
        DOTween.Sequence().Append(rectTransform.DOScaleX(1, 0.5f)).Append(judgesMarksTransform.DOScaleY(1, 0.5f));

        int jumpsCount = resultsManager.roundIndex + 1;
        int competitorId = resultsManager.currentStartList[resultsManager.currentStartListIndex];
        int bib = resultsManager.results[competitorId].Bibs[resultsManager.roundIndex];
        int rank = resultsManager.results[competitorId].Rank;
        CompCal.Competitor competitor = participants.competitors[participants.participants[competitorId].competitors[0]];

        this.jumperName.Text.text = competitor.firstName + " " + competitor.lastName.ToUpper();
        this.bib.Text.text = bib.ToString();


        this.rankPostJump.Text.text = rank.ToString();
        // nextAthleteTransform.localPosition = new Vector3(nextAthleteTransform.localPosition.x, nextAthleteY);
        int xx = jumpsCount - metersPreJump.Length;
        int offset = Mathf.Max(0, metersPreJump.Length - jumpsCount);

        foreach (var item in metersPostJump)
        {
            if (0 <= xx)
            {
                item.Text.text = ((xx + 1) * 10).ToString("F1", CultureInfo.InvariantCulture) + " m";
            }
            else
            {
                item.Text.text = "";
            }
            xx++;
        }

    }

    private void EnableRow2()
    {
        preJumpDataObj.SetActive(true);
        row2.sizeDelta = new Vector2(row2.rect.width, row2height);
    }

    private void DisableRow2()
    {
        preJumpDataObj.SetActive(false);
        row2.sizeDelta = new Vector2(row2.rect.width, 0);
    }

    // void SetJumperInfo(bool value)
    // {
    //     jumperInfoObject.SetActive(value);

    //     if (value)
    //     {
    //         bibText.text = jumperResultInfo.competitor.bib.Value.ToString();
    //         bibText.color = jumperResultInfo.fontColor;
    //         nameText.text = jumperResultInfo.competitor.firstName.Value + " " + jumperResultInfo.competitor.lastName.Value.ToUpper();
    //         nameText.color = jumperResultInfo.fontColor;
    //     }
    // }

    // void SetCountryInfo(bool value)
    // {
    //     countryInfoObject.SetActive(value);

    //     if (value)
    //     {
    //         countryText.text = jumperResultInfo.competitor.countryCode.Value;
    //         countryText.color = jumperResultInfo.fontColor;
    //         countryFlagImage.sprite = flagsData.GetFlag(jumperResultInfo.competitor.countryCode.Value);
    //     }
    // }

    // private void SetResultInfo(bool value)
    // {
    //     resultInfoObject.SetActive(value);

    //     if (value)
    //     {
    //         resultText.text = jumperResultInfo.competitor.result.Value.ToString("#0.0");
    //         resultText.color = jumperResultInfo.fontColor;
    //     }
    // }

    // private void SetRankInfo(bool value)
    // {
    //     rankInfoObject.SetActive(value);

    //     if (value)
    //     {
    //         rankText.text = jumperResultInfo.competitor.rank.Value.ToString();
    //         rankText.color = jumperResultInfo.fontColor;
    //     }
    // }

    // public void SetUI()
    // {
    //     background.color = jumperResultInfo.backgroundColor;
    //     SetJumperInfo(jumperResultInfo.showNameInfo);
    //     SetCountryInfo(jumperResultInfo.showCountryInfo);
    //     SetResultInfo(jumperResultInfo.showResultInfo);
    //     SetRankInfo(jumperResultInfo.showRankInfo);
    // }

    // public void ShowPreJump()
    // {

    // }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetPreJumpUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetPostJumpUI();
        }

    }

}

