using OpenSkiJumping.Competition;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.Jumping;
using OpenSkiJumping.New;
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
        public Material skisMaterial;
        public Material suitBottomBackMaterial;
        public Material suitBottomFrontMaterial;
        public Material suitTopBackMaterial;
        public Material suitTopFrontMaterial;

        public void GetValues()
        {
            var id = resultsManager.Value.GetCurrentJumperId();
            competitor = competitors.competitors[id];
        }

        public void SetValues()
        {
            jumperMale.gameObject.SetActive(competitor.gender == Gender.Male);
            jumperFemale.gameObject.SetActive(competitor.gender == Gender.Female);
            jumperController.jumperModel = (competitor.gender == Gender.Male ? jumperMale : jumperFemale);
            // helmetMaterial.SetColor("_Color", competitor.helmetColor);
            // suitTopFrontMaterial.SetColor("_Color", skiJumperData.suitTopFrontColor);
            // suitTopBackMaterial.SetColor("_Color", skiJumperData.suitTopBackColor);
            // suitBottomFrontMaterial.SetColor("_Color", skiJumperData.suitBottomFrontColor);
            // suitBottomBackMaterial.SetColor("_Color", skiJumperData.suitBottomBackColor);
            // skisMaterial.SetColor("_Color", skiJumperData.skisColor);
        }
    }
}