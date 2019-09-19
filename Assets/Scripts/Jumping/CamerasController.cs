using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    
    public Camera[] cameras;

    public void EnableCamera(int id)
    {
        foreach(Camera cam in cameras)
        {
            cam.enabled = false;
           
        }

        cameras[id].enabled = true;
        Debug.Log(cameras[id].ToString());
        PlayerPrefs.SetInt("camera", id);
    }
}
