using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompetitionManager : MonoBehaviour
{
    public ManagerScript managerScript;
    public JumpUIManager jumpUIManager;
    public bool blocked;
    public bool tournament;
    public bool hillFromEditor;
    public List<JumperData> jumpers;
    public Country[] countries;
    public JudgesController judges;

    public float[] results;
    public bool[] jumped;

    public bool finished;
    public int round;
    public int bib = 0;

    private TournamentData tournamentData;
    public void LoadData()
    {
        string dataAsJson = PlayerPrefs.GetString("competitorsData");
        CompetitorsData loadedData = JsonUtility.FromJson<CompetitorsData>(dataAsJson);
        jumpers = loadedData.jumpersList;
        countries = loadedData.countriesList;


        dataAsJson = PlayerPrefs.GetString("TournamentData");
        tournamentData = JsonUtility.FromJson<TournamentData>(dataAsJson);
        Debug.Log("#HILLS: " + tournamentData.hillsList.Count);
        Debug.Log("#JUMPERS: " + tournamentData.resultList.Count);

    }

    public void CompetitionInit()
    {
        jumped = new bool[jumpers.Count];
        bib = 0;
        if (round == 0)
        {
            results = new float[jumpers.Count];
            round = 0;
        }
        else if (round == 2)
        {
            finished = true;
            
            for (int i = 0; i < jumpers.Count; i++)
            {
                results[i] += tournamentData.resultList[i];
            }
        }
        else if (round >= 4)
        {
            for (int i = 0; i < jumpers.Count; i++)
            {
                tournamentData.resultList[i] = results[i];
            }

            string dataAsJson = JsonUtility.ToJson(tournamentData);
            PlayerPrefs.SetString("TournamentData", dataAsJson);

            int it = PlayerPrefs.GetInt("HillCodeIt")+1;

            if (it < tournamentData.hillsList.Count)
            {
                PlayerPrefs.SetInt("HillCodeIt", it);
                PlayerPrefs.SetInt("HillCode", tournamentData.hillsList[it]);
                SceneManager.LoadScene(3);
            }
            else
            {
                SceneManager.LoadScene(0, LoadSceneMode.Additive);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!blocked)
        {
            finished = false;
            LoadData();
            CompetitionInit();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
