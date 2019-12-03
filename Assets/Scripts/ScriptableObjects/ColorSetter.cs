using UnityEngine;
using System.Globalization;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ColorSetter : MonoBehaviour
{
    private Image image;
    private Color color;

       private void Start()
    {
        image = GetComponent<Image>();
    }
}