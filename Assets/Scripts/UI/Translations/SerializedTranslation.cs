using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace OpenSkiJumping.UI.Translations
{
    [Serializable]
    public class Translation
    {
        [XmlAttribute("k")] public string key;
        [XmlAttribute("v")] public string value;
    }

    [XmlRoot("translation")][Serializable]
    public class SerializedTranslation
    {
        [XmlAttribute("lang")] public string lang;
        [XmlElement("phrase")] public List<Translation> translations = new();
        public Dictionary<string, string> GetTranslationsDict() =>
            translations.ToDictionary(it => it.key, it => it.value);
    }
}