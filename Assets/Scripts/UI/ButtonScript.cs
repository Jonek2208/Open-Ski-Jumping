using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public JumpersManager jumpersManager;
    public TournamentManager tournamentManager;
    public int index;


    void Start()
    {

    }

    public void JumperClick()
    {
        jumpersManager.JumperButtonClick(index);
    }

    public void HillFieldClick()
    {
        tournamentManager.HillFieldButtonClick(index);
    }
}
