using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    void Start()
    {
        
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = true;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void MainMenuButtonClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
