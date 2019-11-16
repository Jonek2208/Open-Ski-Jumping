using UnityEngine;
using UnityEngine.SceneManagement;

public static class MainMenuController
{
    public static void LoadEditor()
    {
        SceneManager.LoadScene("Scenes/Hills/Templates/HillTemplateEditor");
    }

    public static void LoadTournament()
    {
        SceneManager.LoadScene("Scenes/Hills/Templates/HillTemplateEditor");
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public static void LoadTournamentMenu()
    {
        SceneManager.LoadScene("Scenes/TournamentMenu");
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
