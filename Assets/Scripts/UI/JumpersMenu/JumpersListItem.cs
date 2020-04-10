using System.Collections;
using System.Collections.Generic;
using Competition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JumpersListItem : MonoBehaviour
{
    private string jumperId;
    public ToggleExtension toggleExtension;
    public TMP_Text nameText;
    public TMP_Text countryFlagText;
    public Image countryFlagImage;
    public Image genderIconImage;
}

[System.Serializable]
public class JumpersListElementData
{
    public string firstName;
    public string lastName;
    public string countryCode;
    public Sprite genderIcon;
    public Sprite flagSprite;
    public string id;
}
