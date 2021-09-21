using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.Jumping;
using OpenSkiJumping.New;
using OpenSkiJumping.UI;
using UnityEngine;

namespace OpenSkiJumping.ScriptableObjects
{
    public class SkiJumperDataController : MonoBehaviour
    {
        public Competitor competitor;
        public RuntimeCompetitorsList competitors;
        public Material helmetMaterial;
        public JumperController2 jumperController;
        public JumperModel jumperFemale;
        public JumperModel jumperMale;
        public RuntimeResultsManager resultsManager;
        public Material bibMaterial;
        public Material skisMaterial;
        public Material suitBottomBackMaterial;
        public Material suitBottomFrontMaterial;
        public Material suitTopBackMaterial;
        public Material suitTopFrontMaterial;
        public Material skinMaterial;
        public Material hairMaterial;
        private static readonly int Color = Shader.PropertyToID("_BaseColor");

        public void GetValues()
        {
            var id = resultsManager.Value.GetCurrentJumperId();
            competitor = competitors.competitors[id];
        }

        public void SetValues(Color bibColor)
        {
            jumperMale.gameObject.SetActive(competitor.gender == Gender.Male);
            jumperFemale.gameObject.SetActive(competitor.gender == Gender.Female);
            jumperController.jumperModel = (competitor.gender == Gender.Male ? jumperMale : jumperFemale);
            bibMaterial.SetColor(Color, bibColor);
            helmetMaterial.SetColor(Color, SimpleColorPicker.Hex2Color(competitor.helmetColor));
            suitTopFrontMaterial.SetColor(Color, SimpleColorPicker.Hex2Color(competitor.suitTopFrontColor));
            suitTopBackMaterial.SetColor(Color, SimpleColorPicker.Hex2Color(competitor.suitTopBackColor));
            suitBottomFrontMaterial.SetColor(Color, SimpleColorPicker.Hex2Color(competitor.suitBottomFrontColor));
            suitBottomBackMaterial.SetColor(Color, SimpleColorPicker.Hex2Color(competitor.suitBottomBackColor));
            skisMaterial.SetColor(Color, SimpleColorPicker.Hex2Color(competitor.skisColor));
            skinMaterial.SetColor(Color, SimpleColorPicker.Hex2Color(competitor.skinColor));
        }
    }
}