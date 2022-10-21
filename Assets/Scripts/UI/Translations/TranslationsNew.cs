using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenSkiJumping.UI.Translations
{
    [Serializable]
    public class PhraseData
    {
        public string name;
        public TranslatablePhrase phrase;
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/TranslationsNew")]
    public class TranslationsNew : ScriptableObject
    {
        public List<SerializedTranslation> translations;
        public List<PhraseData> phrases;

        public void LoadTranslation(SerializedTranslation translation)
        {
            var dict = translation.GetTranslationsDict();
            foreach (var phraseData in phrases)
            {
                var key = phraseData.name;
                if (dict.ContainsKey(key))
                {
                    phraseData.phrase.CurrentValue = dict[key];
                }
            }
        }
    }
}