using System;
using UnityEngine;

[Serializable]
public class SerializableDecimal : ISerializationCallbackReceiver
{
    public decimal value;
    [SerializeField]
    private int[] data;

    public void OnBeforeSerialize()
    {
        data = decimal.GetBits(value);
    }
    public void OnAfterDeserialize()
    {
        if (data != null && data.Length == 4)
        {
            value = new decimal(data);
        }
    }
}