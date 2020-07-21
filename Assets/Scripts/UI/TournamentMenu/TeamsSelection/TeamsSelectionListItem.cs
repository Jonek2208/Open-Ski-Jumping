using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.TournamentMenu.TeamsSelection
{
    public class TeamsSelectionListItem : ListItemBehaviour
    {
        public ToggleExtension toggleExtension;
        public TMP_Text[] nameText;
        public TMP_Text countryFlagText;
        public Image countryFlagImage;
        public Button editButton;
    }
}