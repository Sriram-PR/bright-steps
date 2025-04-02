using UnityEngine;
using System.Collections.Generic; // Required for List

// This attribute allows you to create instances of this data asset in the Project window
[CreateAssetMenu(fileName = "Level_01", menuName = "ColorConnect/Level Data")]
public class LevelData : ScriptableObject // Inherits from ScriptableObject, not MonoBehaviour
{
    [Header("Grid Dimensions")]
    public int width = 5;
    public int height = 5;

    [Header("Endpoint Pairs")]
    public List<EndpointPair> endpointPairs = new List<EndpointPair>();

    // You could add other level-specific settings here later (e.g., move limits, obstacles)
}