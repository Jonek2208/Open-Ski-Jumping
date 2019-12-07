using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class LabelController : MonoBehaviour
{
    private TMPro.TMP_Text text;
    public StringVariable label;

    private void OnEnable()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    private void OnDisable()
    {
        text = null;
    }
    public void ReplaceText()
    {
        text.text = label.Value;
    }
}