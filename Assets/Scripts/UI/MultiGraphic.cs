using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    public class MultiGraphic : Graphic
    {
        [SerializeField] private Graphic[] graphics;

        public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
        {
            foreach (var graphic in graphics)
            {
                graphic.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, true);
            }
        }
    }
}