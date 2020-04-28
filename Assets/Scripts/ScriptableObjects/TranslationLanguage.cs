using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Translations/TranslationLanguage")]

    public class TranslationLanguage : ScriptableObject
    {
        [SerializeField] private string nativeLanguageName;
        [SerializeField] private string languageId;

        public string NativeLanguageName { get => nativeLanguageName; set => nativeLanguageName = value; }
        public string LanguageId { get => languageId; set => languageId = value; }
    }
}