using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.RadioGroup
{
    public class RadioGroupElement : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private int index;
        [SerializeField] private RadioGroup radioGroup;

        private void OnSelectionChanged(bool value)
        {
            if (value)
            {
                radioGroup.HandleSelectionChanged(index);
            }
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(OnSelectionChanged);
        }
    }
}