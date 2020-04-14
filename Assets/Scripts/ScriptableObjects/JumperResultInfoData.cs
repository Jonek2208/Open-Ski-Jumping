using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/JumperResultInfoData")]
    public class JumperResultInfoData : ScriptableObject
    {
        public Color fontColor;
        public Color backgroundColor;
        public CompetitorVariable competitor;
        public bool showNameInfo;
        public bool showCountryInfo;
        public bool showResultInfo;
        public bool showRankInfo;
        public void SetRankInfo(bool value) { showRankInfo = value; }
        public void SetResultInfo(bool value) { showResultInfo = value; }
        public void SetUIForRound(int roundIt)
        {
            if (roundIt > 0)
            {
                showRankInfo = true;
                showResultInfo = true;
            }
            else
            {
                showRankInfo = false;
                showResultInfo = false;
            }
        }
    }
}