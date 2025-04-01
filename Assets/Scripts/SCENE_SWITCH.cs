using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class MainMenu : MonoBehaviour
{
    public void LoadEasyLevel()
    {
        SceneManager.LoadScene("EASY_LVL"); // Load Easy Level
    }

    public void LoadMediumLevel()
    {
        SceneManager.LoadScene("MEDIUM_LVL"); // Load Medium Level
    }

    public void LoadHardLevel()
    {
        SceneManager.LoadScene("HARD_LVL"); // Load Hard Level
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
