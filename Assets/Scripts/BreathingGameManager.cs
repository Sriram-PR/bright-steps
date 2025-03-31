using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreathingGameManager : MonoBehaviour
{
    // Breathing states
    public enum BreathingState
    {
        Ready,      // Initial state before starting
        Inhale,     // Breathe in
        Hold,       // Hold breath
        Exhale,     // Breathe out
        Rest,       // Pause between cycles (optional)
        Complete    // Session completed
    }

    // Breathing patterns
    public enum BreathingPattern
    {
        BoxBreathing,   // 4-4-4-4
        RelaxingBreath   // 4-7-8
    }

    // UI Elements
    [Header("Main UI Elements")]
    public Image backgroundPanel;
    public Image breathingCircle;
    public Image progressRing;        // Circular timer for current phase
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI sessionTimerText;
    public TextMeshProUGUI countdownText;  // Text to display seconds countdown
    public Button backButton;

    [Header("Settings UI")]
    public GameObject settingsPanel;
    public Button[] patternButtons; // 0=BoxBreathing, 1=RelaxingBreath
    public Button[] sessionLengthButtons; // 0=1min, 1=3min, 2=5min, 3=10min
    public Button startButton;
    public Button settingsButton;

    [Header("Colors")]
    public Color inhaleColor = new Color(0.5f, 0.8f, 1f); // Light blue
    public Color holdColor = new Color(1f, 0.9f, 0.5f);   // Light yellow
    public Color exhaleColor = new Color(0.5f, 0.9f, 0.5f); // Light green
    public Color restColor = new Color(0.7f, 0.7f, 0.9f);   // Light purple
    public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f); // Dark blue-gray

    [Header("Breathing Patterns")]
    // Box Breathing pattern (4-4-4-6)
    private float[] boxBreathingDurations = new float[] { 4f, 4f, 4f, 6f };
    // 4-7-8 Breathing pattern with 6-second rest
    private float[] relaxingBreathDurations = new float[] { 4f, 7f, 8f, 6f };

    // Session lengths in minutes
    private float[] sessionLengths = new float[] { 1f, 3f, 5f, 10f };

    // Game variables
    private BreathingState currentState = BreathingState.Ready;
    private BreathingPattern currentPattern = BreathingPattern.BoxBreathing;
    private int selectedSessionIndex = 0; // Default to 1 minute

    private float[] currentDurations; // Will be set based on pattern
    private float sessionTimeRemaining;
    private float currentPhaseTimeRemaining;
    private int cyclesCompleted = 0;

    // Animation variables
    private Vector3 circleMinSize;
    private Vector3 circleMaxSize;
    private Vector3 targetSize;

    private void Start()
    {
        // Initialize UI
        InitializeUI();

        // Set initial pattern
        SetBreathingPattern(BreathingPattern.BoxBreathing);

        // Make sure progress ring is larger than the breathing circle
        EnsureProgressRingSizing();

        // Setup circle sizes
        SetupCircleSizes();

        // Set up button listeners
        SetupButtonListeners();

        // Start in settings mode
        ShowSettings(true);
    }

    private void EnsureProgressRingSizing()
    {
        // Make sure progress ring is appropriately sized compared to breathing circle
        if (progressRing != null && breathingCircle != null)
        {
            // Get the current sizes
            RectTransform breathingRT = breathingCircle.GetComponent<RectTransform>();
            RectTransform progressRT = progressRing.GetComponent<RectTransform>();

            if (breathingRT != null && progressRT != null)
            {
                // Make progress ring 20% larger than the breathing circle base size
                float breathingSize = breathingRT.sizeDelta.x;
                float targetProgressSize = breathingSize * 1.2f;

                progressRT.sizeDelta = new Vector2(targetProgressSize, targetProgressSize);

                // Ensure progress ring is behind breathing circle in hierarchy
                progressRing.transform.SetSiblingIndex(0);
                breathingCircle.transform.SetSiblingIndex(1);
            }
        }
    }

    private void InitializeUI()
    {
        // Make sure all UI elements are properly set

        // Hide instruction text initially when in settings mode
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }

        // Hide countdown text initially
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        sessionTimerText.gameObject.SetActive(false);

        // Initially hide the breathing circle panel
        if (breathingCircle != null && breathingCircle.transform.parent != null)
        {
            breathingCircle.transform.parent.gameObject.SetActive(false);
        }

        breathingCircle.transform.localScale = Vector3.one; // Reset scale
        backgroundPanel.color = backgroundColor;

        // Make sure settings panel is centered
        if (settingsPanel != null)
        {
            RectTransform settingsRT = settingsPanel.GetComponent<RectTransform>();
            if (settingsRT != null)
            {
                // Center the settings panel
                settingsRT.anchorMin = new Vector2(0.5f, 0.5f);
                settingsRT.anchorMax = new Vector2(0.5f, 0.5f);
                settingsRT.pivot = new Vector2(0.5f, 0.5f);
                settingsRT.anchoredPosition = Vector2.zero;
            }
        }
    }

    private void SetupCircleSizes()
    {
        // Define the min and max sizes for the breathing circle
        // Min size is for exhale, max size is for inhale completion
        // Make sure the max size doesn't exceed the progress ring

        // The breathing circle should be smaller than the progress ring even at max size
        // Progress ring is typically 450x450, so max breathing circle should be smaller
        circleMinSize = new Vector3(0.6f, 0.6f, 1f);
        circleMaxSize = new Vector3(0.9f, 0.9f, 1f); // Reduced from 1.5f to stay within progress ring

        // Initially set to the min size
        breathingCircle.transform.localScale = circleMinSize;
        targetSize = circleMinSize;
    }

    private void SetupButtonListeners()
    {
        // Set up pattern button listeners
        for (int i = 0; i < patternButtons.Length; i++)
        {
            int index = i; // Required for closure
            patternButtons[i].onClick.AddListener(() => SelectBreathingPattern(index));
        }

        // Set up session length button listeners
        for (int i = 0; i < sessionLengthButtons.Length; i++)
        {
            int index = i; // Required for closure
            sessionLengthButtons[i].onClick.AddListener(() => SelectSessionLength(index));
        }

        // Start button listener
        startButton.onClick.AddListener(StartBreathingSession);

        // Settings button
        settingsButton.onClick.AddListener(() => ShowSettings(true));

        // Back button listener
        backButton.onClick.AddListener(ReturnToMenu);
    }

    private void SelectBreathingPattern(int index)
    {
        // Set visual indicator for selected button
        for (int i = 0; i < patternButtons.Length; i++)
        {
            patternButtons[i].GetComponent<Image>().color = (i == index) ?
                Color.white : new Color(0.8f, 0.8f, 0.8f);
        }

        // Set pattern based on selection
        currentPattern = (BreathingPattern)index;
        SetBreathingPattern(currentPattern);
    }

    private void SelectSessionLength(int index)
    {
        // Set visual indicator for selected button
        for (int i = 0; i < sessionLengthButtons.Length; i++)
        {
            sessionLengthButtons[i].GetComponent<Image>().color = (i == index) ?
                Color.white : new Color(0.8f, 0.8f, 0.8f);
        }

        // Store selected session length index
        selectedSessionIndex = index;
    }

    private void SetBreathingPattern(BreathingPattern pattern)
    {
        // Set durations based on selected pattern
        switch (pattern)
        {
            case BreathingPattern.BoxBreathing:
                currentDurations = boxBreathingDurations;
                break;
            case BreathingPattern.RelaxingBreath:
                currentDurations = relaxingBreathDurations;
                break;
        }
    }

    private void StartBreathingSession()
    {
        // Hide settings panel
        ShowSettings(false);

        // Show session timer
        sessionTimerText.gameObject.SetActive(true);

        // Show instruction text
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true);
            instructionText.text = "Ready...";
        }

        // Show countdown text
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "";
        }

        // Activate the breathing circle panel
        if (breathingCircle != null && breathingCircle.transform.parent != null)
        {
            breathingCircle.transform.parent.gameObject.SetActive(true);
        }

        // Calculate total session time
        sessionTimeRemaining = sessionLengths[selectedSessionIndex] * 60f; // Convert minutes to seconds

        // Update timer text immediately with new time
        UpdateSessionTimerText();

        // Reset cycle count
        cyclesCompleted = 0;

        // Start the breathing sequence with a countdown
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        // Show countdown: 3
        instructionText.text = "Starting in 3...";

        // Hide the countdown text during initial countdown
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(0.1f); // Short wait to ensure UI updates

        // Start countdown without affecting the session timer
        float countdownTime = 3f;
        float startTime = Time.time;

        while (countdownTime > 0)
        {
            countdownTime = 3f - (Time.time - startTime);

            // DO NOT decrement session time during countdown
            // sessionTimeRemaining should remain unchanged until actual exercise begins

            // Update countdown text
            int countdownValue = Mathf.CeilToInt(countdownTime);
            if (countdownValue >= 1 && countdownValue <= 3)
            {
                instructionText.text = "Starting in " + countdownValue + "...";
                // Don't update the countdown text in circle during initial countdown
            }

            yield return null;
        }

        // Start first breath cycle
        StartCoroutine(BeginBreathingCycle());
    }

    private IEnumerator BeginBreathingCycle()
    {
        // The actual breathing exercise starts here, after the countdown
        // Now we can start decrementing the session timer

        while (sessionTimeRemaining > 0)
        {
            // Inhale phase
            yield return StartCoroutine(BreathingPhase(BreathingState.Inhale));
            if (sessionTimeRemaining <= 0) break;

            // Hold phase
            yield return StartCoroutine(BreathingPhase(BreathingState.Hold));
            if (sessionTimeRemaining <= 0) break;

            // Exhale phase
            yield return StartCoroutine(BreathingPhase(BreathingState.Exhale));
            if (sessionTimeRemaining <= 0) break;

            // Rest phase (optional, only used in Box Breathing)
            if (currentDurations[3] > 0)
            {
                yield return StartCoroutine(BreathingPhase(BreathingState.Rest));
                if (sessionTimeRemaining <= 0) break;
            }

            // No pause or reset between cycles - flows continuously
            cyclesCompleted++;
        }

        // Session completed
        CompleteSession();
    }

    private IEnumerator BreathingPhase(BreathingState phase)
    {
        // Set current state
        currentState = phase;

        // Determine phase duration
        float phaseDuration = 0f;
        Color phaseColor = backgroundColor;
        string phaseText = "";
        Vector3 phaseTargetSize = circleMinSize;

        // Configure based on phase
        switch (phase)
        {
            case BreathingState.Inhale:
                phaseDuration = currentDurations[0];
                phaseColor = inhaleColor;
                phaseText = "Breathe In";
                phaseTargetSize = circleMaxSize;
                break;
            case BreathingState.Hold:
                phaseDuration = currentDurations[1];
                phaseColor = holdColor;
                phaseText = "Hold";
                phaseTargetSize = circleMaxSize; // Same as inhale end size
                break;
            case BreathingState.Exhale:
                phaseDuration = currentDurations[2];
                phaseColor = exhaleColor;
                phaseText = "Breathe Out";
                phaseTargetSize = circleMinSize;
                break;
            case BreathingState.Rest:
                phaseDuration = currentDurations[3];
                phaseColor = restColor;
                phaseText = "Rest";
                phaseTargetSize = circleMinSize; // Same as exhale end size
                break;
        }

        // Set instruction text
        instructionText.text = phaseText;

        // Set target size for animation
        targetSize = phaseTargetSize;

        // Set UI color - use Lerp for smooth color transitions between phases
        backgroundPanel.color = Color.Lerp(backgroundPanel.color, Color.Lerp(backgroundColor, phaseColor, 0.3f), 0.3f);
        breathingCircle.color = Color.Lerp(breathingCircle.color, phaseColor, 0.3f);

        // Initialize phase timer
        currentPhaseTimeRemaining = phaseDuration;
        float startTime = Time.time;

        // Ensure countdown text is visible for this phase
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            // Set initial countdown value for this phase
            countdownText.text = Mathf.CeilToInt(phaseDuration).ToString();

            // Use white text color for better visibility against all backgrounds
            countdownText.color = Color.white;

            // Make text larger and bold
            countdownText.fontStyle = TMPro.FontStyles.Bold;

            // Make sure text is in front
            countdownText.transform.SetAsLastSibling();
        }

        while (currentPhaseTimeRemaining > 0 && sessionTimeRemaining > 0)
        {
            // Update timers
            float elapsed = Time.time - startTime;
            currentPhaseTimeRemaining = phaseDuration - elapsed;
            sessionTimeRemaining -= Time.deltaTime;

            // Update progress ring (if used)
            if (progressRing != null)
            {
                progressRing.fillAmount = currentPhaseTimeRemaining / phaseDuration;
            }

            // Update countdown text in the center of the circle
            if (countdownText != null)
            {
                // Display seconds remaining as a whole number
                int secondsRemaining = Mathf.CeilToInt(currentPhaseTimeRemaining);
                countdownText.text = secondsRemaining.ToString();
            }

            // Update session timer text
            UpdateSessionTimerText();

            // Calculate progress through this phase (0 to 1)
            float phaseProgress = elapsed / phaseDuration;

            // Animate the breathing circle based on phase
            AnimateBreathingCircle(phase, phaseProgress);

            yield return null;
        }

        // No additional wait - immediately ready for next phase
    }

    private void AnimateBreathingCircle(BreathingState phase, float progress)
    {
        Vector3 startSize = breathingCircle.transform.localScale;
        Vector3 newSize = Vector3.zero;

        switch (phase)
        {
            case BreathingState.Inhale:
                // Gradually expand from min to max size
                newSize = Vector3.Lerp(circleMinSize, circleMaxSize, progress);
                break;
            case BreathingState.Hold:
                // Stay at max size
                newSize = circleMaxSize;
                break;
            case BreathingState.Exhale:
                // Gradually shrink from max to min size
                newSize = Vector3.Lerp(circleMaxSize, circleMinSize, progress);
                break;
            case BreathingState.Rest:
                // Stay at min size
                newSize = circleMinSize;
                break;
            default:
                newSize = startSize;
                break;
        }

        // Apply the new size
        breathingCircle.transform.localScale = newSize;
    }

    private void UpdateSessionTimerText()
    {
        // Convert seconds to minutes:seconds format
        int minutes = Mathf.FloorToInt(sessionTimeRemaining / 60f);
        int seconds = Mathf.FloorToInt(sessionTimeRemaining % 60f);
        sessionTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void CompleteSession()
    {
        // Set completed state
        currentState = BreathingState.Complete;

        // Update UI
        backgroundPanel.color = backgroundColor;
        instructionText.text = "Well Done!\n\nPress Settings to try again";
        breathingCircle.transform.localScale = circleMinSize;

        // Show settings button
        settingsButton.gameObject.SetActive(true);
    }

    private void ShowSettings(bool show)
    {
        // Toggle settings panel
        settingsPanel.SetActive(show);

        // Toggle session elements
        sessionTimerText.gameObject.SetActive(!show);
        settingsButton.gameObject.SetActive(!show);

        // When showing settings, hide the instruction text
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(!show);
        }

        // When showing settings, hide the countdown text
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(!show);
        }

        // When showing settings, hide the breathing circle panel
        if (breathingCircle != null && breathingCircle.transform.parent != null)
        {
            breathingCircle.transform.parent.gameObject.SetActive(!show);
        }

        // If showing settings, stop any ongoing session and reset everything
        if (show)
        {
            // Stop all coroutines to halt any ongoing breathing cycle
            StopAllCoroutines();

            // Reset state
            currentState = BreathingState.Ready;
            cyclesCompleted = 0;

            // Reset UI (prepare text, but it won't be visible until exercise starts)
            if (instructionText != null)
            {
                instructionText.text = "Ready...";
            }

            // Clear countdown text
            if (countdownText != null)
            {
                countdownText.text = "";
            }

            breathingCircle.transform.localScale = circleMinSize;
            backgroundPanel.color = backgroundColor;
            breathingCircle.color = inhaleColor; // Reset to default color

            // Reset the session timer
            sessionTimeRemaining = 0;
            UpdateSessionTimerText();

            // Reset progress ring if used
            if (progressRing != null)
            {
                progressRing.fillAmount = 1f;
            }

            // Force immediate update of circle size
            targetSize = circleMinSize;
            breathingCircle.transform.localScale = circleMinSize;
        }
    }

    private void ReturnToMenu()
    {
        // Stop all coroutines
        StopAllCoroutines();

        // Load the menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    private void Update()
    {
        // Animate toward target size for smoother transitions
        breathingCircle.transform.localScale = Vector3.Lerp(
            breathingCircle.transform.localScale,
            targetSize,
            Time.deltaTime * 3f
        );
    }
}