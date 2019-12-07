using UnityEngine;
using UnityEngine.UI;

public class JudgesMarkUI : MonoBehaviour
{
    [SerializeField]
    private Image flagImage;
    [SerializeField]
    private TMPro.TMP_Text markValueText;
    [SerializeField]
    private FlagsData flagsData;

    public void SetUI(JudgesMarkInfo judgesMarkInfo)
    {
        flagImage.sprite = flagsData.GetFlag(judgesMarkInfo.CountryCode);
        markValueText.text = judgesMarkInfo.MarkValue.ToString("#0.0");
        if (judgesMarkInfo.IsCounted) { markValueText.fontStyle = TMPro.FontStyles.Normal; }
        else { markValueText.fontStyle = TMPro.FontStyles.Strikethrough; }
    }
}
