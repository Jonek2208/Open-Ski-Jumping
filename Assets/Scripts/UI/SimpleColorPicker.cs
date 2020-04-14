using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SimpleColorPicker : MonoBehaviour
    {
        public TMP_InputField input;
        public Image image;
        [SerializeField]
        private Color currentColor;
        private string hexColor;

        private void UpdateUI()
        {
            input.text = hexColor;
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

            if (!int.TryParse(redString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out redVal))
            { return false; }
            if (!int.TryParse(greenString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out greenVal))
            { return false; }
            if (!int.TryParse(blueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out blueVal))
            { return false; }

            return true;
        }

        public static Color Hex2Color(string hex)
        {
            int red, green, blue;
            if (hex == null) { hex = "000000"; }
            TryParseHex(hex, out red, out green, out blue);
            return new Color(red / 255f, green / 255f, blue / 255f);
        }

        public void SetColor(string hex)
        {
            int red, green, blue;
            if (hex == null) { hex = "000000"; }
            TryParseHex(hex, out red, out green, out blue);
            Debug.Log("COLOR: " + red + " " + green + " " + blue);
            currentColor = new Color(red / 255f, green / 255f, blue / 255f);
            hexColor = hex;
            image.color = currentColor;
            OnColorChange?.Invoke();
        }

        public void Set(string hex)
        {
            int red, green, blue;
            if (hex == null) { hex = "000000"; }
            TryParseHex(hex, out red, out green, out blue);
            currentColor = new Color(red / 255f, green / 255f, blue / 255f);
            hexColor = hex;
            image.color = currentColor;
            input.text = hexColor;
        }

        public Color CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;
                UpdateUI();
            }
        }

        public string ToHex
        {
            get => hexColor;
        }
    }
}