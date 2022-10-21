using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.JumpersMenu
{
    public class JumpersListItem : ListItemBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text countryFlagText;
        public TMP_Text birthdateText;
        public ToggleExtension toggleExtension;
        public Image countryFlagImage;
        public Image genderIconImage;
        public Image jumperImage;

        public Button playButton;
        public Button removeButton;
        public UnityEventInt onRemoveClicked;
        public UnityEventInt onPlayClicked;

        public void HandleRemoveButton() => onRemoveClicked.Invoke(Index);
        public void HandlePlayButton() => onPlayClicked.Invoke(Index);

        private new void Awake()
        {
            base.Awake();
            playButton.onClick.AddListener(HandlePlayButton);
            removeButton.onClick.AddListener(HandleRemoveButton);
        }
    }
}