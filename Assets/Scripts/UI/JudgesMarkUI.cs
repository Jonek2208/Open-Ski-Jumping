using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class JudgesMarkUI : MonoBehaviour
    {
        public JudgesMarkUIData judgesMarkUIData;
        public Image flagImage;
        public TMP_Text countryText;
        public TMP_Text markValueText;
        public bool state;
        public void SetValues(decimal markValue, string countryCode, Sprite sprite, bool state)
        {
            markValueText.text = markValue.ToString("F1", CultureInfo.InvariantCulture);
            flagImage.sprite = sprite;
            countryText.text = countryCode;
            this.state = state;

            if (state)
            {
                markValueText.color = judgesMarkUIData.countedTextColor;
            }
            else
            {
                markValueText.color = judgesMarkUIData.notCountedTextColor;
            }
        }
    }
}
