using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    public class CompensationUI : MonoBehaviour
    {
        public int sign;
        public CompensationUIData compensationUIData;
        public Image background;
        public TMP_Text nameText;
        public TMP_Text valueText;
        public void SetValues(decimal value)
        {
            sign = Math.Sign(value);
            nameText.color = compensationUIData.textColors[sign + 1];
            valueText.color = compensationUIData.textColors[sign + 1];
            background.color = compensationUIData.backgroundColors[sign + 1];
            // valueText.text = value.ToString("F1", CultureInfo.InvariantCulture);
            
            valueText.text = value.ToString("+0.0;-0.0;0.0");
        }
    }
}
