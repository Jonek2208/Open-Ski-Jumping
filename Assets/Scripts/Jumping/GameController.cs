using System.Collections;
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

        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject restartButton;
        

        public void Start()
        {
            camerasController.EnableCamera(!PlayerPrefs.HasKey("camera") ? 0 : PlayerPrefs.GetInt("camera"));
        }

        private int _reversedCam;
        private int _currentCam;

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void RestartJump()
        {
            OnJumpRestart.Invoke();
        }
        
        private static IEnumerator ActivateButtonRoutine(float cooldown, GameObject obj)
        {
            yield return new WaitForSeconds(cooldown);
            obj.SetActive(true);
        }

        public void ActivateStartButton(float cooldown)
        {
            StartCoroutine(ActivateButtonRoutine(cooldown, startButton));
        }

        public void ActivateRestartButton(float cooldown)
        {
            StartCoroutine(ActivateButtonRoutine(cooldown, restartButton));
        }

        private void Update()
        {
            KeyCode[] keys =
            {
                KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, /*KeyCode.F6, KeyCode.F7, KeyCode.F8,
                KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12*/
            };
            for (var i = 0; i < keys.Length; i++)
            {
                if (!Input.GetKeyDown(keys[i])) continue;
                camerasController.EnableCamera(2 * i + _reversedCam);
                _currentCam = i;
            }
            

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _reversedCam = 1 - _reversedCam;
                camerasController.EnableCamera(2 * _currentCam + _reversedCam);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnPauseMenu.Invoke();
                // LoadMainMenu();
            }
        }
    }
}