using System.Globalization;
using OpenSkiJumping.ScriptableObjects.Variables;
using UnityEngine;

namespace OpenSkiJumping.UI
{
    public class OptionsMenuController : MonoBehaviour
    {
        [SerializeField] private FloatVariable mouseSensitivityVariable;
        
        [SerializeField] private TMPro.TMP_InputField inputField;
        
        private void Start()
        {
            inputField.SetTextWithoutNotify(mouseSensitivityVariable.Value.ToString(CultureInfo.InvariantCulture));
        }

    }
}
