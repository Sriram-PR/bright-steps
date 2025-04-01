using UnityEngine;
using UnityEngine.EventSystems; // Required for drag interfaces
using UnityEngine.UI;          // Required for Image

// Ensure this component has the components it relies on
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Public field for identification by DropZone and GameManager
    public string colorName;

    // --- References obtained in Awake ---
    // We use HideInInspector because GameManager needs to access these for reset,
    // but we don't need to assign them manually in the Inspector.
    [HideInInspector] public Image image;
    [HideInInspector] public RectTransform rectTransform;
    private Canvas rootCanvas; // The main Canvas the UI element lives on

    // --- Stored Initial State (Set in Awake, Used by GameManager for Reset) ---
    [HideInInspector] public Vector2 startAnchoredPosition;
    [HideInInspector] public Vector3 startScale;
    [HideInInspector] public Transform parentAfterDrag; // Original parent transform

    void Awake()
    {
        // --- Get Necessary Components ---
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        // Find the highest Canvas parent (important for correct coordinate calculations)
        rootCanvas = GetComponentInParent<Canvas>();

        // --- Error Checking ---
        if (image == null) Debug.LogError($"DraggableItem on {gameObject.name} needs Image component.", this);
        if (rectTransform == null) Debug.LogError($"DraggableItem on {gameObject.name} needs RectTransform component.", this);
        if (rootCanvas == null) Debug.LogError($"DraggableItem on {gameObject.name} couldn't find parent Canvas. Ensure it's under a Canvas.", this);

        // --- Store Initial State ---
        // Crucial for resetting the item correctly via GameManager
        startAnchoredPosition = rectTransform.anchoredPosition; // Store position relative to anchors
        startScale = transform.localScale;                     // Store the local scale
        parentAfterDrag = transform.parent;                    // Store the original parent GO's transform

        // --- Debug Logging (Optional but helpful) ---
        // Debug.Log($"{gameObject.name} Awake: Stored pos={startAnchoredPosition}, scale={startScale}, parent={parentAfterDrag?.name}");
    }

    // Called when the user begins dragging this UI element
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Safety check
        if (rectTransform == null || rootCanvas == null || !gameObject.activeSelf) return;

        // Debug.Log($"Begin Drag on {gameObject.name}");

        // Store original parent in case it changes dynamically (usually set reliably in Awake)
        // parentAfterDrag = transform.parent;

        // Move the item to the top level of the Canvas visually so it renders over other UI elements
        transform.SetParent(rootCanvas.transform, true); // Use worldPositionStays = true to maintain current world position temporarily
        transform.SetAsLastSibling();                   // Ensure it renders visually on top

        // Disable raycasting on self so pointer events can go through to potential drop zones underneath
        if (image != null) image.raycastTarget = false;

        // --- Optional: Play Pickup Sound ---
        // GameManager.Instance?.PlayPickupSound(); // Uncomment if you add a pickup sound
    }

    // Called repeatedly while the user is dragging
    public void OnDrag(PointerEventData eventData)
    {
        // Safety check
        if (rectTransform == null || rootCanvas == null || !gameObject.activeSelf) return;

        // Convert the mouse/touch screen position to the local position within the parent RectTransform (the Canvas)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform, // The RectTransform of the root Canvas
            eventData.position,                    // Current mouse/touch position on the screen
            (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : rootCanvas.worldCamera, // Camera reference needed for non-Overlay canvases
            out Vector2 localPoint                 // The output: position within the Canvas's local coordinate system
        );

        // Set the item's anchored position (position relative to anchors) to this calculated local point
        // This makes the item follow the cursor/finger accurately within the Canvas space
        rectTransform.anchoredPosition = localPoint;
    }

    // Called when the user releases the drag
    public void OnEndDrag(PointerEventData eventData)
    {
        // Safety check
        if (!gameObject.activeSelf) return; // Don't process if already deactivated (e.g., by a successful drop)

        // Debug.Log($"End Drag on {gameObject.name}");

        // --- Determine if the drop was successful (i.e., handled by a DropZone) ---
        // A successful drop usually means the DropZone script disabled this GameObject or the DropZone itself.
        // If this GameObject is *still active* after the potential drop, we assume it failed or landed nowhere valid.
        bool dropWasSuccessful = !this.gameObject.activeSelf;

        // --- Reset Parent FIRST ---
        // Always try to reset the parent. If the drop was invalid, it stays here.
        // If valid, the DropZone should have already deactivated this object, so this doesn't matter visually.
        if (parentAfterDrag != null)
        {
            transform.SetParent(parentAfterDrag);
        }
        else
        {
            // Fallback if parent wasn't stored - might indicate an issue in Awake
            Debug.LogWarning($"ParentAfterDrag was null for {gameObject.name} on EndDrag. Could not reset parent.");
        }

        // --- Snap Back if Drop Failed ---
        if (!dropWasSuccessful && this.gameObject.activeSelf) // Double check it's still active
        {
            // The drop wasn't handled by a DropZone, so snap back to original state
            if (rectTransform != null) rectTransform.anchoredPosition = startAnchoredPosition;
            transform.localScale = startScale; // Reset scale explicitly

            // Play incorrect sound because it snapped back
            GameManager.Instance?.PlayIncorrectSound();
            // Debug.Log($"{gameObject.name} snapped back to start position and scale.");
        }
        else
        {
            // DropZone script likely disabled this block, indicating a successful drop. Nothing more to do here.
            // Debug.Log($"{gameObject.name} was disabled, assuming successful drop handled by DropZone.");
        }

        // --- Re-enable Raycasting AFTER potential snapback/reparenting ---
        // Important so the item can be interacted with again if it snapped back.
        if (image != null) image.raycastTarget = true;
    }
}