using UnityEngine;


public class SkiJumperDataController : MonoBehaviour
{
    public JumperController2 jumperController;
    public GameObject jumperMale;
    public GameObject jumperFemale;
    public SkiJumperData skiJumperData;
    public Material helmetMaterial;
    public Material suitTopFrontMaterial;
    public Material suitTopBackMaterial;
    public Material suitBottomFrontMaterial;
    public Material suitBottomBackMaterial;
    public Material skisMaterial;

    public void SetValues()
    {
        jumperController.modelObject = (skiJumperData.gender == CompCal.Gender.Male ? jumperMale : jumperFemale);
        helmetMaterial.SetColor("_Color", skiJumperData.helmetColor);
        suitTopFrontMaterial.SetColor("_Color", skiJumperData.suitTopFrontColor);
        suitTopBackMaterial.SetColor("_Color", skiJumperData.suitTopBackColor);
        suitBottomFrontMaterial.SetColor("_Color", skiJumperData.suitBottomFrontColor);
        suitBottomBackMaterial.SetColor("_Color", skiJumperData.suitBottomBackColor);
        skisMaterial.SetColor("_Color", skiJumperData.skisColor);
    }
}