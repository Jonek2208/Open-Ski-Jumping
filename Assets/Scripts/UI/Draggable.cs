using UnityEngine;
using DG.Tweening;
// using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    public RectTransform rectTransform, rectTransform2;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rectTransform.localScale = new Vector3(1, 0, 1);
            rectTransform2.localScale = new Vector3(0, 1, 1);
            DOTween.Sequence().Append(rectTransform2.DOScaleX(1, 1)).Append(rectTransform.DOScaleY(1, 1));
        }
    }
}