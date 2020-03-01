using UnityEngine;
using System.Globalization;
using UnityEngine.UI;
using System;

public class CompensationUI : MonoBehaviour
{
    public int sign;
    public CompensationUIData compensationUIData;
    public Image background;
    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text valueText;
    public void SetValues(decimal value)
    {
        sign = Math.Sign(value);
        nameText.color = compensationUIData.textColors[sign + 1];
        valueText.color = compensationUIData.textColors[sign + 1];
        background.color = compensationUIData.backgroundColors[sign + 1];
        valueText.text = value.ToString("F1", CultureInfo.InvariantCulture);
    }
}
