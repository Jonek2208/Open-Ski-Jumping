using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderController : MonoBehaviour
{
    private Slider slider;
    public FloatVariable value;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = value.Value;
    }
}