using ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptableObjects
{
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
}