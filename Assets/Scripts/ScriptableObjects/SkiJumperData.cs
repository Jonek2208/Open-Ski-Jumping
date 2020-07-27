using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
{
    [CreateAssetMenu]
    public class SkiJumperData : ScriptableObject
    {
        public Gender gender;
        public Color helmetColor;
        public Color suitTopFrontColor;
        public Color suitTopBackColor;
        public Color suitBottomFrontColor;
        public Color suitBottomBackColor;
        public Color skisColor;
        public Color bibColor;

        public void Set(Competitor competitorData, Color newBibColor)
        {
            gender = competitorData.gender;
            bibColor = newBibColor;
            ColorUtility.TryParseHtmlString(competitorData.helmetColor, out helmetColor);
            ColorUtility.TryParseHtmlString(competitorData.suitTopFrontColor, out suitTopFrontColor);
            ColorUtility.TryParseHtmlString(competitorData.suitTopBackColor, out suitTopBackColor);
            ColorUtility.TryParseHtmlString(competitorData.suitBottomFrontColor, out suitBottomFrontColor);
            ColorUtility.TryParseHtmlString(competitorData.suitBottomBackColor, out suitBottomBackColor);
            ColorUtility.TryParseHtmlString(competitorData.skisColor, out skisColor);
        }
    }
}