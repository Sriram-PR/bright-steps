using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OpenMenuPage);
        }
    }

    public void OpenMenuPage()
    {
        SceneManager.LoadScene("MenuScene");
    }
}