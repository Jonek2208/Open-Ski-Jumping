using UnityEngine;

namespace OpenSkiJumping.Jumping
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
            float minFocalLength = 10f;
            float maxFocalLength = 1200f;
            float sensitivity = 10f;
            float focalLength = cameras[currentCamId].Camera.focalLength;
            float newFov = focalLength + Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            newFov = Mathf.Clamp(newFov, minFocalLength, maxFocalLength);

            cameras[currentCamId].Camera.fieldOfView = newFov;
            cameras[currentCamId].AngleSize = newFov / focalLength * cameras[currentCamId].AngleSize;
        }
    }
}
