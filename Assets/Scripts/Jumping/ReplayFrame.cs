using UnityEngine;

[System.Serializable]
public class ReplayFrame
{
    private Transform[] bodyParts;
    public ReplayFrame(Transform[] value)
    {
        this.bodyParts = value;
    }
}