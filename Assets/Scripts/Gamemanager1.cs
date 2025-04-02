using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Gamemanager : MonoBehaviour
{
    public Image wordImage;
    public AudioSource wordAudio;
    public TMP_Text wordText;
    public List<Sprite> wordImages;
    public List<AudioClip> wordAudios;
    public List<string> wordNames;
    private int currentIndex = 0;

    void Start()
    {
        if (wordImages.Count == 0 || wordAudios.Count == 0 || wordNames.Count == 0)
        {
            Debug.LogError("Word assets are missing!");
            return;
        }

        UpdateWord();
    }

    public void NextWord()
    {
        currentIndex = (currentIndex + 1) % wordImages.Count;
        UpdateWord();
    }

    private void UpdateWord()
    {
        wordImage.sprite = wordImages[currentIndex];
        wordAudio.clip = wordAudios[currentIndex];
        wordText.text = wordNames[currentIndex];
    }

    public void PlayWordAudio()
    {
        if (wordAudio.clip != null)
        {
            wordAudio.Play();
        }
    }
}
