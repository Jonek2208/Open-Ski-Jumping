using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class MainMenuController : ScriptableObject
{
    public void LoadEditor()
    {
        SceneManager.LoadScene("Scenes/Hills/Templates/HillTemplateEditor");
    }
    public void LoadTournament()
    {
        SceneManager.LoadScene("Scenes/Hills/Templates/HillTemplateEditor");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
    public void LoadTournamentMenu()
    {
        SceneManager.LoadScene("Scenes/TournamentMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
