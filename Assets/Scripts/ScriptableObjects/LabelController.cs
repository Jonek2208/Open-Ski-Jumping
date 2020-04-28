using TMPro;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
{
    public class LabelController : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] texts;
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
}