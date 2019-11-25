using UnityEngine;

[CreateAssetMenu]
public class GameTranslationElement : ScriptableObject
{
    [SerializeField]
    private Label label;
    [SerializeField]
    private string text;
}