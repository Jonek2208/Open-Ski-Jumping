using System.Globalization;
using System.Linq;
using OpenSkiJumping.Data;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.UI
{
    public class OptionsMenuController : MonoBehaviour
    {
        [SerializeField] private GameConfigRuntime gameConfig;

        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Dropdown languageDropdown;


        private void Start()
        {
            inputField.SetTextWithoutNotify(gameConfig.Config.mouseSensitivity.ToString(CultureInfo.InvariantCulture));
            inputField.onValueChanged.AddListener(UpdateSensitivity);
            languageDropdown.AddOptions(gameConfig.Translations.Languages.Select(item => item.NativeLanguageName).ToList());
            languageDropdown.SetValueWithoutNotify((int) gameConfig.Config.currentLanguage);
            languageDropdown.onValueChanged.AddListener(UpdateLanguage);
        }

        private void UpdateSensitivity(string val)
        {
            gameConfig.Config.mouseSensitivity = float.Parse(val);
        }

        private void UpdateLanguage(int val)
        {
            gameConfig.SetLanguage((GameConfig.Language) val);
        }
    }
}