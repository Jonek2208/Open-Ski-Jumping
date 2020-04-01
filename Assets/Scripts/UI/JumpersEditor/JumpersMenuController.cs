using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JumpersMenuController : MonoBehaviour
{
    [SerializeField] private CompetitorsRuntime competitorsRuntime;
    [SerializeField] private JumpersListView jumpersListView;
    private Dictionary<string, int> dict;
    [SerializeField] private JumperInfo jumperInfo;
    [SerializeField] private FlagsData flagsData;
    [SerializeField] private Sprite[] genderIcons;
    [SerializeField] private UnityEventString OnValueChanged;

    void Start()
    {
        dict = competitorsRuntime.Data.Select((item, index) => (item.id, index)).ToDictionary(item => item.id, item => item.index);
        jumpersListView.Items = competitorsRuntime.Data;
        jumpersListView.Initialize(BindListViewItem);
        jumperInfo.Bind(competitorsRuntime.Data[0]);
    }
    public JumpersListElementData GetItemFromJumper(Competition.Competitor competitor)
    {
        return new JumpersListElementData
        {
            firstName = competitor.firstName,
            lastName = competitor.lastName,
            countryCode = competitor.countryCode,
            genderIcon = genderIcons[(int)competitor.gender],
            flagSprite = flagsData.GetFlag(competitor.countryCode),
            id = competitor.id
        };
    }

    private void BindListViewItem(int index, JumpersListItem item)
    {
        item.nameText.text = $"{jumpersListView.Items[index].firstName} {jumpersListView.Items[index].lastName.ToUpper()}";
        item.countryFlagText.text = jumpersListView.Items[index].countryCode;
        item.countryFlagImage.sprite = flagsData.GetFlag(jumpersListView.Items[index].countryCode);
        item.genderIconImage.sprite = genderIcons[(int)jumpersListView.Items[index].gender];
        item.toggleExtension.SetElementId(index);
    }

    public void BindData(string jumperId)
    {
        jumperInfo.Bind(competitorsRuntime.GetJumperById(jumperId));
    }

    public void UpdateCompetitor()
    {
        var competitor = jumperInfo.GetCompetitorValue();

        int index = dict[competitor.id];
        dict.Remove(competitor.id);
        competitorsRuntime.Recalculate(competitor);

        dict.Add(competitor.id, index);
        jumpersListView.Items[index] = competitor;
        jumpersListView.RefreshShownValue();
    }

    public void AddCompetitor()
    {
        var competitor = new Competition.Competitor("", "", "");
        competitorsRuntime.Add(competitor);
        jumpersListView.Add(competitor);
        dict.Add(competitor.id, jumpersListView.Items.Count - 1);
    }

    public void RemoveCompetitor()
    {
        var competitor = jumperInfo.GetCompetitorValue();
        if (competitor == null) return;
        competitorsRuntime.Remove(competitor);
        int index = dict[competitor.id];
        dict.Remove(competitor.id);
        jumpersListView.RemoveAt(index);

        for (int i = index; i < jumpersListView.Items.Count; i++)
        {
            dict[jumpersListView.Items[i].id] = i;
        }
        jumperInfo.Bind(null);
    }
}