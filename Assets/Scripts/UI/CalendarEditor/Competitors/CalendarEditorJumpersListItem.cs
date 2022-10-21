using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.CalendarEditor.Competitors
{
    public class CalendarEditorJumpersListItem : ListItemBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text countryFlagText;
        public ToggleExtension toggleExtension;
        public Image countryFlagImage;
        public Image genderIconImage;
        public Image jumperImage;
    }
}