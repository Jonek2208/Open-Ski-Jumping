using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class JudgesMarkUI : MonoBehaviour
{
    public JudgesMarkUIData judgesMarkUIData;
    public Image flagImage;
    public TMPro.TMP_Text countryText;
    public TMPro.TMP_Text markValueText;
    public bool state;
    public void SetValues(decimal markValue, string countryCode, Sprite sprite, bool state)
    {
        this.markValueText.text = markValue.ToString("F1", CultureInfo.InvariantCulture);
        this.flagImage.sprite = sprite;
        this.countryText.text = countryCode;
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
