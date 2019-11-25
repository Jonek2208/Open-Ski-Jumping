using UnityEngine;
public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameTranslation translation;
    private void OnEnable()
    {
        translation.SetLabels();
    }
}