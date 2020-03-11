using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListItem : MonoBehaviour, IListViewElement<ResultData>
{
    public ToggleExtension toggleExtension;
    public void UpdateContent(int index, ResultData val)
    {
        GetComponentInChildren<TMP_Text>().text = $"{val.result:F1} {val.firstName} {val.lastName}";
        toggleExtension.SetElementId(index);
    }
}

[System.Serializable]
public class ResultData
{
    public float result;
    public string firstName;
    public string lastName;
}
