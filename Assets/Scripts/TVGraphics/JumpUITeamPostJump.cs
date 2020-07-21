using System.Globalization;
using DG.Tweening;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace OpenSkiJumping.TVGraphics
{
    public class JumpUITeamPostJump : PostJumpUIManager
    {
        public TMP_Text bibTeam;
        public TMP_Text bibJumper;
        public TMP_Text teamName;
        public TMP_Text jumperName;
        [FormerlySerializedAs("rank")] public TMP_Text rankText;
        public TMP_Text totalTeam;
        public TMP_Text totalJumper;
        public TMP_Text[] meters;

        public CountryInfo countryInfo;

        [SerializeField] private RectTransform judgesMarksTransform;
        [SerializeField] private JudgesUIData judgesUIData;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        private void OnEnable()
        {
            InstantHide();
        }


        private void SetCountry()
        {
            var id = resultsManager.Value.GetCurrentCompetitorId();
            var team = competitors.teams[id];
            countryInfo.FlagImage.sprite = flagsData.GetFlag(team.countryCode);
            countryInfo.CountryName.text = team.countryCode;
        }

        public override void Show()
        {
            canvasGroup.alpha = 1;
            SetCountry();

            rectTransform.localScale = new Vector3(0, 1, 1);
            judgesMarksTransform.localScale = new Vector3(1, 0, 1);
            DOTween.Sequence().Append(rectTransform.DOScaleX(1, 0.5f)).Append(judgesMarksTransform.DOScaleY(1, 0.5f));

            var jumpsCount = resultsManager.Value.RoundIndex + 1;
            var competitorId = resultsManager.Value.GetCurrentCompetitorLocalId();
            var bib = resultsManager.Value.Results[competitorId].Bibs[resultsManager.Value.RoundIndex];
            var rank = resultsManager.Value.CompetitorRank(competitorId);
            var competitor = competitors.competitors[resultsManager.Value.OrderedParticipants[competitorId].competitors[resultsManager.Value.SubroundIndex]];
            var team = competitors.teams[resultsManager.Value.OrderedParticipants[competitorId].id];
            var jumpResults = resultsManager.Value.Results[competitorId].Results[resultsManager.Value.SubroundIndex];

            teamName.text = team.teamName.ToUpper();
            jumperName.text = $"{competitor.firstName} {competitor.lastName.ToUpper()}";
            bibTeam.text = bib.ToString();
            bibJumper.text = (resultsManager.Value.SubroundIndex + 1).ToString();
            rankText.text = rank.ToString();

            var xx = jumpsCount - meters.Length;
            var offset = Mathf.Max(0, meters.Length - jumpsCount);

            var jump = jumpResults.results[resultsManager.Value.RoundIndex];
            totalTeam.text = resultsManager.Value.Results[competitorId].TotalPoints.ToString("F1", CultureInfo.InvariantCulture);
            totalJumper.text = resultsManager.Value.Results[competitorId].TotalResults[resultsManager.Value.SubroundIndex].ToString("F1", CultureInfo.InvariantCulture);
            wind.SetValues(jump.windPoints);
            gate.SetValues(jump.gatePoints);

            for (var i = 0; i < judgesMarks.Length; i++)
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
}
