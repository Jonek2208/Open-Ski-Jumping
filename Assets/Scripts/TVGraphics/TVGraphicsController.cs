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
        int competitorId = resultsManager.currentStartList[resultsManager.currentStartListIndex];
        int bib = resultsManager.results[competitorId].Bibs[resultsManager.roundIndex];
        int rank = resultsManager.results[competitorId].Rank;
        CompCal.Competitor competitor = competitors.competitors[participants.participants[competitorId].competitors[resultsManager.subroundIndex]];
        listView.AddItem(new ResultData() { firstName = competitor.firstName, lastName = competitor.lastName.ToUpper(), result = (float)resultsManager.results[competitorId].TotalPoints });
        listView.Items = listView.Items.OrderByDescending(item => item.result).ToList();
    }

    public void ClearListView()
    {
        listView.Items = new List<ResultData>();
    }
}