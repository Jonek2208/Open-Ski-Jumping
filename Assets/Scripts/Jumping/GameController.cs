using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace OpenSkiJumping.Jumping
{
    public class GameController : MonoBehaviour
    {
        public CamerasController camerasController;
        public UnityEvent OnJumpRestart;
        public UnityEvent OnPauseMenu;

        void Start()
        {
            if (!PlayerPrefs.HasKey("camera"))
            {
                camerasController.EnableCamera(0);
            }
            else
            {
                camerasController.EnableCamera(PlayerPrefs.GetInt("camera"));
            }
        }

        int reversedCam;
        int currentCam;

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void RestartJump()
        {
            OnJumpRestart.Invoke();
        }

        void Update()
        {
            KeyCode[] keys =
            {
                KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, /*KeyCode.F6, KeyCode.F7, KeyCode.F8,
                KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12*/
            };
            for (var i = 0; i < keys.Length; i++)
            {
                if (!Input.GetKeyDown(keys[i])) continue;
                camerasController.EnableCamera(2 * i + reversedCam);
                currentCam = i;
            }
            

            if (Input.GetKeyDown(KeyCode.Q))
            {
                reversedCam = 1 - reversedCam;
                camerasController.EnableCamera(2 * currentCam + reversedCam);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnPauseMenu.Invoke();
                // LoadMainMenu();
            }
        }
    }
}