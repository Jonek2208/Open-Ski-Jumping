using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public class JumpersListItem : ListItemBehaviour
    {
        public ToggleExtension toggleExtension;
        public TMP_Text nameText;
        public TMP_Text countryFlagText;
        public Image countryFlagImage;
        public Image genderIconImage;
    }
}