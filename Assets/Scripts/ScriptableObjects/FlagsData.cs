using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/FlagsData")]
public class FlagsData : ScriptableObject
{
    [SerializeField]
    public CompCal.CountryData data;
    private Dictionary<string, int> countriesDict;
    [SerializeField]
    private SpriteAtlas flagsSpriteAtlas;

    private void OnEnable()
    {
        Debug.Log("ENABLED FLAGS MANAGER");
        LoadCountriesData();
    }

    public void LoadCountriesData()
    {
        this.countriesDict = new Dictionary<string, int>();

        for (int i = 0; i < this.data.spritesList.Count; i++)
        {
            this.countriesDict.Add(this.data.spritesList[i], i);
        }

        foreach (var c in this.data.countryList)
        {
            if (this.countriesDict.ContainsKey(c.alpha2))
            {
                this.countriesDict.Add(c.ioc, this.countriesDict[c.alpha2]);
            }
        }
    }

    public Sprite GetFlag(string countryCode)
    {
        string flagSprite = this.countriesDict.ContainsKey(countryCode) ? this.countriesDict[countryCode].ToString() : "0";
        return this.flagsSpriteAtlas.GetSprite("flags_responsive_uncompressed_" + flagSprite);
    }
}