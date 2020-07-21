using OpenSkiJumping.Competition;
using UnityEngine;
using EventType = OpenSkiJumping.Competition.EventType;

namespace OpenSkiJumping.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/IconsData")]
    public class IconsData : ScriptableObject
    {
        [SerializeField] private Sprite[] genderIcons;
        [SerializeField] private Sprite[] classificationTypeIcons;
        [SerializeField] private Sprite[] eventTypeIcons;
        [SerializeField] private Sprite[] bibSprites;
        

        public Sprite GetGenderIcon(Gender arg)
        {
            return genderIcons[(int) arg];
        }

        public Sprite GetClassificationTypeIcon(ClassificationType arg)
        {
            return classificationTypeIcons[(int) arg];
        }

        public Sprite GetEventTypeIcon(EventType arg)
        {
            return eventTypeIcons[(int) arg];
        }

        public Sprite GetBibIcon(int arg)
        {
            return bibSprites[arg];
        }
    }
}