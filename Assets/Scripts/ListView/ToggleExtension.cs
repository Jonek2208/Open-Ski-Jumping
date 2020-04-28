using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.ListView
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleExtension : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private ToggleGroupExtension toggleGroupExtension;
        [SerializeField] private int elementId;

        public int ElementId
        {
            get => elementId;
            set => elementId = value;
        }

        public Toggle Toggle
        {
            get => toggle;
            set => toggle = value;
        }

        public ToggleGroupExtension ToggleGroupExtension
        {
            get => toggleGroupExtension;
            set => toggleGroupExtension = value;
        }

        private void OnEnable()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        public void OnValueChanged(bool value)
        {
            toggleGroupExtension.HandleSelectionChanged(elementId, value);
        }

        public void SetElementId(int newId)
        {
            bool isSelected = elementId == toggleGroupExtension.CurrentValue;
            elementId = newId;
            if (toggleGroupExtension.AllowMultipleSelection) return;

            if (isSelected) toggleGroupExtension.ToggleGroup.allowSwitchOff = true;
            
            if (toggleGroupExtension.GetElementValue(newId))
            {
                toggle.isOn = true;
                toggleGroupExtension.ToggleGroup.allowSwitchOff = false;
            }
            else
            {
                toggle.isOn = false;
            }
        }
    }
}