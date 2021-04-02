using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenSkiJumping.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace OpenSkiJumping.UI
{
    public class OptionsMenuController : MonoBehaviour
    {
        [SerializeField] private GameConfigRuntime gameConfig;

        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Dropdown languageDropdown;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private SegmentedControl fullScreenSelect;

        private List<Resolution> _resolutions;

        private void Start()
        {
            inputField.SetTextWithoutNotify(gameConfig.Config.mouseSensitivity.ToString(CultureInfo.InvariantCulture));
            inputField.onValueChanged.AddListener(UpdateSensitivity);

            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(gameConfig.Translations.Languages.Select(item => item.NativeLanguageName)
                .ToList());
            languageDropdown.SetValueWithoutNotify((int) gameConfig.Config.currentLanguage);
            languageDropdown.onValueChanged.AddListener(UpdateLanguage);

            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(QualitySettings.names.ToList());
            // Debug.Log(QualitySettings.names[QualitySettings.GetQualityLevel()]);
            
            qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
            qualityDropdown.onValueChanged.AddListener(UpdateQuality);
            
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(Screen.resolutions.Select(it => $"{it.width} x {it.height}").ToList());
            _resolutions = Screen.resolutions.ToList();
            var resolutionIndex = _resolutions.IndexOf(Screen.currentResolution);
            resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
            resolutionDropdown.onValueChanged.AddListener(UpdateResolution);

            fullScreenSelect.SetSelectedSegmentWithoutNotify(Screen.fullScreen == false ? 0 : 1);
            fullScreenSelect.onValueChanged.AddListener(UpdateFullScreen);
        }

        private void UpdateQuality(int arg)
        {
            QualitySettings.SetQualityLevel(arg);
        }

        private void UpdateResolution(int arg)
        {
            Screen.SetResolution(_resolutions[arg].width, _resolutions[arg].height, Screen.fullScreen);
        }

        private void UpdateFullScreen(int arg)
        {
            Screen.fullScreen = arg != 0;
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