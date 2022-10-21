using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI.Translations;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [Serializable]
    public class GameConfig
    {
        public enum Language
        {
            English,
            Polish,
            Slovenian
        }

        public float mouseSensitivity;

        [JsonConverter(typeof(StringEnumConverter))]
        public Language currentLanguage;
    }


    [CreateAssetMenu(menuName = "ScriptableObjects/Data/GameConfig")]
    public class GameConfigRuntime : DatabaseObject<GameConfig>
    {
        [SerializeField] private TranslationsSet translationsSet;

        public TranslationsSet Translations => translationsSet;

        public void SetLanguage(GameConfig.Language language)
        {
            Config.currentLanguage = language;
            SetTranslations();
        }

        public GameConfig Config
        {
            get => Data;
            set => Data = value;
        }

        public void SetTranslations()
        {
            translationsSet.SetLanguage((int) Config.currentLanguage);
        }
        
        public override bool LoadData()
        {
            var tmp = base.LoadData();
            SetTranslations();
            return tmp;
        }
    }
}