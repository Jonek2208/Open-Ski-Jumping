using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    // public GameObject pauseMenuUI;

    // void Start()
    // {

    // }

    // public void Resume()
    // {
    //     Time.timeScale = 1f;
    //     GameIsPaused = true;
    // }

    // public void Pause()
    // {
    //     Time.timeScale = 0f;
    //     GameIsPaused = true;
    // }

    public void MainMenuButtonClick()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadEditor()
    {
        SceneManager.LoadScene("Scenes/Hills/Templates/HillTemplateEditor");
    }

    public void LoadCompetition()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadCompetition2()
    {
        SceneManager.LoadScene(3);
    }

    /* Temporary */
    public void Zakopane()
    {
        SceneManager.LoadScene("Scenes/Hills/Zakopane");
    }

    public void Wisla()
    {
        SceneManager.LoadScene("Scenes/Hills/Zakopane");
    }

    public void Vikersund()
    {
        SceneManager.LoadScene("Scenes/Hills/Vikersund");
    }

    public void Falun()
    {
        SceneManager.LoadScene("Scenes/Hills/Falun");
    }

    /******************* */
    public void QuitGame()
    {
        Application.Quit();
    }
}
