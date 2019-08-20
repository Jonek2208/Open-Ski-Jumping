using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    private JumperController jumperController;
    public GameObject gateObject;
    public GameObject jumperObject;
    public int currentGate;
    public int gateCount;
    public Vector3 firstGate;
    public Vector3 lastGate;
    public Vector3 jumperOffset;
    private Vector3 gateDiff;

    void Start()
    {
        jumperController = jumperObject.GetComponent<JumperController>();
        gateDiff = (lastGate - firstGate) / (gateCount - 1);
        if (PlayerPrefs.HasKey("lastGate"))
        {
            GateSet(PlayerPrefs.GetInt("lastGate"));
        }
    }

    public void GateSet(int nr)
    {
        //animacja
        currentGate = nr;
        gateObject.transform.position = firstGate + gateDiff * (currentGate - 1);
        jumperObject.transform.position = jumperOffset + firstGate + gateDiff * (currentGate - 1);
    }

}
