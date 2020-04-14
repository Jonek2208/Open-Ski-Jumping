using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "ScriptableObjects/JudgesMarkUIData")]
    public class JudgesMarkUIData : ScriptableObject
    {
        public Color countedTextColor;
        public Color countedBackgroundColor;
        public Color notCountedTextColor;
        public Color notCountedBackgroundColor;
    }
}

