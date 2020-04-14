using System;
using ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.JumpersMenu
{
    public class JumpersListItem : MonoBehaviour
    {
        private string jumperId;
        public ToggleExtension toggleExtension;
        public TMP_Text nameText;
        public TMP_Text countryFlagText;
        public Image countryFlagImage;
        public Image genderIconImage;
    }
}