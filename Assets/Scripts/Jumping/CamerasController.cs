using UnityEngine;

namespace Jumping
{
    public class CamerasController : MonoBehaviour
    {
        public int currentCamId;
        public CameraController[] cameras;

        public void EnableCamera(int id)
        {
            currentCamId = id;
            foreach (CameraController cam in cameras)
            {
                cam.Camera.enabled = false;

            }

            cameras[id].Camera.enabled = true;
            Debug.Log(cameras[id].ToString());
            PlayerPrefs.SetInt("camera", id);
        }
        private void Update()
        {
            float minFov = 1f;
            float maxFov = 90f;
            float sensitivity = -10f;
            float fov = cameras[currentCamId].Camera.fieldOfView;
            float newFov = fov + Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            newFov = Mathf.Clamp(newFov, minFov, maxFov);

            cameras[currentCamId].Camera.fieldOfView = newFov;
            cameras[currentCamId].AngleSize = newFov / fov * cameras[currentCamId].AngleSize;
        }
    }
}
