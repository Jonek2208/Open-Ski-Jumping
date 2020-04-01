using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListItem : MonoBehaviour
{
    public ToggleExtension toggleExtension;
    public void UpdateContent(int index, IList<ResultData> val)
    {
        GetComponentInChildren<TMP_Text>().text = $"{val[index].result:F1} {val[index].firstName} {val[index].lastName}";
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
