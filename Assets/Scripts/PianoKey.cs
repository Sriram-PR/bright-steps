using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Button

public class PianoKey : MonoBehaviour
{
    // Public variable to hold the sound for THIS key.
    // You will drag your sound file onto this slot in the Unity Editor.
    public AudioClip keySound;

    // Reference to the central AudioSource component that will play the sound.
    private AudioSource audioManagerSource;

    // This function runs when the script first starts
    void Start()
    {
        // Find the AudioManager GameObject in the scene
        GameObject audioManagerObject = GameObject.Find("AudioManager");

        // Check if we found it
        if (audioManagerObject != null)
        {
            // Get the AudioSource component attached to the AudioManager
            audioManagerSource = audioManagerObject.GetComponent<AudioSource>();

            // Optional: Add error message if AudioSource wasn't found on AudioManager
            if (audioManagerSource == null)
            {
                Debug.LogError("AudioSource component not found on the AudioManager GameObject!");
            }
        }
        else
        {
            // Add an error message if the AudioManager itself wasn't found
            Debug.LogError("AudioManager GameObject not found in the scene!");
        }

        // --- This part is removed as we will link via the Button's OnClick event instead ---
        // // Get the Button component attached to this same GameObject
        // Button button = GetComponent<Button>();
        // // Check if we found it
        // if (button != null)
        // {
        //     // Tell the button to call our PlayNote function when it's clicked
        //     button.onClick.AddListener(PlayNote);
        // }
        // else
        // {
        //     Debug.LogError("Button component not found on this GameObject!", this.gameObject);
        // }
        // --- End of removed part ---
    }

    // This is the function that the Button will call when clicked.
    // It MUST be public to be accessible from the Button's OnClick event in the Inspector.
    public void PlayNote()
    {
        // Check if we have a sound assigned AND we found the AudioManager's AudioSource
        if (keySound != null && audioManagerSource != null)
        {
            // Tell the AudioManager's AudioSource to play our assigned sound clip ONCE
            // PlayOneShot allows multiple sounds to overlap if keys are pressed quickly
            audioManagerSource.PlayOneShot(keySound);
        }
        else
        {
            // Optional: Log warnings if something is missing
            if (keySound == null)
            {
                Debug.LogWarning("No AudioClip assigned to this key: " + gameObject.name);
            }
            if (audioManagerSource == null)
            {
                Debug.LogWarning("AudioManager's AudioSource reference is missing.");
            }
        }
    }
}