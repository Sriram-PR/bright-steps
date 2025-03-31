//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class ReactionGameManager : MonoBehaviour
//{
//    // Game states
//    private enum GameState
//    {
//        Initial,
//        Countdown,
//        WaitForReaction,
//        ShowResult
//    }

//    // UI Elements - Assign these in the Inspector
//    public Image backgroundPanel;
//    public TextMeshProUGUI countdownText;
//    public TextMeshProUGUI resultText;
//    public Button screenButton;

//    // Colors for different states
//    public Color initialColor = Color.blue;
//    public Color reactionColor = Color.green;
//    public Color resultColor = Color.yellow;

//    // Private variables
//    private GameState currentState;
//    private float reactionStartTime;
//    private float reactionTime;

//    void Start()
//    {
//        // Set initial state
//        countdownText.gameObject.SetActive(true);
//        resultText.gameObject.SetActive(false);

//        // Set initial background color
//        backgroundPanel.color = initialColor;

//        // Disable clicking initially
//        screenButton.interactable = false;

//        // Add listener to the screen button
//        screenButton.onClick.AddListener(HandleScreenClick);

//        // Start the game
//        StartCoroutine(StartGame());
//    }

//    private IEnumerator StartGame()
//    {
//        // Re-enable countdown text and reset background if necessary
//        countdownText.gameObject.SetActive(true);
//        backgroundPanel.color = initialColor;

//        // Set state to countdown
//        currentState = GameState.Countdown;

//        // Display countdown: 3
//        countdownText.text = "3";
//        yield return new WaitForSeconds(1f);

//        // Display countdown: 2
//        countdownText.text = "2";
//        yield return new WaitForSeconds(1f);

//        // Display countdown: 1
//        countdownText.text = "1";
//        yield return new WaitForSeconds(1f);

//        // Hide countdown text
//        countdownText.gameObject.SetActive(false);

//        // Change color to signal reaction time
//        backgroundPanel.color = reactionColor;

//        // Enable clicking
//        screenButton.interactable = true;

//        // Record start time for reaction
//        reactionStartTime = Time.time;

//        // Change state to wait for reaction
//        currentState = GameState.WaitForReaction;
//    }

//    private void HandleScreenClick()
//    {
//        if (currentState == GameState.WaitForReaction)
//        {
//            // Calculate reaction time in milliseconds
//            reactionTime = (Time.time - reactionStartTime) * 1000f;

//            // Disable clicking again
//            screenButton.interactable = false;

//            // Change background color
//            backgroundPanel.color = resultColor;

//            // Show result
//            resultText.gameObject.SetActive(true);
//            resultText.text = $"Your reaction time: {reactionTime:F0} ms\n\nTap anywhere to try again";

//            // Change state to show result
//            currentState = GameState.ShowResult;

//            // Add delay before allowing restart
//            StartCoroutine(EnableRestart());
//        }
//        else if (currentState == GameState.ShowResult)
//        {
//            // Hide result text
//            resultText.gameObject.SetActive(false);

//            // Restart game
//            StartCoroutine(StartGame());
//        }
//    }

//    private IEnumerator EnableRestart()
//    {
//        // Wait a moment before allowing restart to prevent accidental taps
//        yield return new WaitForSeconds(1f);

