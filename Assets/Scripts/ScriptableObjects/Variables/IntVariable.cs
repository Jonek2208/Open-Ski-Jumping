using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Variables/IntVariable")]
public class IntVariable : ScriptableObject
{
    [SerializeField]
    private int value;
    public int Value
    {
        get => value;
        set => this.value = value;
    }
}