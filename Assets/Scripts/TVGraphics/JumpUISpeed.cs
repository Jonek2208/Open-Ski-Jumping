using System.Globalization;
using DG.Tweening;
using OpenSkiJumping.Competition.Runtime;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.TVGraphics
{
    public class JumpUISpeed : SpeedUIManager
    {
        [SerializeField] private RuntimeJumpData jumpData;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        private void OnEnable()
        {
            InstantHide();
        }

        public override void Show()
        {
            speedText.text = $"{jumpData.Speed.ToString("F1", CultureInfo.InvariantCulture)} km/h";
            canvasGroup.alpha = 1;
            rectTransform.localScale = new Vector3(0, 1, 1);
            DOTween.Sequence().Append(rectTransform.DOScaleX(1, 0.5f));
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



