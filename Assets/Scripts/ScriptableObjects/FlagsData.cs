using System.Collections.Generic;
using Competition;
using UnityEngine;
using UnityEngine.U2D;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/FlagsData")]
    public class FlagsData : ScriptableObject
    {
        [SerializeField]
        public CountryData data;
        private Dictionary<string, string> countriesDict;
        [SerializeField]
        private SpriteAtlas flagsSpriteAtlas;

        private void OnEnable()
        {
            Debug.Log("ENABLED FLAGS MANAGER");
            LoadCountriesData();
        }

        public void LoadCountriesData()
        {
            countriesDict = new Dictionary<string, string>();

            foreach (var c in data.countryList)
            {
                countriesDict.Add(c.ioc, c.alpha2);
            }
        }

        public Sprite GetFlag(string countryCode)
        {
            string flagSprite = countriesDict.ContainsKey(countryCode) ? countriesDict[countryCode] : "ioc";
            return flagsSpriteAtlas.GetSprite(flagSprite);
        }
    }
}