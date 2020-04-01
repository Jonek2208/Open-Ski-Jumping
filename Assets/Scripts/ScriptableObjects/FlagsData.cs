using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/FlagsData")]
public class FlagsData : ScriptableObject
{
    [SerializeField]
    public Competition.CountryData data;
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

        foreach (var c in this.data.countryList)
        {
            countriesDict.Add(c.ioc, c.alpha2);
        }
    }

    public Sprite GetFlag(string countryCode)
    {
        string flagSprite = countriesDict.ContainsKey(countryCode) ? countriesDict[countryCode].ToString() : "ioc";
        return flagsSpriteAtlas.GetSprite(flagSprite);
    }
}