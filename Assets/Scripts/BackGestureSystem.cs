using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

[Serializable]
public class SceneNavigationRule
{
    public string sceneName;
    public string destinationSceneName;
    [Tooltip("If true, will navigate to destinationSceneName. If false, will use normal back history.")]
    public bool overrideBackNavigation = false;
}

public class BackGestureSystem : MonoBehaviour
{
    [Header("Gesture Settings")]
    [SerializeField] private float minSwipeDistance = 50f;
    [SerializeField] private float maxSwipeTime = 0.5f;
    [SerializeField] private float edgeThreshold = 50f; // Distance from the edge to start gesture

    [Header("Visual Feedback")]
    [SerializeField] private RectTransform gestureIndicator;
    [SerializeField] private CanvasGroup indicatorCanvasGroup;
    [SerializeField] private float maxIndicatorAlpha = 0.5f;

    [Header("Navigation Settings")]
    [SerializeField] private string defaultScene = "MainMenu";
    [SerializeField] private bool enableBackGesture = true;
    [SerializeField] private List<SceneNavigationRule> sceneNavigationRules = new List<SceneNavigationRule>();

    // Navigation history
    private Stack<string> navigationHistory = new Stack<string>();

    // Gesture tracking variables
    private Vector2 startPos;
    private float startTime;
    private bool isTrackingSwipe = false;
    private bool isShowingIndicator = false;

    // Make this a singleton that persists between scenes
    public static BackGestureSystem Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize indicator if present
            if (indicatorCanvasGroup != null)
            {
                indicatorCanvasGroup.alpha = 0f;
            }

