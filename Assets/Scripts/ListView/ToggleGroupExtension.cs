using System;
using UnityEngine;
using UnityEngine.UI;

namespace ListView
{
    [RequireComponent(typeof(ToggleGroup))]
    public class ToggleGroupExtension : MonoBehaviour
    {
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private int currentId = -1;
        [SerializeField] private bool allowMultipleSelection;


        public int CurrentValue
        {
            get => currentId;
            private set => currentId = value;
        }

        public ToggleGroup ToggleGroup
        {
            get => toggleGroup;
            set => toggleGroup = value;
        }

        public bool AllowMultipleSelection
        {
            get => allowMultipleSelection;
            set => allowMultipleSelection = value;
        }

        public void HandleSelectionChanged(int elementId, bool value = true, bool sendCallback = true)
        {
            toggleGroup.allowSwitchOff = true;
            if (value)
            {
                bool tmp = (CurrentValue == elementId);
                CurrentValue = elementId;
                if (!tmp && sendCallback)
                {
                    OnValueChanged?.Invoke(CurrentValue);
                }
            }
            
            if(sendCallback)
            {
                OnSelectionChanged?.Invoke(elementId, value);
            }
        }

        public bool GetElementValue(int elementId)
        {
            return elementId == CurrentValue;
        }

        public event Action<int> OnValueChanged;
        public event Action<int, bool> OnSelectionChanged;
    }
}