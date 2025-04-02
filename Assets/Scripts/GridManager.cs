using UnityEngine;
using System.Collections.Generic; // Make sure this is included

[System.Serializable] // Makes this visible in the Inspector when used in a List
public struct EndpointPair
{
    public Color color;
    public Vector2Int startPos; // Use Vector2Int for integer grid coordinates
    public Vector2Int endPos;
}

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 1.0f;
    public float spacing = 0.1f;
    public Color defaultCellBackgroundColor = Color.gray; // Choose a default background

    [Header("Level Data")]
    public LevelData currentLevelData; // Assign your Level_01 asset here in the Inspector

    [Header("References")]
    public GameObject cellPrefab; // Prefab for the visual cell (needs Cell script)

    private Cell[,] cellScripts; // 2D array to hold references to the Cell components
    private int gridWidth; // Internal storage for width from level data
    private int gridHeight; // Internal storage for height from level data

    void Start()
    {
        if (currentLevelData == null)
        {
            Debug.LogError("Current Level Data is not assigned in the GridManager Inspector!");
            return;
        }
        if (cellPrefab == null)
        {
            Debug.LogError("Cell Prefab is not assigned in the GridManager Inspector!");
            return;
        }
        if (cellPrefab.GetComponent<Cell>() == null)
        {
            Debug.LogError("Cell Prefab is missing the 'Cell' script component!");
            return;
        }

        LoadLevel(currentLevelData);
    }

    void LoadLevel(LevelData levelData)
    {
        Debug.Log($"Loading level: {levelData.name}");
        ClearGrid();
        gridWidth = levelData.width;
        gridHeight = levelData.height;
        CreateGrid();
        PlaceEndpoints(levelData);
        Debug.Log("Level loaded successfully.");
    }

    void CreateGrid()
    {
        cellScripts = new Cell[gridWidth, gridHeight];

        float totalGridWidth = gridWidth * cellSize + (gridWidth > 1 ? (gridWidth - 1) * spacing : 0);
        float totalGridHeight = gridHeight * cellSize + (gridHeight > 1 ? (gridHeight - 1) * spacing : 0);
        Vector3 startOffset = new Vector3(-totalGridWidth / 2f + cellSize / 2f, totalGridHeight / 2f - cellSize / 2f, 0f);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float posX = x * (cellSize + spacing);
                float posY = y * (cellSize + spacing);
                Vector3 cellPosition = new Vector3(posX, -posY, 0f) + startOffset;

                GameObject newCellObject = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                newCellObject.transform.SetParent(this.transform);
                newCellObject.name = $"Cell ({x}, {y})";

                Cell cellScript = newCellObject.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.Initialize(x, y, defaultCellBackgroundColor);
                    cellScripts[x, y] = cellScript;
                }
                else
                {
                    Debug.LogError($"Instantiated cell at ({x},{y}) is missing the Cell script!");
                    Destroy(newCellObject);
                }
            }
        }
        Debug.Log($"Grid created with {gridWidth}x{gridHeight} cells.");
    }

    void PlaceEndpoints(LevelData levelData)
    {
        if (levelData == null || levelData.endpointPairs == null) return;

        foreach (EndpointPair pair in levelData.endpointPairs)
        {
            if (IsValidCoordinate(pair.startPos.x, pair.startPos.y) &&
                IsValidCoordinate(pair.endPos.x, pair.endPos.y))
            {
                Cell startCell = cellScripts[pair.startPos.x, pair.startPos.y];
                Cell endCell = cellScripts[pair.endPos.x, pair.endPos.y];

                if (startCell != null && endCell != null)
                {
                    startCell.SetAsEndpoint(pair.color);
                    endCell.SetAsEndpoint(pair.color);
                    Debug.Log($"Placed endpoint pair: Color={pair.color}, Start=({pair.startPos.x},{pair.startPos.y}), End=({pair.endPos.x},{pair.endPos.y})");
                }
                else
                {
                    Debug.LogError($"Failed to find Cell scripts for endpoint pair: Color={pair.color}, Start=({pair.startPos.x},{pair.startPos.y}), End=({pair.endPos.x},{pair.endPos.y})");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid coordinates in endpoint pair: Color={pair.color}, Start=({pair.startPos.x},{pair.startPos.y}), End=({pair.endPos.x},{pair.endPos.y}). Skipping.");
            }
        }
    }

    bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    public Cell GetCell(int x, int y)
    {
        if (IsValidCoordinate(x, y))
        {
            return cellScripts[x, y];
        }
        return null;
    }

    void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        cellScripts = null;
        gridWidth = 0;
        gridHeight = 0;
        Debug.Log("Previous grid cleared.");
    }
}