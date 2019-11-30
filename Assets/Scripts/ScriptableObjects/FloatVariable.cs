using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject
{
    [SerializeField]
    private float value;
    [SerializeField]
    private GameEvent gameEvent;
    public float Value
    {
        get => value;
        set
        {
            this.value = value;
            gameEvent.Raise();
        }
    }
}