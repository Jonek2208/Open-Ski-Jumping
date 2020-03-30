using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavesListItem : MonoBehaviour, IListViewElement<string>
{
    public ToggleExtension toggleExtension;
    [SerializeField] private TMP_Text valueText;
    public void UpdateContent(int index, string val)
    {
        this.valueText.text = val;
        this.toggleExtension.SetElementId(index);
    }
}
