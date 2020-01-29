using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    public int currentCamId;
    public Camera[] cameras;

    public void EnableCamera(int id)
    {
        currentCamId = id;
        foreach (Camera cam in cameras)
        {
            cam.enabled = false;

        }

        cameras[id].enabled = true;
        Debug.Log(cameras[id].ToString());
        PlayerPrefs.SetInt("camera", id);
    }




    private void Update()
    {
        float minFov = 15f;
        float maxFov = 90f;
        float sensitivity = -10f;
        float fov = cameras[currentCamId].fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        cameras[currentCamId].fieldOfView = fov;
    }
}
