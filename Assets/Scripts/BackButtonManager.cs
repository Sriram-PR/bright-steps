using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackButtonManager : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(ReturnToMenu);
        }
    }

    void ReturnToMenu()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        switch (currentSceneName)
        {
            case "MenuScene":
                SceneManager.LoadScene("WelcomeScene");
                break;
            case "GameOneScene":
                SceneManager.LoadScene("MenuScene");
                break;
            case "GameTwoScene":
                SceneManager.LoadScene("MenuScene");
                break;
            case "GameThreeScene":
                SceneManager.LoadScene("MenuScene");
                break;
            default:
                SceneManager.LoadScene("WelcomeScene");
                break;
        }
    }
}
