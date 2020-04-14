using ListView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CalendarEditor.Classifications
{
    public class ClassificationsListItem : MonoBehaviour
    {
        private string jumperId;
        public ToggleExtension toggleExtension;
        public TMP_Text nameText;
        public Image eventTypeImage;
        public Image classificationTypeImage;
    }
}