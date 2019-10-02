using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using HillProfile;


public class ManagerScript : MonoBehaviour
{
    public bool muted;
    public bool predefinedHill;
    public TMPro.TMP_Dropdown dataDropdown;
    public TMPro.TMP_Dropdown typeDropdown;

    public TMPro.TMP_InputField nameInput;

    public TMPro.TMP_InputField wInput;
    public TMPro.TMP_InputField hnInput;
    public TMPro.TMP_InputField gammaInput;
    public TMPro.TMP_InputField alphaInput;
    public TMPro.TMP_InputField eInput;
    public TMPro.TMP_InputField esInput;
    public TMPro.TMP_InputField tInput;
    public TMPro.TMP_InputField r1Input;
    public TMPro.TMP_InputField betaPInput;
    public TMPro.TMP_InputField betaKInput;
    public TMPro.TMP_InputField betaLInput;
    public TMPro.TMP_InputField sInput;
    public TMPro.TMP_InputField l1Input;
    public TMPro.TMP_InputField l2Input;
    public TMPro.TMP_InputField rLInput;
    public TMPro.TMP_InputField r2LInput;
    public TMPro.TMP_InputField r2Input;

    //Construction
    public TMPro.TMP_InputField gatesInput;
    public Slider terrainSlider;
    public TMPro.TMP_Text terrainText;

    public TMPro.TMP_Text timeText;

    //Other
    public Slider gateSlider;
    public TMPro.TMP_Text gateText;
    public Slider windSlider;
    public TMPro.TMP_Text windText;
    public TMPro.TMP_Text distText;
    public TMPro.TMP_Text speedText;


    public MeshScript meshGenerator;
    public JudgesController judgesController;
    public JumperController2 jumperController;

    // public AudioSource audioSource;

    public AllData allData;
    public List<ProfileData> hills;
    private List<int> deletedHills;

    public Hill currentHill;
    Vector2[] landingAreaPoints;

    private string dataFileName = "data.json";


    void Start()
    {
        LoadData();

        if (hills.Count == 0)
        {
            hills.Add(GetProfileData(new Hill(Hill.ProfileType.ICR1992, 42, 120, 0.575f, 35, 11, 95.2f, 20.5f, 6.5f, 107, 37.5f, 37.5f, 37.5f, 3, 30, 11, 0, 0, 126f), "Hakuba HS131", 0.5f));
            hills.Add(GetProfileData(new Hill(Hill.ProfileType.ICR1996, 35, 120, 0.575f, 35, 11, 99, 23, 6.5f, 115, 37.43f, 35.5f, 32.4f, 3.38f, 11.15f, 17.42f, 321, 100, 100), "Oberstdorf HS137", 0.65f));
            hills.Add(GetProfileData(new Hill(Hill.ProfileType.ICR2008, 35, 125, 0.575f, 35, 11, 98.7f, 22, 6.5f, 90, 37.05f, 34.3f, 31.4f, 3.13f, 16, 15, 310, 168, 99.3f), "Zakopane HS140", 1f));
        }

        // meshGenerator = GetComponentInChildren<MeshScript>();
        int hillCode = 0;
        if (predefinedHill)
        {
            judgesController.HillInit();
            judgesController.jumpUIManager.SetGateSliderRange(currentHill.gates);
            judgesController.NewJump();
        }
        else
        {
            if (!muted)
            {
                dataDropdown.options = new List<TMPro.TMP_Dropdown.OptionData>();

                foreach (ProfileData it in hills)
                {
                    TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData();
                    option.text = it.name;
                    dataDropdown.options.Add(option);
                }
                dataDropdown.RefreshShownValue();
            }
            else
            {
                hillCode = PlayerPrefs.GetInt("HillCode");
                meshGenerator.inrunTerrain = PlayerPrefs.GetFloat("HillSteepness");
                Debug.Log(hillCode);
            }

            LoadDefaultHill(hillCode);
            GenerateButtonClick();
        }


        // audioSource.Stop();
    }

    public float Distance(Vector3 contact)
    {
        float dist = judgesController.Distance(landingAreaPoints, contact);
        distText.text = "Distance: " + dist;
        return dist;
    }

    public void Speed(float speed)
    {
        speedText.text = "Speed: " + speed * 3.4f;
        // audioSource.Play();
    }

