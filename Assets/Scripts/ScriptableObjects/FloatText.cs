using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class FloatText : MonoBehaviour
{
    private TMPro.TMP_Text text;
    public string variableName;
    public FloatVariable value;
    private void OnEnable()
    {
        text = GetComponent<TMPro.TMP_Text>();
        SetValue();
    }
    public void SetValue()
    {
        text.text = variableName + ": " + this.value.Value.ToString("#0.00");
    }
}