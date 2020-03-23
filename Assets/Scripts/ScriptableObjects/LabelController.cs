using UnityEngine;

public class LabelController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text[] texts;
    [SerializeField] private TranslatablePhrase label;

    private void OnEnable()
    {
        ReplaceText();
    }
    public void ReplaceText()
    {
        foreach (var item in texts)
        {
            item.text = label.CurrentValue;
        }
    }
}