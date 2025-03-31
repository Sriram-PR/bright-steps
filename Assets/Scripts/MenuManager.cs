using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Assign these in the Unity Inspector
    public Button gameOneButton;
    public Button gameTwoButton;
    public Button gameThreeButton;
    public Button gameFourButton;
    public Button gameFiveButton;
    public Button gameSixButton;

    void Start()
    {
        // Connect the buttons to their respective functions
        if (gameOneButton != null)
        {
            gameOneButton.onClick.AddListener(() => OpenGameScene("GameOneScene"));
        }

        if (gameTwoButton != null)
        {
            gameTwoButton.onClick.AddListener(() => OpenGameScene("GameTwoScene"));
        }

        if (gameThreeButton != null)
        {
            gameThreeButton.onClick.AddListener(() => OpenGameScene("GameThreeScene-Legend"));
        }

        if (gameFourButton != null)
        {
            gameFourButton.onClick.AddListener(() => OpenGameScene("GameFourScene-Legend"));
        }

        if (gameFiveButton != null)
        {
            gameFiveButton.onClick.AddListener(() => OpenGameScene("GameFiveScene-Osho"));
        }

        if (gameSixButton != null)
        {
            gameSixButton.onClick.AddListener(() => OpenGameScene("GameSixScene-Osho"));
        }
    }

    void OpenGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
