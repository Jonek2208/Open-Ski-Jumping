using UnityEngine;


public class SkiJumperDataController : MonoBehaviour
{
    public RuntimeResultsManager resultsManager;
    public RuntimeParticipantsList participants;
    public RuntimeCompetitorsList competitors;
    public JumperController2 jumperController;
    public GameObject jumperMale;
    public GameObject jumperFemale;
    public CompCal.Competitor competitor;
    public Material helmetMaterial;
    public Material suitTopFrontMaterial;
    public Material suitTopBackMaterial;
    public Material suitBottomFrontMaterial;
    public Material suitBottomBackMaterial;
    public Material skisMaterial;

    public void GetValues()
    {
        int competitorId = resultsManager.currentStartList[resultsManager.startListIndex];
        competitor = competitors.competitors[participants.participants[competitorId].competitors[resultsManager.subroundIndex]];
    }

    public void SetValues()
    {
        jumperMale.SetActive(competitor.gender == CompCal.Gender.Male);
        jumperFemale.SetActive(competitor.gender == CompCal.Gender.Female);
        jumperController.modelObject = (competitor.gender == CompCal.Gender.Male ? jumperMale : jumperFemale);
        // helmetMaterial.SetColor("_Color", competitor.helmetColor);
        // suitTopFrontMaterial.SetColor("_Color", skiJumperData.suitTopFrontColor);
        // suitTopBackMaterial.SetColor("_Color", skiJumperData.suitTopBackColor);
        // suitBottomFrontMaterial.SetColor("_Color", skiJumperData.suitBottomFrontColor);
        // suitBottomBackMaterial.SetColor("_Color", skiJumperData.suitBottomBackColor);
        // skisMaterial.SetColor("_Color", skiJumperData.skisColor);
    }
}