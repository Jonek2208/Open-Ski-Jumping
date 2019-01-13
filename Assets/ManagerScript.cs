using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using HillProfile;
using HillDataSerialization;


public class ManagerScript : MonoBehaviour 
{

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


	public MeshScript meshGenerator;

	public AllData allData;
	public List<ProfileData> hills;

	private string dataFileName = "data.json";
	

	void Start () 
	{
		meshGenerator = GetComponentInChildren<MeshScript>();
		LoadData();
		if(hills.Count == 0)
		{
			hills.Add(GetProfileData(new Hill(Hill.ProfileType.ICR1992, 120, 0.575f, 35, 11, 95.2f, 20.5f, 6.5f, 107, 37.5f, 37.5f, 37.5f, 3, 30, 11, 0, 0, 126f), "Hakuba HS131"));
			hills.Add(GetProfileData(new Hill(Hill.ProfileType.ICR1996, 120, 0.575f, 35, 11, 99, 23, 6.5f, 115, 37.43f, 35.5f, 32.4f, 3.38f, 11.15f, 17.42f, 321, 100, 100), "Oberstdorf HS137"));
			hills.Add(GetProfileData(new Hill(Hill.ProfileType.ICR2008, 125, 0.575f, 35, 11, 98.7f, 22, 6.5f, 90, 37.05f, 34.3f, 31.4f, 3.13f, 16, 15, 310, 168, 99.3f), "Zakopane HS140"));
		}

		dataDropdown.options = new List<TMPro.TMP_Dropdown.OptionData>();

		foreach(ProfileData it in hills) 
		{
			TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData();
			option.text = it.name;
			dataDropdown.options.Add(option);
		}

		LoadDefaultHill(0);
	}
	void Update () 
	{
		
	}

	public void GenerateButtonClick()
	{
		Hill hill = new Hill((Hill.ProfileType)(typeDropdown.value), float.Parse(wInput.text), float.Parse(hnInput.text), float.Parse(gammaInput.text), float.Parse(alphaInput.text),
			float.Parse(eInput.text), float.Parse(esInput.text), float.Parse(tInput.text), float.Parse(r1Input.text), float.Parse(betaPInput.text),
			float.Parse(betaKInput.text), float.Parse(betaLInput.text), float.Parse(sInput.text), float.Parse(l1Input.text), float.Parse(l2Input.text),
			float.Parse(rLInput.text), float.Parse(r2LInput.text), float.Parse(r2Input.text));
		
		meshGenerator.GenerateMesh(hill, int.Parse(gatesInput.text));
	}

	public void SaveButtonClick()
	{
		Hill hill = new Hill((Hill.ProfileType)(typeDropdown.value), float.Parse(wInput.text), float.Parse(hnInput.text), float.Parse(gammaInput.text), float.Parse(alphaInput.text),
			float.Parse(eInput.text), float.Parse(esInput.text), float.Parse(tInput.text), float.Parse(r1Input.text), float.Parse(betaPInput.text),
			float.Parse(betaKInput.text), float.Parse(betaLInput.text), float.Parse(sInput.text), float.Parse(l1Input.text), float.Parse(l2Input.text),
			float.Parse(rLInput.text), float.Parse(r2LInput.text), float.Parse(r2Input.text));

		hills.Add(GetProfileData(hill, nameInput.text));
		allData.profileData = hills;
		SaveData();
		TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData();
		option.text = nameInput.text;
		dataDropdown.options.Add(option);
		dataDropdown.value = dataDropdown.options.Count-1;
		//MeshScript meshScript = GetComponentInChildren<MeshScript>();
		//meshScript.SaveMesh();
	}

	public void LoadDefaultHill(int id) 
	{
		
		// Hill[] tab = {
		// 	new Hill(Hill.ProfileType.ICR1996, 120, 0.575f, 35, 11, 99, 23, 6.5f, 115, 37.43f, 35.5f, 32.4f, 3.38f, 11.15f, 17.42f, 321, 100, 100),
		// 	new Hill(Hill.ProfileType.ICR2008, 125, 0.575f, 35, 11, 98.7f, 22, 6.5f, 90, 37.05f, 34.3f, 31.4f, 3.13f, 16, 15, 310, 168, 99.3f),
		// 	new Hill(Hill.ProfileType.ICR1992, 120, 0.575f, 35, 11, 95.2f, 20.5f, 6.5f, 107, 37.5f, 37.5f, 37.5f, 3, 30, 11, 0, 0, 126f)
        // };
		// Hill hill = tab[id];

		ProfileData tmp = hills.ToArray()[id];
		nameInput.text = tmp.name;
		typeDropdown.value = (int)(tmp.type);
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
	}

	public ProfileData GetProfileData(Hill hill, string name)
	{
		ProfileData res = new ProfileData();
		res.name = name;
		res.type = hill.type;
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
		return res;
	}

	private void LoadData()
	{
		Debug.Log(Application.streamingAssetsPath);
		string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
		if(File.Exists(filePath))
		{
			string dataAsJson = File.ReadAllText(filePath);
			AllData loadedData = JsonUtility.FromJson<AllData>(dataAsJson);
			hills = loadedData.profileData;
		}
		else
		{
			Debug.LogError("No data!");
		}
	}

	private void SaveData()
	{
		string dataAsJson = JsonUtility.ToJson(allData);
		string filePath = Path.Combine(Application.streamingAssetsPath, dataFileName);
		File.WriteAllText(filePath, dataAsJson);
	}
	
}
