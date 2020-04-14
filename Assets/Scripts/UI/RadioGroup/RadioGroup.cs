using UnityEngine;

namespace UI.RadioGroup
{
    public class RadioGroup : MonoBehaviour
    {
        [SerializeField] private int value;

        public int Value
        {
            get => value;
            set => this.value = value; //TODO
        }

        public void HandleSelectionChanged(int index)
        {
            value = index;
        }
    }
}