    public void GenerateButtonClick()
    {
        if (!muted)
        {
            Hill hill = new Hill((Hill.ProfileType)(typeDropdown.value), int.Parse(gatesInput.text), float.Parse(wInput.text), float.Parse(hnInput.text), float.Parse(gammaInput.text), float.Parse(alphaInput.text),
                float.Parse(eInput.text), float.Parse(esInput.text), float.Parse(tInput.text), float.Parse(r1Input.text), float.Parse(betaPInput.text),
                float.Parse(betaKInput.text), float.Parse(betaLInput.text), float.Parse(sInput.text), float.Parse(l1Input.text), float.Parse(l2Input.text),
                float.Parse(rLInput.text), float.Parse(r2LInput.text), float.Parse(r2Input.text));
            currentHill = hill;
        }
        Debug.Log(currentHill.w);
        meshGenerator.profileData = GetProfileData(currentHill, nameInput.text, terrainSlider.value);
        Debug.Log(meshGenerator.profileData.w);
        meshGenerator.GenerateMesh(currentHill);
        currentHill.Calculate();
        landingAreaPoints = meshGenerator.landingAreaPoints;

        judgesController.HillInit();
        judgesController.jumpUIManager.SetGateSliderRange(currentHill.gates);
        judgesController.NewJump();
    }

    public void AddButtonClick()
    {
        Hill hill = new Hill((Hill.ProfileType)(typeDropdown.value), int.Parse(gatesInput.text), float.Parse(wInput.text), float.Parse(hnInput.text), float.Parse(gammaInput.text), float.Parse(alphaInput.text),
            float.Parse(eInput.text), float.Parse(esInput.text), float.Parse(tInput.text), float.Parse(r1Input.text), float.Parse(betaPInput.text),
            float.Parse(betaKInput.text), float.Parse(betaLInput.text), float.Parse(sInput.text), float.Parse(l1Input.text), float.Parse(l2Input.text),
            float.Parse(rLInput.text), float.Parse(r2LInput.text), float.Parse(r2Input.text));

        hills.Add(GetProfileData(hill, nameInput.text, terrainSlider.value));
        allData.profileData = hills;
        SaveData();
        TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData();
        option.text = nameInput.text;
        dataDropdown.options.Add(option);
        dataDropdown.value = dataDropdown.options.Count - 1;
        //MeshScript meshScript = GetComponentInChildren<MeshScript>();
        //meshScript.SaveMesh();
    }

    public void ChangeButtonClick()
    {
        Hill hill = new Hill((Hill.ProfileType)(typeDropdown.value), int.Parse(gatesInput.text), float.Parse(wInput.text), float.Parse(hnInput.text), float.Parse(gammaInput.text), float.Parse(alphaInput.text),
            float.Parse(eInput.text), float.Parse(esInput.text), float.Parse(tInput.text), float.Parse(r1Input.text), float.Parse(betaPInput.text),
            float.Parse(betaKInput.text), float.Parse(betaLInput.text), float.Parse(sInput.text), float.Parse(l1Input.text), float.Parse(l2Input.text),
            float.Parse(rLInput.text), float.Parse(r2LInput.text), float.Parse(r2Input.text));

        hills[dataDropdown.value] = GetProfileData(hill, nameInput.text, terrainSlider.value);
        allData.profileData = hills;
        SaveData();
        TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData();
        option.text = nameInput.text;
        dataDropdown.options[dataDropdown.value] = option;
        dataDropdown.RefreshShownValue();
    }

    public void DeleteButtonClick()
    {
        int val = dataDropdown.value;
        dataDropdown.value = 0;
        hills.RemoveAt(val);
        dataDropdown.options.RemoveAt(val);
        allData.profileData = hills;
        SaveData();
        // GameObject tmp = jumperButtons[selected];
        // deletedJumpers.Add(selected);
        // Destroy(tmp);
        // selected = -1;
        // state = State.init;
        // changeButton.interactable = false;
        // deleteButton.interactable = false;
    }

    public void TimeButtonClick()
    {
        if (meshGenerator.time == 0)
        {
            meshGenerator.time = 1;
            timeText.text = "Day";
        }
        else
        {
            meshGenerator.time = 0;
            timeText.text = "Night";
        }
    }

