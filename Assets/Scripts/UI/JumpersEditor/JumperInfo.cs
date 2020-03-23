using System;
using System.Collections;
using System.IO;
using CompCal;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JumperInfo : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private TMP_InputField firstNameInput;
    [SerializeField] private TMP_InputField lastNameInput;
    [SerializeField] private TMP_InputField countryCodeInput;
    [SerializeField] private TMP_Dropdown genderDropdown;
    [SerializeField] private SimpleColorPicker helmetColorPicker;
    [SerializeField] private SimpleColorPicker suitTopFrontColorPicker;
    [SerializeField] private SimpleColorPicker suitTopBackColorPicker;
    [SerializeField] private SimpleColorPicker suitBottomFrontColorPicker;
    [SerializeField] private SimpleColorPicker suitBottomBackColorPicker;
    [SerializeField] private SimpleColorPicker skisColorPicker;
    [SerializeField] private TMP_InputField imagePathInput;
    [SerializeField] private RawImage image;
    [SerializeField] private Competitor boundCompetitor;
    [SerializeField] private GameEventListener gameEventListener;
    public void UpdateBoundCompetitor()
    {
        if (boundCompetitor == null) return;
        boundCompetitor.firstName = firstNameInput.text;
        boundCompetitor.lastName = lastNameInput.text;
        boundCompetitor.countryCode = countryCodeInput.text;
        boundCompetitor.gender = (CompCal.Gender)(genderDropdown.value);
        boundCompetitor.helmetColor = helmetColorPicker.ToHex;
        boundCompetitor.suitBottomBackColor = suitBottomBackColorPicker.ToHex;
        boundCompetitor.suitBottomFrontColor = suitBottomFrontColorPicker.ToHex;
        boundCompetitor.suitTopBackColor = suitTopBackColorPicker.ToHex;
        boundCompetitor.suitTopFrontColor = suitTopFrontColorPicker.ToHex;
        boundCompetitor.skisColor = skisColorPicker.ToHex;
        bool tmp = (boundCompetitor.imagePath != imagePathInput.text);
        boundCompetitor.imagePath = imagePathInput.text;
        if (tmp) { StartCoroutine(LoadImage()); }
    }

    private void RefreshShownValue()
    {
        if (boundCompetitor == null)
        {
            content.SetActive(false);
            return;
        }
        content.SetActive(true);
        
        firstNameInput.text = boundCompetitor.firstName;
        lastNameInput.text = boundCompetitor.lastName;
        countryCodeInput.text = boundCompetitor.countryCode;
        genderDropdown.value = (int)boundCompetitor.gender;
        helmetColorPicker.Set(boundCompetitor.helmetColor);
        suitBottomBackColorPicker.Set(boundCompetitor.suitBottomBackColor);
        suitBottomFrontColorPicker.Set(boundCompetitor.suitBottomFrontColor);
        suitTopBackColorPicker.Set(boundCompetitor.suitTopBackColor);
        suitTopFrontColorPicker.Set(boundCompetitor.suitTopFrontColor);
        skisColorPicker.Set(boundCompetitor.skisColor);
        imagePathInput.text = boundCompetitor.imagePath;
        StartCoroutine(LoadImage());
    }

    private string GetImageUri(string fileName)
    {
        var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, "images", fileName));
        return uri.AbsoluteUri;
    }

    public void Bind(CompCal.Competitor competitor)
    {
        this.boundCompetitor = competitor;
        gameEventListener.enabled = false;
        RefreshShownValue();
        gameEventListener.enabled = true;
    }

    public Competitor GetCompetitorValue()
    {
        return this.boundCompetitor;
    }

    IEnumerator LoadImage()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(GetImageUri(boundCompetitor.imagePath));
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            image.enabled = false;
        }
        else
        {
            Debug.Log("Image succesfully loaded");
            image.enabled = true;
            image.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }

}