using UnityEngine;
using TMPro;                 // For TextMeshPro UI elements
using System.Collections.Generic; // For Lists
using UnityEngine.Audio;     // For AudioSource
using System;                  // For TimeSpan formatting
using UnityEngine.UI;          // For Button components
using System.Linq;             // For sorting leaderboard (.OrderBy, .Sort)

// --- Data Structure for Leaderboard Entries ---
[System.Serializable]
public class LeaderboardData { public List<float> times = new List<float>(); }

// Ensures an AudioSource component is attached to this GameObject
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    #region Singleton Pattern
    private static GameManager _instance;
    public static GameManager Instance
    { /* ... Same singleton code as before ... */
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager_Instance");
                    _instance = singletonObject.AddComponent<GameManager>();
                    Debug.Log("GameManager instance created.");
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Public Inspector Fields (Assign These!)
    [Header("Gameplay References (Assign in Inspector!)")]
    public List<DraggableItem> colorBlocks;
    public List<DropZone> dropZones;
    [Header("UI References (Assign in Inspector!)")]
    public TextMeshProUGUI scoreTextDisplay;
    public TextMeshProUGUI timerTextDisplay;
    public GameObject winPanel;
    public Button resetButton;
    public Button leaderboardButton;
    public GameObject leaderboardPanel;
    public TextMeshProUGUI leaderboardEntriesText;
    public Button closeLeaderboardButton;
    [Header("Audio Clips (Assign in Inspector!)")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    [Header("Settings")]
    public int maxLeaderboardEntries = 5;
    public int totalPairsToMatch = 4;
    #endregion

    #region Private State Variables
    private AudioSource audioSource;
    private int currentScore = 0;
    private int matchedPairs = 0;
    private float elapsedTime = 0f;
    private bool isTimerRunning = false;
    private float finalTime = 0f;
    private LeaderboardData leaderboardData = new LeaderboardData();
    private const string LeaderboardSaveKey = "ColorMatchLeaderboardTimes_v1";
    #endregion

    #region Unity Lifecycle Methods (Awake, Start, Update)

    void Awake()
    {
        // --- Enforce Singleton ---
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate GameManager instance found. Destroying self.");
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        // --- Get Necessary Components ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { Debug.LogError("GameManager missing AudioSource component!", this); }
        audioSource.playOnAwake = false;

        // --- Load Saved Data ---
        LoadLeaderboard(); // Safe to do in Awake

        // --- Initial UI State (Doesn't depend on DraggableItem Awake) ---
        if (winPanel != null) winPanel.SetActive(false); else Debug.LogError("Win Panel not assigned in GameManager Inspector!");
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false); else Debug.LogError("Leaderboard Panel not assigned in GameManager Inspector!");

        // --- Validate Core Gameplay References (Check if assigned at all) ---
        if (colorBlocks == null || colorBlocks.Count == 0) Debug.LogError("Color Blocks list is not assigned or empty in GameManager Inspector!");
        if (dropZones == null || dropZones.Count == 0) Debug.LogError("Drop Zones list is not assigned or empty in GameManager Inspector!");
    }

    // Start runs AFTER all Awake methods have completed
    void Start()
    {
        // --- Initial Game Setup (Depends on DraggableItem Awake completing) ---
        // This now runs after DraggableItem.Awake has set startPosition, startScale, parentAfterDrag
        InitializeGame();

        // --- Start the Timer ---
        // Make sure InitializeGame runs first to reset timer state if needed
        if (!isTimerRunning) StartTimer();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    #endregion

    #region Game Flow Control

    // Sets up the game state for starting fresh or resetting
    void InitializeGame()
    {
        currentScore = 0;
        matchedPairs = 0;
        elapsedTime = 0f;
        finalTime = 0f;
        isTimerRunning = false; // Will be set true by StartTimer() shortly after

        UpdateScoreDisplay();
        UpdateTimerDisplay(); // Show "Time: 00:00"

        // Hide end-game UI panels
        if (winPanel != null) winPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);

        // CRITICAL: Reset items now that DraggableItem.Awake should have run
        ReactivateItemsAndZones();

        // Debug.Log($"Game Initialized/Reset. Need to match {totalPairsToMatch} pairs.");
    }

    // --- StartTimer, StopTimer, RecordMatch, CheckWinCondition ---
    // --- (These methods remain exactly the same as the previous version) ---
    public void StartTimer()
    {
        if (isTimerRunning) return;
        elapsedTime = 0f;
        isTimerRunning = true;
        UpdateTimerDisplay();
        Debug.Log("Timer Started.");
    }
    public void StopTimer()
    {
        if (!isTimerRunning) return;
        isTimerRunning = false;
        finalTime = elapsedTime;
        Debug.Log($"Timer Stopped. Final Time: {finalTime:F2} seconds");
    }
    public void RecordMatch(int scoreValue)
    {
        if (!isTimerRunning) return;
        currentScore += scoreValue;
        matchedPairs++;
        UpdateScoreDisplay();
        PlayCorrectSound();
        // Debug.Log($"Match recorded! Matched pairs: {matchedPairs}/{totalPairsToMatch}");
        CheckWinCondition();
    }
    void CheckWinCondition()
    {
        if (matchedPairs >= totalPairsToMatch && isTimerRunning)
        {
            Debug.Log("Win Condition Met!");
            StopTimer();
            SaveTimeToLeaderboard(finalTime);
            ShowWinUI();
        }
    }
    #endregion

    #region UI Update & Panel Control
    // --- UpdateScoreDisplay, UpdateTimerDisplay, ShowWinUI, ---
    // --- ShowLeaderboardPanel, HideLeaderboardPanel ---
    // --- (These methods remain exactly the same as the previous version) ---
    void UpdateScoreDisplay()
    {
        if (scoreTextDisplay != null) scoreTextDisplay.text = "Score: " + currentScore;
        else Debug.LogWarning("Score Text Display not assigned!");
    }
    void UpdateTimerDisplay()
    {
        if (timerTextDisplay != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
            timerTextDisplay.text = $"Time: {timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
        else Debug.LogWarning("Timer Text Display not assigned!");
    }
    void ShowWinUI()
    {
        // Debug.Log("Showing Win UI...");
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            TextMeshProUGUI winText = winPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (winText != null)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(finalTime);
                winText.text = $"All Matched!\nTime: {timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
            }
        }
        else Debug.LogError("Win Panel not assigned!");
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }
    public void ShowLeaderboardPanel()
    {
        // Debug.Log("Showing Leaderboard Panel");
        if (leaderboardPanel == null || leaderboardEntriesText == null)
        {
            Debug.LogError("Leaderboard Panel or Text not assigned!"); return;
        }
        string entries = "";
        if (leaderboardData.times.Count == 0) entries = "No times recorded yet!";
        else
        {
            for (int i = 0; i < leaderboardData.times.Count; i++)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(leaderboardData.times[i]);
                entries += $"{i + 1}.  {timeSpan.Minutes:00}:{timeSpan.Seconds:00}\n";
            }
        }
        leaderboardEntriesText.text = entries;
        leaderboardPanel.SetActive(true);
    }
    public void HideLeaderboardPanel()
    {
        // Debug.Log("Hiding Leaderboard Panel");
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }
    #endregion

    #region Leaderboard Data Management
    // --- SaveTimeToLeaderboard, LoadLeaderboard ---
    // --- (These methods remain exactly the same as the previous version) ---
    void SaveTimeToLeaderboard(float newTime)
    {
        if (newTime <= 0) return;
        if (leaderboardData == null) leaderboardData = new LeaderboardData();
        if (leaderboardData.times == null) leaderboardData.times = new List<float>();
        leaderboardData.times.Add(newTime);
        leaderboardData.times.Sort();
        if (leaderboardData.times.Count > maxLeaderboardEntries)
        {
            leaderboardData.times = leaderboardData.times.GetRange(0, maxLeaderboardEntries);
        }
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(LeaderboardSaveKey, json);
        PlayerPrefs.Save();
        // Debug.Log($"Leaderboard saved with {leaderboardData.times.Count} entries.");
    }
    void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LeaderboardSaveKey))
        {
            string json = PlayerPrefs.GetString(LeaderboardSaveKey);
            try
            {
                leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
                if (leaderboardData == null || leaderboardData.times == null) leaderboardData = new LeaderboardData();
                leaderboardData.times.Sort();
                // Debug.Log($"Leaderboard loaded with {leaderboardData.times.Count} entries.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load/parse leaderboard: {e.Message}. Resetting.");
                leaderboardData = new LeaderboardData();
                PlayerPrefs.DeleteKey(LeaderboardSaveKey);
            }
        }
        else
        {
            leaderboardData = new LeaderboardData();
            // Debug.Log("No existing leaderboard data found.");
        }
    }
    #endregion

    #region Reset Functionality

    // Called by the Reset Button's OnClick event
    public void ResetGame()
    {
        Debug.Log("----- Resetting Game -----");
        // InitializeGame resets state AND reactivates items
        InitializeGame();
        // Start the timer again AFTER initialization is complete
        StartTimer();
    }

    // Reactivates and resets all items - Called by InitializeGame
    void ReactivateItemsAndZones()
    {
        if (colorBlocks == null || dropZones == null) { /* Error handled in Awake */ return; }
        // Debug.Log($"Reactivating {colorBlocks.Count} blocks and {dropZones.Count} zones...");

        // Reactivate and Reset Blocks
        for (int i = 0; i < colorBlocks.Count; i++)
        {
            DraggableItem block = colorBlocks[i];
            if (block != null && block.gameObject != null)
            {
                block.gameObject.SetActive(true);
                RectTransform blockRect = block.rectTransform; // Use cached reference
                if (blockRect != null) blockRect.anchoredPosition = block.startAnchoredPosition;
                block.transform.localScale = block.startScale;

                // CRITICAL CHECK: parentAfterDrag should now be set because DraggableItem.Awake ran before Start/InitializeGame
                if (block.parentAfterDrag != null)
                {
                    block.transform.SetParent(block.parentAfterDrag, true);
                }
                else
                {
                    // If this still happens, there's a deeper issue (script disabled? hierarchy problem?)
                    Debug.LogError($"Block {block.name} STILL missing stored parent (parentAfterDrag) even after moving init to Start! Check DraggableItem Awake/Hierarchy.", block.gameObject);
                }

                if (block.image != null) { block.image.enabled = true; block.image.raycastTarget = true; }
                // Debug.Log($"Reactivated block: {block.name}");
            }
            else { Debug.LogError($"NULL entry at index {i} in 'Color Blocks' list!"); }
        }

        // Reactivate Drop Zones
        for (int i = 0; i < dropZones.Count; i++)
        {
            DropZone zone = dropZones[i];
            if (zone != null && zone.gameObject != null)
            {
                zone.gameObject.SetActive(true);
                var textComponent = zone.GetComponentInChildren<TMP_Text>(true);
                if (textComponent != null) textComponent.raycastTarget = true;
                else
                {
                    var imageComponent = zone.GetComponentInChildren<Image>(true);
                    if (imageComponent != null) imageComponent.raycastTarget = true;
                    // else Debug.LogWarning($"Cannot find Raycast Target on {zone.name}");
                }
                // Debug.Log($"Reactivated zone: {zone.name}");
            }
            else { Debug.LogError($"NULL entry at index {i} in 'Drop Zones' list!"); }
        }
    }
    #endregion

    #region Audio Playback Methods
    // --- PlayCorrectSound, PlayIncorrectSound ---
    // --- (These methods remain exactly the same as the previous version) ---
    public void PlayCorrectSound()
    {
        if (correctSound != null && audioSource != null && audioSource.isActiveAndEnabled) audioSource.PlayOneShot(correctSound);
        else if (correctSound == null) Debug.LogWarning("Correct Sound missing.");
    }
    public void PlayIncorrectSound()
    {
        if (incorrectSound != null && audioSource != null && audioSource.isActiveAndEnabled) audioSource.PlayOneShot(incorrectSound);
        else if (incorrectSound == null) Debug.LogWarning("Incorrect Sound missing.");
    }
    #endregion
}