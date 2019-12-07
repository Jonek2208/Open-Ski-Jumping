using UnityEngine;

[CreateAssetMenu]
public class GameTranslationElement : ScriptableObject
{
    [SerializeField]
    private StringVariable label;
    [SerializeField]
    private string text;
}