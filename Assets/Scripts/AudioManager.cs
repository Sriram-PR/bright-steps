using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] wordAudioClips;  // Drag & drop word audios here

    private int currentIndex = 0;

    void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayWordAudio()
    {
        if (wordAudioClips.Length == 0 || wordAudioClips[currentIndex] == null)
        {
            Debug.LogError("No audio clip assigned!");
            return;
        }

        audioSource.clip = wordAudioClips[currentIndex];
        audioSource.Play();
    }

    public void NextWord()
    {
        currentIndex = (currentIndex + 1) % wordAudioClips.Length;
    }

    public int GetCurrentIndex() => currentIndex;
}
