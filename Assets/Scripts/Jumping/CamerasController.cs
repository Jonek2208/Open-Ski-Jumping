using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.Jumping
{
    [Serializable]
    public class CameraData
    {
        public CinemachineVirtualCamera camera;
        public CinemachineFollowZoom followZoom;
        public bool follow;
        public bool lookAt;
        public bool zoom;
        public float zoomWidth;
        public float minFov;
        public float maxFov;
    }

    public class CamerasController : MonoBehaviour
    {
        public int currentCamId;
        public CameraData[] cameras;
        [SerializeField] private float minWidth = 3;
        [SerializeField] private float maxWidth = 100;
        [SerializeField] private float sensitivity = -10f;

        public void EnableCamera(int id)
        {
            foreach (var cam in cameras) cam.camera.enabled = false;
            currentCamId = id;

            cameras[id].camera.enabled = true;
            cameras[id].followZoom.enabled = cameras[id].zoom;
            PlayerPrefs.SetInt("camera", id);
        }

        private void Update()
        {
            var zoomWidth = cameras[currentCamId].zoomWidth;
            
            var newWidth = zoomWidth + Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            newWidth = Mathf.Clamp(newWidth, minWidth, maxWidth);
            cameras[currentCamId].zoomWidth = newWidth;
            cameras[currentCamId].followZoom.m_Width = cameras[currentCamId].zoomWidth;
        }
    }
}