            // Register for scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Unregister event when destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // You can perform scene-specific setup here
        Debug.Log($"Scene loaded: {scene.name}");
    }

    private void Update()
    {
        if (enableBackGesture)
        {
            DetectBackGesture();
        }
    }

    private void DetectBackGesture()
    {
        // Handle touch input (mobile devices)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Only register swipes starting from the left edge
                    if (touch.position.x <= edgeThreshold)
                    {
                        startPos = touch.position;
                        startTime = Time.time;
                        isTrackingSwipe = true;
                        isShowingIndicator = true;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isTrackingSwipe)
                    {
                        // Calculate and show visual feedback
                        float progress = Mathf.Clamp01((touch.position.x - startPos.x) / (Screen.width * 0.4f));
                        UpdateGestureIndicator(progress);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isTrackingSwipe)
                    {
                        float swipeTime = Time.time - startTime;
                        float swipeDistance = touch.position.x - startPos.x;

                        // Hide indicator
                        if (isShowingIndicator)
                        {
                            UpdateGestureIndicator(0f);
                            isShowingIndicator = false;
                        }

                        // Check if swipe meets the criteria for a back gesture
                        if (swipeTime < maxSwipeTime && swipeDistance > minSwipeDistance)
                        {
                            HandleBackGesture();
                        }

                        isTrackingSwipe = false;
                    }
                    break;
            }
        }

        // Handle mouse input (for testing in editor)
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x <= edgeThreshold)
        {
            startPos = Input.mousePosition;
            startTime = Time.time;
            isTrackingSwipe = true;
            isShowingIndicator = true;
        }
        else if (Input.GetMouseButton(0) && isTrackingSwipe)
        {
            // Calculate and show visual feedback
            float progress = Mathf.Clamp01((Input.mousePosition.x - startPos.x) / (Screen.width * 0.4f));
            UpdateGestureIndicator(progress);
        }
        else if (Input.GetMouseButtonUp(0) && isTrackingSwipe)
        {
            float swipeTime = Time.time - startTime;
            float swipeDistance = Input.mousePosition.x - startPos.x;

            // Hide indicator
            if (isShowingIndicator)
            {
                UpdateGestureIndicator(0f);
                isShowingIndicator = false;
            }

            if (swipeTime < maxSwipeTime && swipeDistance > minSwipeDistance)
            {
                HandleBackGesture();
            }

            isTrackingSwipe = false;
        }

        // Also handle hardware back button (Android)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackGesture();
        }
    }

    private void UpdateGestureIndicator(float progress)
    {
        if (gestureIndicator != null && indicatorCanvasGroup != null)
        {
            // Show indicator with fade based on progress
            indicatorCanvasGroup.alpha = progress * maxIndicatorAlpha;

            // Optionally animate the indicator position/scale
            gestureIndicator.anchoredPosition = new Vector2(progress * 100f, 0);
        }
    }

    private void HandleBackGesture()
    {
        Debug.Log("Back gesture detected!");

        // Check if there's a special rule for the current scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneNavigationRule rule = sceneNavigationRules.Find(r => r.sceneName == currentScene);

        if (rule != null && rule.overrideBackNavigation)
        {
            // Follow the custom rule
            Debug.Log($"Using custom navigation rule: going to {rule.destinationSceneName}");
            SceneManager.LoadScene(rule.destinationSceneName);
        }
        else
        {
            // Use standard back navigation
            GoBack();
        }
    }

    // --- Navigation Management Methods ---

    /// <summary>
    /// Navigate to a specific scene, adding current scene to history
    /// </summary>
    public void NavigateTo(string screenName)
    {
        // Add current screen to history before navigating
        if (SceneManager.GetActiveScene().name != screenName)
        {
            navigationHistory.Push(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(screenName);
        }
    }

    /// <summary>
    /// Navigate to a specific scene without adding to navigation history
    /// </summary>
    public void NavigateToWithoutHistory(string screenName)
    {
        if (SceneManager.GetActiveScene().name != screenName)
        {
            SceneManager.LoadScene(screenName);
        }
    }

    /// <summary>
    /// Go back to the previous scene in history
    /// </summary>
    public void GoBack()
    {
        if (navigationHistory.Count > 0)
        {
            string previousScreen = navigationHistory.Pop();
            SceneManager.LoadScene(previousScreen);
        }
        else
        {
            // No history available, handle exit or show main menu
            HandleNoBackDestination();
        }
    }

    private void HandleNoBackDestination()
    {
        // Check if we're already at the default scene
        if (SceneManager.GetActiveScene().name != defaultScene)
        {
            // Go to default scene (usually main menu)
            SceneManager.LoadScene(defaultScene);
        }
        else
        {
            // We're already at the default scene, show exit dialog
            ShowExitDialog();
        }
    }

    private void ShowExitDialog()
    {
        Debug.Log("Showing exit confirmation dialog");
        // You would implement your own exit dialog UI here
        // For now, just log it

#if UNITY_ANDROID && !UNITY_EDITOR
        // Example exit dialog implementation
        // Replace with your own UI system
        /*
        ExitDialogUI.Instance.Show(
            onConfirm: () => Application.Quit(),
            onCancel: () => ExitDialogUI.Instance.Hide()
        );
        */
#endif
    }

    /// <summary>
    /// Helper method to clear navigation history (e.g., when going to main menu)
    /// </summary>
    public void ClearNavigationHistory()
    {
        navigationHistory.Clear();
    }

    /// <summary>
    /// Add a temporary navigation rule at runtime
    /// </summary>
    public void AddNavigationRule(string fromScene, string toScene, bool overrideBack = true)
    {
        // Remove any existing rule for this scene
        sceneNavigationRules.RemoveAll(r => r.sceneName == fromScene);

        // Add the new rule
        SceneNavigationRule newRule = new SceneNavigationRule
        {
            sceneName = fromScene,
            destinationSceneName = toScene,
            overrideBackNavigation = overrideBack
        };

        sceneNavigationRules.Add(newRule);
    }

    /// <summary>
    /// Remove a navigation rule
    /// </summary>
    public void RemoveNavigationRule(string sceneName)
    {
        sceneNavigationRules.RemoveAll(r => r.sceneName == sceneName);
    }

    /// <summary>
    /// Enable or disable back gesture detection
    /// </summary>
    public void SetBackGestureEnabled(bool enabled)
    {
        enableBackGesture = enabled;
    }
}