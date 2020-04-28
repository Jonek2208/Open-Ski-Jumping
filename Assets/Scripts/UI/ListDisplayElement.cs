using UnityEngine;

namespace OpenSkiJumping.UI
{
    public class ListDisplayElement : MonoBehaviour
    {
        public ListDisplay listDisplay;
        public int index;
        public void OnValueChanged(bool flag)
        {
            if (flag)
            {
                listDisplay.OnListElementClick(index);
            }
        }
    }
}
