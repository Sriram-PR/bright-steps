using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Cell State")]
    public int xCoord; // X coordinate in the grid
    public int yCoord; // Y coordinate in the grid
    public bool isEndpoint = false; // Is this cell a starting/ending dot?
    public Color endpointColor = Color.clear; // Color of the endpoint dot, if applicable
    public Color pathColor = Color.clear; // Color of the path currently drawn on this cell
    // We might add more state later, like which path ID it belongs to

    [Header("Visual References")]
    // Assign these in the Prefab Inspector
    public SpriteRenderer mainSpriteRenderer; // The main background/path visual
    public SpriteRenderer dotSpriteRenderer;  // The visual for the endpoint dot

    private Color defaultBackgroundColor = Color.white; // Or choose another default

    void Awake()
    {
        // Get component references if not assigned in Inspector (optional, but good practice)
        if (mainSpriteRenderer == null)
        {
            mainSpriteRenderer = GetComponent<SpriteRenderer>();
            if (mainSpriteRenderer == null)
            {
                Debug.LogError($"Cell ({name}) is missing its Main Sprite Renderer!", this);
            }
        }
        if (dotSpriteRenderer == null)
        {
            // Assuming dot is on a child object named "DotVisual"
            Transform dotTransform = transform.Find("DotVisual");
            if (dotTransform != null)
            {
                dotSpriteRenderer = dotTransform.GetComponent<SpriteRenderer>();
            }
            if (dotSpriteRenderer == null)
            {
                Debug.LogWarning($"Cell ({name}) could not find its Dot Sprite Renderer! Ensure a child named 'DotVisual' with a SpriteRenderer exists.", this);
            }
        }

        // Initialize visuals
        if (mainSpriteRenderer != null)
        {
            mainSpriteRenderer.color = defaultBackgroundColor;
        }
        if (dotSpriteRenderer != null)
        {
            dotSpriteRenderer.enabled = false; // Hide dot initially
            dotSpriteRenderer.color = Color.clear;
        }
    }

    // Call this from GridManager after creating the cell
    public void Initialize(int x, int y, Color defaultBgColor)
    {
        xCoord = x;
        yCoord = y;
        defaultBackgroundColor = defaultBgColor;
        if (mainSpriteRenderer != null)
        {
            mainSpriteRenderer.color = defaultBackgroundColor;
        }
        isEndpoint = false;
        endpointColor = Color.clear;
        pathColor = Color.clear;

        // Ensure dot is hidden on initialization
        if (dotSpriteRenderer != null)
        {
            dotSpriteRenderer.enabled = false;
        }
    }

    // Call this from Level Loader/GridManager to mark an endpoint
    public void SetAsEndpoint(Color color)
    {
        isEndpoint = true;
        endpointColor = color;
        pathColor = Color.clear; // Endpoints start with no path drawn on them yet

        if (dotSpriteRenderer != null)
        {
            dotSpriteRenderer.enabled = true;
            dotSpriteRenderer.color = color;
            // Ensure endpoint dot draws on top of any potential path later
            // You might adjust sorting layers/order here if needed
            // dotSpriteRenderer.sortingOrder = 1; // Example
            // mainSpriteRenderer.sortingOrder = 0; // Example
        }
        else
        {
            // Fallback: If no dot renderer, color the main sprite (less ideal)
            if (mainSpriteRenderer != null)
                mainSpriteRenderer.color = color;
            Debug.LogWarning($"Cell ({name}) is an endpoint but has no Dot Sprite Renderer assigned. Coloring main sprite.", this);
        }
    }

    // Call this when drawing a path
    public void SetPath(Color color)
    {
        pathColor = color;
        if (mainSpriteRenderer != null)
        {
            mainSpriteRenderer.color = color; // Path color takes over the background
        }
        if (dotSpriteRenderer != null && isEndpoint)
        {
            // Optional: Ensure dot is still visible if path covers it
            // This might involve setting sorting order dynamically or just ensuring dot renderer exists
            dotSpriteRenderer.enabled = true; // Make sure it's visible
        }
    }

    // Call this when erasing a path
    public void ClearPath()
    {
        pathColor = Color.clear;
        if (mainSpriteRenderer != null)
        {
            mainSpriteRenderer.color = defaultBackgroundColor; // Reset to default background
        }

        // If it's an endpoint, make sure the dot visual is restored correctly
        if (isEndpoint && dotSpriteRenderer != null)
        {
            SetAsEndpoint(endpointColor); // Re-apply endpoint visuals
        }
        else if (dotSpriteRenderer != null)
        {
            dotSpriteRenderer.enabled = false; // Hide dot if it's not an endpoint
        }
    }

    // Gets the color currently visible on the cell (either path or endpoint dot if no path)
    public Color GetCurrentColor()
    {
        if (pathColor != Color.clear && pathColor != defaultBackgroundColor)
        {
            return pathColor;
        }
        if (isEndpoint)
        {
            return endpointColor;
        }
        return Color.clear; // Represents an empty, non-endpoint cell
    }
}