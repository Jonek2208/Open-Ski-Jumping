using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class LabelController : MonoBehaviour
{
    private TMPro.TMP_Text text;
    public StringVariable label;

    public void ReplaceText()
    {
        if (text == null)
        {
            text = GetComponent<TMPro.TMP_Text>();
        }
        
        text.text = label.Value;
    }
}