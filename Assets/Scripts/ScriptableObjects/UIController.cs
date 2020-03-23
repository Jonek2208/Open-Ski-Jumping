using System.Linq;
using UnityEngine;
public class UIController : MonoBehaviour
{
    [SerializeField] private TranslationsSet translations;
    [SerializeField] private int currentTranslation;
    [SerializeField] private TMPro.TMP_Dropdown languagesDropdown;

    private void Start()
    {
        Refresh();
        languagesDropdown.AddOptions(translations.Languages.Select(item => item.NativeLanguageName).ToList());
    }
    public void Refresh()
    {
        translations.SetLanguage(currentTranslation);
    }

    public void SetCurrentTranslation(int index)
    {
        currentTranslation = index;
        Refresh();
    }
}