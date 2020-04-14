using DG.Tweening;
using ScriptableObjects.Variables;
using TMPro;
using UnityEngine;

namespace TVGraphics
{
    public class JumpUIToBeat : ToBeatUIManager
    {
        [SerializeField] private FloatVariable toBeatDistFV;
        [SerializeField] private TMP_Text toBeatText;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        private void OnEnable()
        {
            InstantHide();
        }

        public override void Show()
        {
            toBeatText.text = $"To beat: {toBeatDistFV.Value:F1} m";
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



