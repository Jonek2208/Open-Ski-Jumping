using System.Linq;
using TMPro;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TranslationsSet translations;
        [SerializeField] private int currentTranslation;
        [SerializeField] private TMP_Dropdown languagesDropdown;

        private void Start()
        {
            Refresh();
        }
        public void Refresh()
        {
            translations.SetLanguage(currentTranslation);
        }

        public void SetCurrentTranslation(int index)
        {
            currentTranslation = index;
            Refresh();
        }
    }
}