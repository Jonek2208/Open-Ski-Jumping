using OpenSkiJumping.UI.ListView;
using TMPro;
using UnityEngine.UI;

namespace OpenSkiJumping.UI.ItemsSearch
{
    public class ItemsSearchListItem : ListItemBehaviour
    {
        public TMP_Text valueText;
        public Button selectButton;
        public UnityEventInt onSelected;
        public void HandlePlayButton() => onSelected.Invoke(Index);

        private new void Awake()
        {
            base.Awake();
            selectButton.onClick.AddListener(HandlePlayButton);
        }
    }
}