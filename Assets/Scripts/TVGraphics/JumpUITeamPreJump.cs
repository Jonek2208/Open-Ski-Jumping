using System.Globalization;
using DG.Tweening;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.UI;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.TVGraphics
{
    public class JumpUITeamPreJump : PreJumpUIManager
    {
        public TMP_Text bibTeam;
        public TMP_Text bibJumper;
        public TMP_Text teamName;
        public TMP_Text jumperName;
        public TMP_Text nextAthleteName;
        public TMP_Text rank;
        public TMP_Text totalTeam;
        public TMP_Text totalJumper;
        public TMP_Text[] meters;

        public CountryInfo countryInfo;

        public GameObject row1;
        public GameObject rankObj;
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
            Competitor competitor = competitors.competitors[
                resultsManager.Value.OrderedParticipants[competitorId].competitors[resultsManager.Value.SubroundIndex]];
            Team team = competitors.teams[resultsManager.Value.OrderedParticipants[competitorId].id];

            teamName.text = team.teamName.ToUpper();
            jumperName.text = $"{competitor.firstName} {competitor.lastName.ToUpper()}";
            bibTeam.text = bib.ToString();
            bibJumper.text = (resultsManager.Value.SubroundIndex + 1).ToString();

            if (resultsManager.Value.StartListIndex + 1 < resultsManager.Value.StartList.Count)
            {
                nextAthleteObj.SetActive(true);
                int nextCompetitorId = resultsManager.Value.StartList[resultsManager.Value.StartListIndex + 1];
                Competitor nextCompetitor = competitors.competitors[
                    resultsManager.Value.OrderedParticipants[nextCompetitorId]
                        .competitors[resultsManager.Value.SubroundIndex]];
                nextAthleteName.text = $"Next athlete: {nextCompetitor.firstName} {nextCompetitor.lastName.ToUpper()}";
            }
            else
            {
                nextAthleteObj.SetActive(false);
            }

            if (jumpsCount == 0 && resultsManager.Value.SubroundIndex == 0)
            {
                totalTeam.text = "";
                rankObj.SetActive(false);
            }
            else
            {
                rankObj.SetActive(true);
                this.rank.text = rank.ToString();
                totalTeam.text = resultsManager.Value.Results[competitorId].TotalPoints
                    .ToString("F1", CultureInfo.InvariantCulture);
            }

            if (jumpsCount == 0)
            {
                totalJumper.text = "";
                foreach (var item in meters)
                {
                    item.text = "";
                }
            }
            else
            {
                JumpResults jumpResults = resultsManager.Value.Results[competitorId]
                    .Results[resultsManager.Value.SubroundIndex];
                JumpResult jump = jumpResults.results[resultsManager.Value.RoundIndex - 1];
                totalJumper.text = resultsManager.Value.Results[competitorId]
                    .TotalResults[resultsManager.Value.SubroundIndex].ToString("F1", CultureInfo.InvariantCulture);
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
            var id = resultsManager.Value.GetCurrentCompetitorId();
            var team = competitors.teams[id];
            countryInfo.FlagImage.sprite = flagsData.GetFlag(team.countryCode);
            countryInfo.CountryName.text = team.countryCode;
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