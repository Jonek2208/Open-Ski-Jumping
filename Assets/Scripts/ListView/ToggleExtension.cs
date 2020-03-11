using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Toggle))]
public class ToggleExtension : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private ToggleGroupExtension toggleGroupExtension;
    [SerializeField] private int elementId;
    public int ElementId { get => elementId; set => elementId = value; }
    public Toggle Toggle { get => toggle; set => toggle = value; }
    public ToggleGroupExtension ToggleGroupExtension { get => toggleGroupExtension; set => toggleGroupExtension = value; }

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
        if (value)
        {
            toggleGroupExtension.SetCurrentId(elementId);
            toggleGroupExtension.ToggleGroup.allowSwitchOff = false;
        }
    }

    public void SetElementId(int newId)
    {
        if (elementId == toggleGroupExtension.CurrentValue)
        {
            toggleGroupExtension.ToggleGroup.allowSwitchOff = true;
        }

        elementId = newId;
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