    // public void SliderChange()
    // {
    //     meshGenerator.SetGate(currentHill, (int)gateSlider.value);
    //     gateText.text = "Gate: " + (int)gateSlider.value;
    // }

    // public void WindSliderChange()
    // {
    //     jumperController.windForce = windSlider.value;
    //     windText.text = "Wind: " + windSlider.value;
    // }

    public void TerrainSliderChange()
    {
        meshGenerator.inrunTerrain = terrainSlider.value;
        terrainText.text = "Terrain steepness: " + terrainSlider.value;
    }

    public void LoadDefaultHill(int id)
    {

        // Hill[] tab = {
        // 	new Hill(Hill.ProfileType.ICR1996, 120, 0.575f, 35, 11, 99, 23, 6.5f, 115, 37.43f, 35.5f, 32.4f, 3.38f, 11.15f, 17.42f, 321, 100, 100),
        // 	new Hill(Hill.ProfileType.ICR2008, 125, 0.575f, 35, 11, 98.7f, 22, 6.5f, 90, 37.05f, 34.3f, 31.4f, 3.13f, 16, 15, 310, 168, 99.3f),
        // 	new Hill(Hill.ProfileType.ICR1992, 120, 0.575f, 35, 11, 95.2f, 20.5f, 6.5f, 107, 37.5f, 37.5f, 37.5f, 3, 30, 11, 0, 0, 126f)
        // };
        // Hill hill = tab[id];

        ProfileData tmp = hills[id];
        if (muted)
        {
            currentHill = new Hill(tmp);
            Debug.Log(tmp.w);
        }
        
        nameInput.text = tmp.name;
        typeDropdown.value = (int)(tmp.type);
        gatesInput.text = tmp.gates.ToString();
        wInput.text = tmp.w.ToString();
        hnInput.text = tmp.hn.ToString();
        gammaInput.text = tmp.gamma.ToString();
        alphaInput.text = tmp.alpha.ToString();
        eInput.text = tmp.e.ToString();
        esInput.text = tmp.es.ToString();
        tInput.text = tmp.t.ToString();
        r1Input.text = tmp.r1.ToString();
        betaPInput.text = tmp.betaP.ToString();
        betaKInput.text = tmp.betaK.ToString();
        betaLInput.text = tmp.betaL.ToString();
        sInput.text = tmp.s.ToString();
        l1Input.text = tmp.l1.ToString();
        l2Input.text = tmp.l2.ToString();
        rLInput.text = tmp.rL.ToString();
        r2LInput.text = tmp.r2L.ToString();
        r2Input.text = tmp.r2.ToString();
        terrainSlider.value = tmp.terrainSteepness;
    }

    public ProfileData GetProfileData(Hill hill, string name, float terrainSteepness)
    {
        ProfileData res = new ProfileData();
        res.name = name;
        res.terrainSteepness = terrainSteepness;
        res.type = hill.type;
        res.gates = hill.gates;
        res.w = hill.w;
        res.hn = hill.hn;
        res.gamma = hill.gamma;
        res.alpha = hill.alpha;
        res.e = hill.e;
        res.es = hill.es;
        res.t = hill.t;
        res.r1 = hill.r1;
        res.betaP = hill.betaP;
        res.betaK = hill.betaK;
        res.betaL = hill.betaL;
        res.s = hill.s;
        res.l1 = hill.l1;
        res.l2 = hill.l2;
        res.rL = hill.rL;
        res.r2L = hill.r2L;
        res.r2 = hill.r2;

        res.a = 100;
        res.rA = 0;
        res.betaA = 0;
        res.b1 = 2.5f;
        res.b2 = 10;
        res.bK = 20;
        res.bU = 25;

        return res;
    }

