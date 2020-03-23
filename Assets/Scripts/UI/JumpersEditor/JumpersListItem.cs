using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JumpersListItem : MonoBehaviour, IListViewElement<JumpersListElementData>
{
    private string jumperId;
    public ToggleExtension toggleExtension;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countryFlagText;
    [SerializeField] private Image countryFlagImage;
    [SerializeField] private Image genderIconImage;

    public void UpdateContent(int index, JumpersListElementData val)
    {
        this.jumperId = val.id;
        this.nameText.text = $"{val.firstName} {val.lastName.ToUpper()}";
        this.countryFlagText.text = val.countryCode;
        this.countryFlagImage.sprite = val.flagSprite;
        this.genderIconImage.sprite = val.genderIcon;
        this.toggleExtension.SetElementId(index);
    }

    public UnityEventString onClickedEvent;

    public void OnClicked(bool value)
    {
        if (value)
        { this.onClickedEvent.Invoke(jumperId); }
    }
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
