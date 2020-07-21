using System.Globalization;
using DG.Tweening;
using OpenSkiJumping.UI;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.TVGraphics
{
    public class JumpUINormalPreJump : PreJumpUIManager
    {
        public TMP_Text bib;
        public TMP_Text jumperName;
        public TMP_Text nextAthleteName;

        public TMP_Text rank;
        public TMP_Text total;
        public TMP_Text[] meters;

        public CountryInfo countryInfo;

        public GameObject row1;
        public GameObject row2;
        public GameObject nextAthleteObj;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        private void OnEnable()
        {
            InstantHide();
        }

        public override void Show()
        {
            canvasGroup.alpha = 1;
            LoadImage();
            SetCountry();

            rectTransform.localScale = new Vector3(0, 1, 1);
            // jumperImageTransform.localScale = new Vector3(1, 0, 1);
            // jumperImageObj.SetActive(jumperImage.texture != null);

            DOTween.Sequence().Append(rectTransform.DOScaleX(1, 0.5f));

            int jumpsCount = resultsManager.Value.RoundIndex;
            int competitorId = resultsManager.Value.GetCurrentCompetitorLocalId();
            int bib = resultsManager.Value.Results[competitorId].Bibs[resultsManager.Value.RoundIndex];
            int rank = resultsManager.Value.LastRank[competitorId];
            var competitor = GetCompetitorById(competitorId, resultsManager.Value.SubroundIndex);


            jumperName.text = $"{competitor.firstName} {competitor.lastName.ToUpper()}";
            this.bib.text = bib.ToString();

            if (resultsManager.Value.StartListIndex + 1 < resultsManager.Value.StartList.Count)
            {
                nextAthleteObj.SetActive(true);
                int nextCompetitorId = resultsManager.Value.StartList[resultsManager.Value.StartListIndex + 1];
                var nextCompetitor = competitors.competitors[
                    resultsManager.Value.OrderedParticipants[nextCompetitorId]
                        .competitors[resultsManager.Value.SubroundIndex]];
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

                var jumpResults = resultsManager.Value.Results[competitorId]
                    .Results[resultsManager.Value.SubroundIndex];
                var jump = jumpResults.results[resultsManager.Value.RoundIndex - 1];
                total.text = resultsManager.Value.Results[competitorId].TotalPoints
                    .ToString("F1", CultureInfo.InvariantCulture);
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
        
        private void SetCountry()
        {
            var id = resultsManager.Value.GetCurrentJumperId();
            var competitor = competitors.competitors[id];
            countryInfo.FlagImage.sprite = flagsData.GetFlag(competitor.countryCode);
            countryInfo.CountryName.text = competitor.countryCode;
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
}