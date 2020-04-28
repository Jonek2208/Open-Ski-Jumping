using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OpenSkiJumping.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Translations/TranslationsSet")]

    public class TranslationsSet : ScriptableObject
    {
        [SerializeField] private List<TranslationLanguage> languages;
        [SerializeField] private List<TranslatablePhrase> phrases;
        public List<TranslationLanguage> Languages { get => languages; set => languages = value; }
        public UnityEvent onSetLanguage;
        public bool SetLanguage(int id)
        {
            if (id < 0 || languages.Count <= id) { return false; }
            foreach (var item in phrases)
            {
                item.SetLanguage(languages[id].LanguageId);
            }
            onSetLanguage.Invoke();
            return true;
        }

        public void AddPhrase(TranslatablePhrase phrase)
        {
            if (!phrases.Contains(phrase)) { phrases.Add(phrase); }
        }
    }
}