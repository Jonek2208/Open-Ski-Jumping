using ScriptableObjects.Variables;
using TMPro;
using UnityEngine;

namespace ScriptableObjects
{
    [RequireComponent(typeof(TMP_Text))]
    public class FloatText : MonoBehaviour
    {
        private TMP_Text text;
        public string variableName;
        public FloatVariable value;
        private void OnEnable()
        {
            text = GetComponent<TMP_Text>();
            SetValue();
        }
        public void SetValue()
        {
            text.text = variableName + ": " + value.Value.ToString("#0.00");
        }

        public void Reset()
        {

            text.text = variableName + ": ";

        }
    }
}