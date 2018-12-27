using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    public Camera backCamera;
    public Camera sideCamera;

    void Start()
    {

    }

    public void EnableBackCamera()
    {
        backCamera.enabled = true;
        sideCamera.enabled = false;
        PlayerPrefs.SetInt("camera", 0);
    }

    public void EnableSideCamera()
    {
        backCamera.enabled = false;
        sideCamera.enabled = true;
        PlayerPrefs.SetInt("camera", 1);
    }
}
