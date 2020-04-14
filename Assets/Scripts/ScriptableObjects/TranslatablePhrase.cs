using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Translations/TranslatablePhrase")]
    public class TranslatablePhrase : ScriptableObject
    {
        [Serializable]
        public class PairStringTranslation
        {
            [SerializeField] private TranslationLanguage language;
            [SerializeField] private string value;

            public TranslationLanguage Language { get => language; set => language = value; }
            public string Value { get => value; set => this.value = value; }
        }

        [SerializeField] private string currentValue;
        [SerializeField] private List<PairStringTranslation> translations;
        [SerializeField] private TranslationsSet translationsSet;
        private readonly Dictionary<string, int> dict = new Dictionary<string, int>();

        public string CurrentValue { get => currentValue; set => currentValue = value; }

        private void OnEnable()
        {
            translationsSet.AddPhrase(this);
            for (int i = 0; i < translations.Count; i++)
            {
                dict.Add(translations[i].Language.LanguageId, i);
            }
        }

        public void SetLanguage(string languageId)
        {
            if (dict.ContainsKey(languageId))
            {
                CurrentValue = translations[dict[languageId]].Value;
            }
            else
            {
                CurrentValue = "NO TRANSLATION";
            }
        }
    }
}