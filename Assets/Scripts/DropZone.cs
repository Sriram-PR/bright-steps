using UnityEngine;
using UnityEngine.EventSystems; // Required for drop interface
using TMPro;                 // Required if using TextMeshPro for visual label (optional here)

public class DropZone : MonoBehaviour, IDropHandler // Implement the IDropHandler interface
{
    // The color name this zone expects (MUST match DraggableItem.colorName)
    // Set this value in the Inspector for each DropZone GameObject (e.g., "Red", "Blue")
    public string expectedColorName;

    // Optional: Reference to the visual text label if needed
    // public TextMeshProUGUI labelText;

    void Awake()
    {
        // Basic check on setup
        if (string.IsNullOrEmpty(expectedColorName))
        {
            Debug.LogWarning($"DropZone on {gameObject.name} has no 'Expected Color Name' set in the Inspector!", this);
        }
        // Ensure Raycast Target is enabled on the Text/Image component so drops can be detected
        // This needs to be done manually in the Inspector on the TextMeshPro or Image component.
    }

    // This method is called by the EventSystem when a draggable item is released over this GameObject
    public void OnDrop(PointerEventData eventData)
    {
        // Debug.Log($"{gameObject.name} detected drop");

        // Get the GameObject that was being dragged
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return; // Exit if nothing was actually dropped

        // Try to get the DraggableItem script from the dropped object
        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();

        // --- Check for a Correct Match ---
        if (draggableItem != null && draggableItem.colorName == expectedColorName)
        {
            // --- Success! ---
            Debug.Log($"Correct Match: {draggableItem.colorName} dropped on {gameObject.name} (expects {expectedColorName})");

            // 1. Notify the GameManager to record the score and handle win condition checks
            //    (GameManager will also play the correct sound internally)
            GameManager.Instance?.RecordMatch(1); // Assuming score increases by 1

            // 2. Disable the dropped block
            draggableItem.gameObject.SetActive(false);

            // 3. Disable this drop zone (so it can't be used again)
            gameObject.SetActive(false);
        }
        else
        {
            // --- Incorrect Match or Not a Draggable Item ---
            // No action needed here. The DraggableItem's OnEndDrag method will handle
            // snapping the item back to its start position and playing the incorrect sound.
            if (draggableItem != null)
            {
                // Debug.Log($"Incorrect Match: {draggableItem.colorName} dropped on {gameObject.name} (expects {expectedColorName})");
            }
            else
            {
                // Debug.Log($"Non-DraggableItem {droppedObject.name} dropped on {gameObject.name}");
            }
        }
    }
}