    private void LoadData()
    {
        /* WebGL */
        // if (PlayerPrefs.HasKey("Hills3"))
        // {
        //     string dataAsJson = PlayerPrefs.GetString("Hills3");
        //     AllData loadedData = JsonUtility.FromJson<AllData>(dataAsJson);
        //     hills = loadedData.profileData;
        // }
        /* Standalone */
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            AllData loadedData = JsonUtility.FromJson<AllData>(dataAsJson);
            hills = loadedData.profileData;
        }
        else
        {
            Debug.LogError("No data!");
            string dataAsJson = "{\"profileData\":[{\"name\":\"Hakuba HS131\",\"terrainSteepness\":0.5529257655143738,\"type\":0,\"gates\":42,\"w\":120.0,\"hn\":0.574999988079071,\"gamma\":35.0,\"alpha\":11.0,\"e\":95.19999694824219,\"es\":20.5,\"t\":6.5,\"r1\":107.0,\"betaP\":37.5,\"betaK\":37.5,\"betaL\":37.5,\"s\":3.0,\"l1\":30.0,\"l2\":11.0,\"rL\":0.0,\"r2L\":0.0,\"r2\":126.0},{\"name\":\"Hakuba HS98\",\"terrainSteepness\":0.0,\"type\":0,\"gates\":42,\"w\":90.0,\"hn\":0.5619999766349793,\"gamma\":36.0,\"alpha\":10.5,\"e\":78.52999877929688,\"es\":20.5,\"t\":5.800000190734863,\"r1\":85.0,\"betaP\":36.5,\"betaK\":36.5,\"betaL\":36.5,\"s\":2.440000057220459,\"l1\":20.0,\"l2\":8.0,\"rL\":0.0,\"r2L\":0.0,\"r2\":115.0},{\"name\":\"Oberstdorf HS137\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":35,\"w\":120.0,\"hn\":0.574999988079071,\"gamma\":35.0,\"alpha\":11.0,\"e\":99.0,\"es\":23.0,\"t\":6.5,\"r1\":115.0,\"betaP\":37.43000030517578,\"betaK\":35.5,\"betaL\":32.400001525878909,\"s\":3.380000114440918,\"l1\":11.149999618530274,\"l2\":17.420000076293947,\"rL\":321.0,\"r2L\":100.0,\"r2\":100.0},{\"name\":\"Oberstdorf HS106\",\"terrainSteepness\":0.0,\"type\":2,\"gates\":42,\"w\":95.0,\"hn\":0.5410000085830689,\"gamma\":35.0,\"alpha\":11.0,\"e\":89.0,\"es\":20.700000762939454,\"t\":6.5,\"r1\":95.0,\"betaP\":37.0,\"betaK\":34.5,\"betaL\":32.0,\"s\":2.200000047683716,\"l1\":10.0,\"l2\":11.0,\"rL\":220.0,\"r2L\":106.5,\"r2\":106.5999984741211},{\"name\":\"Zakopane HS140\",\"terrainSteepness\":1.0,\"type\":2,\"gates\":35,\"w\":125.0,\"hn\":0.574999988079071,\"gamma\":35.0,\"alpha\":11.0,\"e\":98.69999694824219,\"es\":22.0,\"t\":6.5,\"r1\":90.0,\"betaP\":37.04999923706055,\"betaK\":34.29999923706055,\"betaL\":31.399999618530275,\"s\":3.130000114440918,\"l1\":16.0,\"l2\":15.0,\"rL\":310.0,\"r2L\":168.0,\"r2\":99.30000305175781},{\"name\":\"Engelberg HS140\",\"terrainSteepness\":0.0,\"type\":2,\"gates\":32,\"w\":125.0,\"hn\":0.5809999704360962,\"gamma\":36.0,\"alpha\":11.0,\"e\":99.0,\"es\":22.0,\"t\":7.0,\"r1\":101.0999984741211,\"betaP\":37.79999923706055,\"betaK\":34.79999923706055,\"betaL\":32.20000076293945,\"s\":3.1500000953674318,\"l1\":16.200000762939454,\"l2\":15.0,\"rL\":310.0,\"r2L\":135.0,\"r2\":105.0},{\"name\":\"Seefeld HS109\",\"terrainSteepness\":0.0,\"type\":2,\"gates\":33,\"w\":99.0,\"hn\":0.5550000071525574,\"gamma\":34.0,\"alpha\":11.5,\"e\":91.36000061035156,\"es\":18.209999084472658,\"t\":5.769999980926514,\"r1\":95.19999694824219,\"betaP\":35.79999923706055,\"betaK\":33.29999923706055,\"betaL\":31.0,\"s\":2.4600000381469728,\"l1\":10.039999961853028,\"l2\":10.0,\"rL\":230.0,\"r2L\":200.0,\"r2\":110.0},{\"name\":\"Innsbruck HS130\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":28,\"w\":120.0,\"hn\":0.5789999961853027,\"gamma\":35.0,\"alpha\":10.75,\"e\":90.69999694824219,\"es\":17.700000762939454,\"t\":6.5,\"r1\":100.0,\"betaP\":37.0,\"betaK\":34.29999923706055,\"betaL\":32.0,\"s\":3.0799999237060549,\"l1\":11.130000114440918,\"l2\":10.0,\"rL\":240.0,\"r2L\":100.0,\"r2\":100.0},{\"name\":\"Ga-Pa HS142\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":49,\"w\":125.0,\"hn\":0.5789999961853027,\"gamma\":35.0,\"alpha\":11.0,\"e\":96.0,\"es\":24.0,\"t\":6.900000095367432,\"r1\":103.0,\"betaP\":37.20000076293945,\"betaK\":34.70000076293945,\"betaL\":32.20000076293945,\"s\":3.200000047683716,\"l1\":14.640000343322754,\"l2\":17.0,\"rL\":335.0,\"r2L\":100.0,\"r2\":114.5},{\"name\":\"Bischofshofen HS142\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":47,\"w\":125.0,\"hn\":0.5799999833106995,\"gamma\":27.0,\"alpha\":11.0,\"e\":118.5,\"es\":23.709999084472658,\"t\":6.5,\"r1\":96.0,\"betaP\":37.5,\"betaK\":35.0,\"betaL\":32.5,\"s\":4.5,\"l1\":14.829999923706055,\"l2\":17.0,\"rL\":335.0,\"r2L\":115.0,\"r2\":115.0},{\"name\":\"Predazzo HS135\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":40,\"w\":120.0,\"hn\":0.5770000219345093,\"gamma\":30.0,\"alpha\":10.5,\"e\":115.36000061035156,\"es\":35.0,\"t\":7.0,\"r1\":106.0,\"betaP\":37.900001525878909,\"betaK\":34.900001525878909,\"betaL\":32.29999923706055,\"s\":3.0,\"l1\":16.600000381469728,\"l2\":15.0,\"rL\":311.0,\"r2L\":100.0,\"r2\":100.0},{\"name\":\"Predazzo HS104\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":35,\"w\":95.0,\"hn\":0.5509999990463257,\"gamma\":30.0,\"alpha\":10.5,\"e\":92.58000183105469,\"es\":23.799999237060548,\"t\":6.5,\"r1\":85.0,\"betaP\":36.400001525878909,\"betaK\":34.0,\"betaL\":31.399999618530275,\"s\":2.380000114440918,\"l1\":10.34000015258789,\"l2\":9.0,\"rL\":240.0,\"r2L\":102.0,\"r2\":102.0},{\"name\":\"Wisla HS134\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":33,\"w\":120.0,\"hn\":0.578000009059906,\"gamma\":35.0,\"alpha\":10.5,\"e\":88.19999694824219,\"es\":16.0,\"t\":6.599999904632568,\"r1\":97.0,\"betaP\":37.79999923706055,\"betaK\":34.79999923706055,\"betaL\":32.290000915527347,\"s\":3.0,\"l1\":16.780000686645509,\"l2\":14.0,\"rL\":317.010009765625,\"r2L\":94.4000015258789,\"r2\":94.4000015258789},{\"name\":\"Willingen HS145\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":49,\"w\":130.0,\"hn\":0.5899999737739563,\"gamma\":35.0,\"alpha\":11.0,\"e\":100.0,\"es\":26.950000762939454,\"t\":6.699999809265137,\"r1\":105.0,\"betaP\":38.0,\"betaK\":35.0,\"betaL\":32.5,\"s\":3.25,\"l1\":18.209999084472658,\"l2\":15.0,\"rL\":347.0,\"r2L\":108.0,\"r2\":108.0},{\"name\":\"Oberstdorf HS235\",\"terrainSteepness\":0.0,\"type\":2,\"gates\":49,\"w\":200.0,\"hn\":0.6050000190734863,\"gamma\":38.70000076293945,\"alpha\":11.199999809265137,\"e\":109.5,\"es\":24.0,\"t\":8.5,\"r1\":110.0,\"betaP\":36.900001525878909,\"betaK\":33.900001525878909,\"betaL\":31.399999618530275,\"s\":3.4000000953674318,\"l1\":28.799999237060548,\"l2\":35.0,\"rL\":500.0,\"r2L\":150.0,\"r2\":100.0},{\"name\":\"Lahti HS130\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":24,\"w\":116.0,\"hn\":0.5619999766349793,\"gamma\":38.63999938964844,\"alpha\":10.5,\"e\":83.55000305175781,\"es\":14.800000190734864,\"t\":6.449999809265137,\"r1\":95.55000305175781,\"betaP\":37.70000076293945,\"betaK\":34.70000076293945,\"betaL\":32.20000076293945,\"s\":3.4100000858306886,\"l1\":16.0,\"l2\":14.0,\"rL\":305.79998779296877,\"r2L\":92.87000274658203,\"r2\":92.87000274658203},{\"name\":\"Seefeld HS109\",\"terrainSteepness\":0.0,\"type\":2,\"gates\":33,\"w\":99.0,\"hn\":0.5550000071525574,\"gamma\":34.0,\"alpha\":11.5,\"e\":91.36000061035156,\"es\":18.209999084472658,\"t\":5.769999980926514,\"r1\":95.19999694824219,\"betaP\":35.79999923706055,\"betaK\":33.29999923706055,\"betaL\":31.0,\"s\":2.4600000381469728,\"l1\":10.039999961853028,\"l2\":10.0,\"rL\":230.0,\"r2L\":200.0,\"r2\":110.0},{\"name\":\"Seefeld HS75\",\"terrainSteepness\":0.0,\"type\":2,\"gates\":32,\"w\":65.0,\"hn\":0.5230000019073486,\"gamma\":30.0,\"alpha\":9.5,\"e\":68.61000061035156,\"es\":23.200000762939454,\"t\":6.0,\"r1\":80.69999694824219,\"betaP\":36.0,\"betaK\":34.20000076293945,\"betaL\":32.70000076293945,\"s\":1.899999976158142,\"l1\":5.0,\"l2\":7.0,\"rL\":140.0,\"r2L\":200.0,\"r2\":110.0},{\"name\":\"Szczyrk HS106\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":26,\"w\":95.0,\"hn\":0.550000011920929,\"gamma\":35.0,\"alpha\":11.0,\"e\":85.0,\"es\":18.200000762939454,\"t\":6.099999904632568,\"r1\":90.0,\"betaP\":36.0,\"betaK\":34.0,\"betaL\":31.5,\"s\":2.4000000953674318,\"l1\":9.0,\"l2\":11.0,\"rL\":243.0800018310547,\"r2L\":90.0,\"r2\":90.0},{\"name\":\"Szczyrk HS77\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":20,\"w\":70.0,\"hn\":0.5239999890327454,\"gamma\":34.0,\"alpha\":10.5,\"e\":70.0,\"es\":13.699999809265137,\"t\":5.599999904632568,\"r1\":75.0,\"betaP\":34.79999923706055,\"betaK\":33.0,\"betaL\":30.700000762939454,\"s\":1.75,\"l1\":5.0,\"l2\":7.0,\"rL\":160.0,\"r2L\":80.0,\"r2\":80.0},{\"name\":\"Planica HS240\",\"terrainSteepness\":0.0,\"type\":2,\"gates\":31,\"w\":200.0,\"hn\":0.6000000238418579,\"gamma\":35.099998474121097,\"alpha\":11.0,\"e\":125.80000305175781,\"es\":31.139999389648439,\"t\":8.0,\"r1\":105.0,\"betaP\":35.599998474121097,\"betaK\":33.20000076293945,\"betaL\":30.0,\"s\":2.930000066757202,\"l1\":17.0,\"l2\":40.0,\"rL\":744.0,\"r2L\":110.0,\"r2\":100.0},{\"name\":\"Harrachov HS142\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":22,\"w\":125.0,\"hn\":0.5770000219345093,\"gamma\":27.5,\"alpha\":10.5,\"e\":107.80000305175781,\"es\":20.100000381469728,\"t\":7.5,\"r1\":101.0,\"betaP\":37.0,\"betaK\":34.5,\"betaL\":32.099998474121097,\"s\":3.9000000953674318,\"l1\":17.0,\"l2\":17.0,\"rL\":390.0,\"r2L\":110.0,\"r2\":110.0},{\"name\":\"Harrachov HS210\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":39,\"w\":185.0,\"hn\":0.609000027179718,\"gamma\":35.0,\"alpha\":10.5,\"e\":114.5,\"es\":19.0,\"t\":8.0,\"r1\":125.0,\"betaP\":38.5,\"betaK\":34.5,\"betaL\":32.29999923706055,\"s\":4.619999885559082,\"l1\":37.0,\"l2\":25.0,\"rL\":520.0,\"r2L\":115.0,\"r2\":115.0},{\"name\":\"Klingenthal HS140\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":54,\"w\":125.0,\"hn\":0.578000009059906,\"gamma\":35.0,\"alpha\":11.0,\"e\":97.5,\"es\":24.0,\"t\":6.800000190734863,\"r1\":105.0,\"betaP\":37.0,\"betaK\":34.5,\"betaL\":32.5,\"s\":3.0999999046325685,\"l1\":19.0,\"l2\":15.0,\"rL\":424.1000061035156,\"r2L\":120.0,\"r2\":120.0},{\"name\":\"Whistler HS142\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":30,\"w\":125.0,\"hn\":0.574999988079071,\"gamma\":35.0,\"alpha\":11.0,\"e\":95.55000305175781,\"es\":20.549999237060548,\"t\":6.599999904632568,\"r1\":103.0,\"betaP\":37.0,\"betaK\":35.0,\"betaL\":32.5,\"s\":3.0999999046325685,\"l1\":12.0,\"l2\":17.0,\"rL\":337.0,\"r2L\":115.0,\"r2\":115.0},{\"name\":\"Whistler HS104\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":34,\"w\":95.0,\"hn\":0.550000011920929,\"gamma\":35.0,\"alpha\":11.0,\"e\":86.0999984741211,\"es\":22.0,\"t\":6.099999904632568,\"r1\":90.0,\"betaP\":36.5,\"betaK\":34.0,\"betaL\":31.5,\"s\":2.380000114440918,\"l1\":11.0,\"l2\":9.0,\"rL\":244.0,\"r2L\":115.0,\"r2\":115.0},{\"name\":\"Kuopio HS127\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":34,\"w\":120.0,\"hn\":0.574999988079071,\"gamma\":35.0,\"alpha\":11.0,\"e\":92.0,\"es\":24.600000381469728,\"t\":6.599999904632568,\"r1\":95.0,\"betaP\":37.79999923706055,\"betaK\":34.79999923706055,\"betaL\":33.29999923706055,\"s\":3.0,\"l1\":15.0,\"l2\":7.0,\"rL\":283.0,\"r2L\":95.0,\"r2\":95.0},{\"name\":\"Trondheim HS140\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":31,\"w\":124.0,\"hn\":0.5720000267028809,\"gamma\":34.0,\"alpha\":11.300000190734864,\"e\":88.6500015258789,\"es\":13.40999984741211,\"t\":6.5,\"r1\":105.0,\"betaP\":36.29999923706055,\"betaK\":33.79999923706055,\"betaL\":29.700000762939454,\"s\":3.0,\"l1\":16.0,\"l2\":16.0,\"rL\":286.6000061035156,\"r2L\":107.0,\"r2\":107.0},{\"name\":\"Lillehammer HS140\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":33,\"w\":123.0,\"hn\":0.5740000009536743,\"gamma\":34.0,\"alpha\":11.100000381469727,\"e\":101.5,\"es\":26.649999618530275,\"t\":6.599999904632568,\"r1\":107.0,\"betaP\":37.0,\"betaK\":34.5,\"betaL\":32.0,\"s\":3.3499999046325685,\"l1\":15.0,\"l2\":17.0,\"rL\":347.0,\"r2L\":108.0,\"r2\":108.0},{\"name\":\"Oslo HS134\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":40,\"w\":120.0,\"hn\":0.5699999928474426,\"gamma\":36.0,\"alpha\":11.0,\"e\":89.73999786376953,\"es\":17.600000381469728,\"t\":6.599999904632568,\"r1\":108.80000305175781,\"betaP\":35.70000076293945,\"betaK\":33.20000076293945,\"betaL\":30.799999237060548,\"s\":3.0,\"l1\":14.399999618530274,\"l2\":14.0,\"rL\":329.79998779296877,\"r2L\":106.0,\"r2\":106.0},{\"name\":\"Vikersund HS240\",\"terrainSteepness\":0.0,\"type\":2,\"gates\":41,\"w\":200.0,\"hn\":0.6000000238418579,\"gamma\":36.0,\"alpha\":10.5,\"e\":121.76000213623047,\"es\":35.709999084472659,\"t\":8.0,\"r1\":115.0,\"betaP\":38.0,\"betaK\":34.5,\"betaL\":31.700000762939454,\"s\":2.640000104904175,\"l1\":35.0,\"l2\":40.0,\"rL\":648.0,\"r2L\":120.0,\"r2\":65.0},{\"name\":\"Hinterzarten HS108\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":45,\"w\":95.0,\"hn\":0.5609999895095825,\"gamma\":35.20000076293945,\"alpha\":11.199999809265137,\"e\":83.25,\"es\":21.5,\"t\":6.25,\"r1\":75.27999877929688,\"betaP\":37.0,\"betaK\":35.0,\"betaL\":32.0,\"s\":2.5799999237060549,\"l1\":9.0,\"l2\":13.0,\"rL\":256.6000061035156,\"r2L\":95.0,\"r2\":95.0},{\"name\":\"Hinterzarten HS77\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":37,\"w\":70.0,\"hn\":0.5299999713897705,\"gamma\":35.0,\"alpha\":10.5,\"e\":74.0,\"es\":17.200000762939454,\"t\":5.900000095367432,\"r1\":80.0,\"betaP\":36.0,\"betaK\":33.5,\"betaL\":30.799999237060548,\"s\":1.75,\"l1\":7.0,\"l2\":7.0,\"rL\":156.0,\"r2L\":85.0,\"r2\":85.0},{\"name\":\"Pragelato HS140\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":44,\"w\":125.0,\"hn\":0.5830000042915344,\"gamma\":35.0,\"alpha\":11.0,\"e\":98.5999984741211,\"es\":21.0,\"t\":6.800000190734863,\"r1\":105.0,\"betaP\":37.400001525878909,\"betaK\":34.900001525878909,\"betaL\":32.400001525878909,\"s\":3.240000009536743,\"l1\":15.0,\"l2\":15.0,\"rL\":336.0,\"r2L\":111.0,\"r2\":111.0},{\"name\":\"Pragelato HS106\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":38,\"w\":95.0,\"hn\":0.5580000281333923,\"gamma\":35.0,\"alpha\":11.0,\"e\":88.0,\"es\":18.079999923706056,\"t\":6.400000095367432,\"r1\":92.0,\"betaP\":37.0,\"betaK\":34.5,\"betaL\":32.0,\"s\":2.380000114440918,\"l1\":11.0,\"l2\":11.0,\"rL\":241.0,\"r2L\":111.0,\"r2\":111.0},{\"name\":\"Einsiedeln HS117\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":32,\"w\":105.0,\"hn\":0.550000011920929,\"gamma\":35.0,\"alpha\":10.5,\"e\":93.30000305175781,\"es\":21.799999237060548,\"t\":6.699999809265137,\"r1\":105.0,\"betaP\":36.900001525878909,\"betaK\":33.900001525878909,\"betaL\":31.399999618530275,\"s\":2.5999999046325685,\"l1\":15.0,\"l2\":12.0,\"rL\":274.0,\"r2L\":92.0,\"r2\":92.0},{\"name\":\"Einsiedeln HS77\",\"terrainSteepness\":0.0,\"type\":1,\"gates\":28,\"w\":70.0,\"hn\":0.5149999856948853,\"gamma\":30.0,\"alpha\":10.5,\"e\":88.0,\"es\":24.0,\"t\":6.199999809265137,\"r1\":85.0,\"betaP\":35.5,\"betaK\":32.5,\"betaL\":29.81999969482422,\"s\":1.75,\"l1\":18.0,\"l2\":7.0,\"rL\":158.5,\"r2L\":79.0,\"r2\":79.0}]}";
            AllData loadedData = JsonUtility.FromJson<AllData>(dataAsJson);
            hills = loadedData.profileData;
        }
    }

    private void SaveData()
    {
        string dataAsJson = JsonUtility.ToJson(allData);

        /*****      WebGL    *****/
        // PlayerPrefs.SetString("Hills3", dataAsJson);

        /*****   Standalone  *****/
        string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
        File.WriteAllText(filePath, dataAsJson);
    }


}