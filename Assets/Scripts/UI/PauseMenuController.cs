using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private MainMenuController menuController;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
            }
        }

        public void OnEnable()
        {
            Pause();
        }

        public void OnDisable()
        {
            Resume();
        }

        private void Pause()
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        private void Resume()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
        }

        public void LoadMainMenu()
        {
            Resume();
            menuController.LoadMainMenu();
        }
    }
}