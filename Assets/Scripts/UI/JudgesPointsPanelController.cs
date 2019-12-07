using UnityEngine;
using System.Collections.Generic;

public class JudgesPointsPanelController : MonoBehaviour
{
    [SerializeField]
    private JudgesMarkUI[] judgesMarkUI;
    [SerializeField]
    private JudgesMarkInfo[] judgesMarkInfos;

    public void SetUI()
    {
        for (int i = 0; i < Mathf.Min(judgesMarkUI.Length, judgesMarkInfos.Length); i++)
        {
            judgesMarkUI[i].SetUI(judgesMarkInfos[i]);
        }
    }
}