//        // Enable clicking for restart
//        screenButton.interactable = true;
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReactionGameManager : MonoBehaviour
{
    // Game states
    private enum GameState
    {
        Initial,
        Countdown,
        WaitForReaction,
        ShowResult
    }

    // UI Elements for main game
    public Image backgroundPanel;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI resultText;
    public Button screenButton;

    // UI Elements for leaderboard
    public Button leaderboardButton;
    public GameObject leaderboardPanel;
    public Transform leaderboardContent;
    public GameObject scoreEntryPrefab;
    public Button closeLeaderboardButton;
    public Button resetLeaderboardButton;

    // Colors for different states
    public Color initialColor = Color.blue;
    public Color reactionColor = Color.green;
    public Color resultColor = Color.yellow;

    // Game variables
    private GameState currentState;
    private float reactionStartTime;
    private float reactionTime;

    // Leaderboard variables
    private List<float> reactionTimes = new List<float>();
    private const int MAX_SCORES = 10;
    private const string PREFS_KEY = "ReactionTimeScores";

    void Start()
    {
        // Initialize main game UI
        countdownText.gameObject.SetActive(true);
        resultText.gameObject.SetActive(false);
        backgroundPanel.color = initialColor;

        // Initialize leaderboard UI
        leaderboardButton.gameObject.SetActive(false);
        leaderboardPanel.SetActive(false);

        // Set up button listeners
        screenButton.interactable = false;
        screenButton.onClick.AddListener(HandleScreenClick);
        leaderboardButton.onClick.AddListener(ShowLeaderboard);
        closeLeaderboardButton.onClick.AddListener(CloseLeaderboard);
        resetLeaderboardButton.onClick.AddListener(ResetLeaderboard);

        // Load saved scores
        LoadScores();

        // Start the game
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        // Re-enable countdown text and reset background
        countdownText.gameObject.SetActive(true);
        backgroundPanel.color = initialColor;

        // Set state to countdown
        currentState = GameState.Countdown;

        // Display countdown: 3
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        // Display countdown: 2
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        // Display countdown: 1
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        // Hide countdown text
        countdownText.gameObject.SetActive(false);

        // Change color to signal reaction time
        backgroundPanel.color = reactionColor;

        // Enable clicking
        screenButton.interactable = true;

        // Record start time for reaction
        reactionStartTime = Time.time;

        // Change state to wait for reaction
        currentState = GameState.WaitForReaction;
    }

    private void HandleScreenClick()
    {
        if (currentState == GameState.WaitForReaction)
        {
            // Calculate reaction time in milliseconds
            reactionTime = (Time.time - reactionStartTime) * 1000f;

            // Disable clicking again
            screenButton.interactable = false;

            // Change background color
            backgroundPanel.color = resultColor;

            // Show result
            resultText.gameObject.SetActive(true);
            resultText.text = $"Your reaction time: {reactionTime:F0} ms\n\nTap anywhere to try again";

            // Add score to leaderboard
            AddScore(reactionTime);

            // Show leaderboard button
            leaderboardButton.gameObject.SetActive(true);

            // Change state to show result
            currentState = GameState.ShowResult;

            // Add delay before allowing restart
            StartCoroutine(EnableRestart());
        }
        else if (currentState == GameState.ShowResult)
        {
            // Hide result text and leaderboard button
            resultText.gameObject.SetActive(false);
            leaderboardButton.gameObject.SetActive(false);

            // Restart game
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator EnableRestart()
    {
        // Wait a moment before allowing restart
        yield return new WaitForSeconds(1f);

        // Enable clicking for restart
        screenButton.interactable = true;
    }

    // Leaderboard methods
    private void AddScore(float score)
    {
        // Add the new score
        reactionTimes.Add(score);

        // Sort scores (lower is better for reaction time)
        reactionTimes.Sort();

        // Limit to top scores
        if (reactionTimes.Count > MAX_SCORES)
        {
            reactionTimes.RemoveRange(MAX_SCORES, reactionTimes.Count - MAX_SCORES);
        }

        // Save updated scores
        SaveScores();
    }

    private void SaveScores()
    {
        // Convert list to comma-separated string
        string scoreString = "";
        foreach (float score in reactionTimes)
        {
            scoreString += score.ToString() + ",";
        }

        // Save to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, scoreString);
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        // Clear existing scores
        reactionTimes.Clear();

        // Get saved scores string
        string scoreString = PlayerPrefs.GetString(PREFS_KEY, "");

        // Parse scores if available
        if (!string.IsNullOrEmpty(scoreString))
        {
            string[] scores = scoreString.Split(',');

            foreach (string score in scores)
            {
                if (!string.IsNullOrEmpty(score))
                {
                    if (float.TryParse(score, out float parsedScore))
                    {
                        reactionTimes.Add(parsedScore);
                    }
                }
            }

            // Make sure scores are sorted
            reactionTimes.Sort();
        }
    }

    private void ShowLeaderboard()
    {
        // Show the panel
        leaderboardPanel.SetActive(true);

        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Create entries for each score
        for (int i = 0; i < reactionTimes.Count; i++)
        {
            // Create score entry from prefab
            GameObject entry = Instantiate(scoreEntryPrefab, leaderboardContent);

            // Get text components
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                // Set rank and score text
                texts[0].text = (i + 1).ToString();
                texts[1].text = $"{reactionTimes[i]:F0} ms";

                // Highlight best score (first entry)
                if (i == 0)
                {
                    texts[0].color = Color.green;
                    texts[1].color = Color.green;
                    texts[1].fontStyle = FontStyles.Bold;
                }
            }
        }
    }

    public void CloseLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    public void ResetLeaderboard()
    {
        // Clear the scores list
        reactionTimes.Clear();

        // Delete the saved scores from PlayerPrefs
        PlayerPrefs.DeleteKey(PREFS_KEY);
        PlayerPrefs.Save();

        // If the leaderboard is currently visible, refresh it
        if (leaderboardPanel.activeSelf)
        {
            // Re-show the empty leaderboard
            ShowLeaderboard();
        }
    }
}