using UnityEngine;

[CreateAssetMenu]
public class Label : ScriptableObject
{
    [SerializeField]
    private string value = "";
    public string Value
    {
        get => value;
        set
        {
            this.value = value;
        } 
    }
}