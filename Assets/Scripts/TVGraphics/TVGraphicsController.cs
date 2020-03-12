using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GraphicsData
{
    public string name;
    public PreJumpUIManager preJump;
    public SpeedUIManager speed;
    public ToBeatUIManager toBeat;
    public PostJumpUIManager postJump;
}

public class TVGraphicsController : MonoBehaviour
{
    public int current;
    public float preJumpGraphicsCooldown;
    public float postJumpGraphicsCooldown;
    public RuntimeResultsManager resultsManager;
    public RuntimeParticipantsList participants;
    public RuntimeCompetitorsList competitors;
    public ListText listView;
    public List<GraphicsData> graphicsData;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            graphicsData[current].preJump.Show();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            graphicsData[current].postJump.Show();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            graphicsData[current].preJump.Hide();
            graphicsData[current].postJump.Hide();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            listView.gameObject.SetActive(!listView.gameObject.activeSelf);
        }
    }

    public void UpdateListView()
    {
        int competitorId = resultsManager.currentStartList[resultsManager.startListIndex];
        int bib = resultsManager.results[competitorId].Bibs[resultsManager.roundIndex];
        int rank = resultsManager.results[competitorId].Rank;
        CompCal.Team team = competitors.teams[participants.participants[competitorId].id];
        listView.AddItem(new ResultData() { firstName = team.teamName, lastName = "", result = (float)resultsManager.results[competitorId].TotalPoints });
        listView.Items = listView.Items.OrderByDescending(item => item.result).ToList();
    }

    public void ClearListView()
    {
        listView.Items = new List<ResultData>();
    }

    public void ShowPreJump()
    {
        graphicsData[current].preJump.Show();
    }

    public void HidePreJump()
    {
        StartCoroutine(HidePreJumpRoutine());
    }

    public void ShowPostJump()
    {
        StartCoroutine(ShowPostJumpRoutine());
    }

    public void HidePostJump()
    {
        graphicsData[current].postJump.InstantHide();
    }

    public void ShowToBeat()
    {
        graphicsData[current].toBeat.Show();
    }

    public void HideToBeat()
    {
        graphicsData[current].toBeat.Hide();
    }

    public void ShowSpeed()
    {
        graphicsData[current].speed.Show();

    }

    public void HideSpeed()
    {
        graphicsData[current].speed.Hide();

    }

    private IEnumerator HidePreJumpRoutine()
    {
        yield return new WaitForSeconds(preJumpGraphicsCooldown);
        graphicsData[current].preJump.Hide();
    }

    private IEnumerator ShowPostJumpRoutine()
    {
        yield return new WaitForSeconds(postJumpGraphicsCooldown);
        graphicsData[current].postJump.Show();
    }
}