using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    public class SimpleColorPicker : MonoBehaviour
    {
        public TMP_InputField input;
        public Image image;
        [SerializeField] private Color currentColor;

        private void UpdateUI()
        {
            input.text = ToHex;
            image.color = currentColor;
        }

        public event Action OnColorChange;

        public static bool TryParseHex(string hex, out int redVal, out int greenVal, out int blueVal)
        {
            redVal = 0;
            greenVal = 0;
            blueVal = 0;

            if (hex.Length != 6) return false;

            string redString = hex.Substring(0, 2);
            string greenString = hex.Substring(2, 2);
            string blueString = hex.Substring(4, 2);

            return int.TryParse(redString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out redVal) &&
                   int.TryParse(greenString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out greenVal) &&
                   int.TryParse(blueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out blueVal);
        }

        public static Color Hex2Color(string hex)
        {
            hex = hex ?? "000000";

            TryParseHex(hex, out var red, out var green, out var blue);
            return new Color(red / 255f, green / 255f, blue / 255f);
        }

        public void SetValue(string hex, bool notify = true)
        {
            hex = hex ?? "000000";
            TryParseHex(hex, out var red, out var green, out var blue);
            currentColor = new Color(red / 255f, green / 255f, blue / 255f);
            ToHex = hex;
            image.color = currentColor;
            input.SetTextWithoutNotify(ToHex);
            if (notify)
                OnColorChange?.Invoke();
        }

        public void HandleValueChanged()
        {
            var hex = input.text;
            bool tmp = TryParseHex(hex, out var red, out var green, out var blue);
            if (!tmp)
            {
                hex = "000000";
                input.SetTextWithoutNotify(hex);
            }

            currentColor = new Color(red / 255f, green / 255f, blue / 255f);
            image.color = currentColor;
            ToHex = hex;
            OnColorChange?.Invoke();
        }

        public void SetValueWithoutNotify(string hex) => SetValue(hex, false);

        public Color CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;
                UpdateUI();
            }
        }

        public string ToHex { get; private set; }
    }
}