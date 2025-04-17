using UnityEngine;
using TMPro; // Need this for TextMeshPro
using UnityEngine.UI; // Need this for Button clicks (if using Button component for letter click)
using UnityEngine.EventSystems; // Need this for Event Trigger (better for clicking text)

public class AlphabetController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI letterText; // Drag your TextMeshPro object here

    [Header("Alphabet Data")]
    public AudioClip[] letterSounds; // Drag your audio clips here IN ORDER (A, B, C...)

    [Header("Audio Settings")]
    public AudioSource audioSource; // We'll add this component later

    private char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private int currentIndex = 0;

    void Start()
    {
        // Try to get the AudioSource component if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            // If still null, add one
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Initial display
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (letterText != null)
        {
            letterText.text = alphabet[currentIndex].ToString();
        }
        else
        {
            Debug.LogError("Letter Text UI element is not assigned in the Inspector!");
        }
    }

    public void ShowNextLetter()
    {
        currentIndex++;
        // Wrap around from Z back to A
        if (currentIndex >= alphabet.Length)
        {
            currentIndex = 0;
        }
        UpdateDisplay();
        // Optional: Play sound automatically when changing letter
        // PlayCurrentLetterSound();
    }

    public void ShowPreviousLetter()
    {
        currentIndex--;
        // Wrap around from A back to Z
        if (currentIndex < 0)
        {
            currentIndex = alphabet.Length - 1;
        }
        UpdateDisplay();
        // Optional: Play sound automatically when changing letter
        // PlayCurrentLetterSound();
    }

    public void PlayCurrentLetterSound()
    {
        // Check if audioSource is ready and the index is valid for the sounds array
        if (audioSource != null && letterSounds != null && currentIndex >= 0 && currentIndex < letterSounds.Length)
        {
            AudioClip clipToPlay = letterSounds[currentIndex];
            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay); // PlayOneShot is good for non-looping effects
            }
            else
            {
                Debug.LogWarning($"AudioClip for letter '{alphabet[currentIndex]}' (index {currentIndex}) is missing!");
            }
        }
        else
        {
            if (audioSource == null) Debug.LogError("AudioSource is missing!");
            if (letterSounds == null) Debug.LogError("Letter Sounds array is not assigned!");
            if (currentIndex < 0 || currentIndex >= letterSounds.Length) Debug.LogError($"CurrentIndex ({currentIndex}) is out of bounds for letterSounds array (Length: {letterSounds.Length})");
        }
    }

    public void ResetLetter()
    {
        currentIndex = 0; // Set the index back to the first letter (A)
        UpdateDisplay();  // Update the text displayed on screen
        // Optional: Uncomment the line below if you want it to play the 'A' sound on reset
        // PlayCurrentLetterSound();
    }
}