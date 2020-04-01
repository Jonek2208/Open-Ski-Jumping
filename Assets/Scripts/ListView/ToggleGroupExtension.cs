using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ToggleGroup))]
public class ToggleGroupExtension : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private int currentId;
    public int CurrentValue { get => currentId; private set => currentId = value; }
    public ToggleGroup ToggleGroup { get => toggleGroup; set => toggleGroup = value; }

    public void SetCurrentId(int elementId, bool sendCallback = true)
    {
        toggleGroup.allowSwitchOff = true;
        bool tmp = (CurrentValue == elementId);
        CurrentValue = elementId;
        if (tmp && sendCallback) { onValueChanged?.Invoke(CurrentValue); }
    }

    public bool GetElementValue(int elementId)
    {
        return elementId == CurrentValue;
    }

    public event Action<int> onValueChanged;
}