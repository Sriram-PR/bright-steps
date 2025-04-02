using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image wordImage;
    public Sprite[] wordSprites; // Drag images here
    public AudioManager audioManager;

    void Start()
    {
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        UpdateWord();
    }

    public void UpdateWord()
    {
        int index = audioManager.GetCurrentIndex();
        if (index >= 0 && index < wordSprites.Length)
        {
            wordImage.sprite = wordSprites[index];
        }
    }
}
