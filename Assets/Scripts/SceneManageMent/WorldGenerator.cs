using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>WorldBounds</c> A static class which will contain the bounds of the world. Updates every frame</summary>
///
public static class WorldBounds
{
    // Current tile Min/Max
    public static Vector2 currentTileHorizontalMinMax;
    public static Vector2 currentTileVericalMinMax;
    public static Vector3 groundTileSize;

    /// <summary></summary>
    public static float GroundTileWidth
    {
        get => groundTileSize.x;
    }
    /// <summary></summary>
    public static float GroundTileHeight
    {
        get => groundTileSize.z;
    }


    public static Vector2 worldBoundsHorizontalMinMax;
    public static Vector2 worldBoundsVericalMinMax;

}

/// <summary>Class <c>WorldGenerator</c> Spawns in the ground, updates ground, updates WorldBounds. 
/// Expects there to be an object with BikeScript in the scene.</summary>
public class WorldGenerator : MonoBehaviour
{
    private BikeScript bike;
    #region ground
    [SerializeField] private GameObject ground;
    private GameObject[,] groundTiles;
    private const int GROUND_ARRAY_WIDTH = 9;
    private int GROUND_ARRAY_HEIGHT = 9;
    private float groundTileSpawnHeight = -3.12f;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        BikeScript[] bikeScripts = Object.FindObjectsOfType<BikeScript>();
        if (bikeScripts.Length <= 0) 
        {
            Debug.LogError("WorldGenerator did not find any BikeScripts in scene");
        }
        else 
        {
            bike = bikeScripts[0];
        }

        InitializeGround();
    }

    // Update is called once per frame
    void Update()
    {
        CheckUpdateGroundTiles();
    }

    #region ground
    /// <summary>
    /// Spawns in ground gameObjects into groundTiles array and initializes the WorldBounds class
    /// </summary>
    private void InitializeGround()
    {
        WorldBounds.groundTileSize = ground.GetComponent<Renderer>().bounds.size;
        float groundWidth = WorldBounds.GroundTileWidth;
        float groundHeight = WorldBounds.GroundTileHeight;
        groundTiles = new GameObject[GROUND_ARRAY_WIDTH, GROUND_ARRAY_HEIGHT];

        // Initialize tiles at origin
        for (int i = 0; i < GROUND_ARRAY_HEIGHT; i++)
        {
            for (int j = 0; j < GROUND_ARRAY_WIDTH; j++)
            {
                GameObject goundTile = Instantiate(ground);
                Vector3 spawnLocation = new Vector3(groundHeight * (i - 1), groundTileSpawnHeight, groundWidth * (j - 1));
                goundTile.transform.position = spawnLocation;
                groundTiles[i, j] = goundTile;
            }
        }
        UpdateMiddleTileMinMax();
        UpdateWorldMinMax();
    }

    /// <summary>
    /// Returns the center tile (the one the player is on)
    /// </summary>
    private GameObject GetMiddleTile()
    {
        int middleHeightIdx = GetMiddleTileVerticalIdx();
        int middleWidthIdx = GetMiddleTileHorizontalIdx();
        return groundTiles[middleHeightIdx, middleWidthIdx];
    }

    /// <summary>
    /// Returns the vertical index of the middle tile inside the groundTiles array
    /// </summary>
    private int GetMiddleTileVerticalIdx()
    {
        return (GROUND_ARRAY_HEIGHT - 1) / 2;
    }

    /// <summary>
    /// Returns the horizontal index of the middle tile inside the groundTiles array
    /// </summary>
    private int GetMiddleTileHorizontalIdx()
    {
        return (GROUND_ARRAY_WIDTH - 1) / 2;
    }

    /// <summary>
    /// Caches the Min/Max of the tile the player is currently on inside WorldBounds
    /// </summary>
    private void UpdateMiddleTileMinMax()
    {
        Vector3 currentTileLocation = GetMiddleTile().transform.position;
        float halfTileWidth = WorldBounds.groundTileSize.x / 2;
        WorldBounds.currentTileHorizontalMinMax = new Vector2(currentTileLocation.x - halfTileWidth, currentTileLocation.x + halfTileWidth);
        float halfTileHeight = WorldBounds.groundTileSize.z / 2;
        WorldBounds.currentTileVericalMinMax = new Vector2(currentTileLocation.z - halfTileHeight, currentTileLocation.z + halfTileHeight);
    }

    /// <summary>
    /// Caches the Min/Max of the world (all of the tiles) on inside WorldBounds
    /// </summary>
    private void UpdateWorldMinMax()
    {
        WorldBounds.worldBoundsHorizontalMinMax = new Vector2(WorldBounds.currentTileHorizontalMinMax.x - WorldBounds.GroundTileWidth, WorldBounds.currentTileHorizontalMinMax.y + WorldBounds.GroundTileWidth);
        WorldBounds.worldBoundsVericalMinMax = new Vector2(WorldBounds.currentTileVericalMinMax.x - WorldBounds.GroundTileHeight, WorldBounds.currentTileVericalMinMax.y + WorldBounds.GroundTileHeight);
    }

    /// <summary>
    /// Checks if the groundTiles array needs to be updated and updates it if needed
    /// </summary>
    private void CheckUpdateGroundTiles()
    {
        Vector3 bikePosition = bike.transform.position;
        float bikeHorizontalPos = bikePosition.x;
        float bikeVerticalPos = bikePosition.z;
        if (bikeHorizontalPos > WorldBounds.currentTileHorizontalMinMax.y && bikeVerticalPos > WorldBounds.currentTileVericalMinMax.y) { MoveGroundTiles(1, 1); } // Upper Right
        else if (bikeHorizontalPos > WorldBounds.currentTileHorizontalMinMax.y && bikeVerticalPos < WorldBounds.currentTileVericalMinMax.x) { MoveGroundTiles(1, -1); } // Lower Right
        else if (bikeHorizontalPos < WorldBounds.currentTileHorizontalMinMax.x && bikeVerticalPos > WorldBounds.currentTileVericalMinMax.y) { MoveGroundTiles(-1, 1); } // Upper Left
        else if (bikeHorizontalPos < WorldBounds.currentTileHorizontalMinMax.x && bikeVerticalPos < WorldBounds.currentTileVericalMinMax.x) { MoveGroundTiles(-1, -1); } // Lower Left
        else if (bikeHorizontalPos > WorldBounds.currentTileHorizontalMinMax.y) { MoveGroundTiles(1, 0); } // Right Quyadrant
        else if (bikeHorizontalPos < WorldBounds.currentTileHorizontalMinMax.x) { MoveGroundTiles(-1, 0); } // Left Quadrant
        else if (bikeVerticalPos > WorldBounds.currentTileVericalMinMax.y) { MoveGroundTiles(0, 1); } // Upper Quadrant
        else if (bikeVerticalPos < WorldBounds.currentTileVericalMinMax.x) { MoveGroundTiles(0, -1); } // Lower Quadrant
    }

    /// <summary>
    /// Adjusts the location of the ground tiles in the x and y directions
    /// </summary>
    /// <param name="xDiff">A value representing the direction and quantity of ground tile widths the grid must be shifted in the x direction.</param>
    /// <param name="yDiff">A value representing the direction and quantity of ground tile widths the grid must be shifted in the y direction.</param>
    private void MoveGroundTiles(int xDiff, int yDiff)
    {
        for (int i = 0; i < GROUND_ARRAY_HEIGHT; i++)
        {
            for (int j = 0; j < GROUND_ARRAY_WIDTH; j++)
            {
                Vector3 newPosition = groundTiles[i, j].transform.position + new Vector3(xDiff * WorldBounds.groundTileSize.x, 0, yDiff * WorldBounds.groundTileSize.z);
                groundTiles[i, j].transform.position = newPosition;
            }
        }
        // Update WorldBounds object
        UpdateMiddleTileMinMax();
        UpdateWorldMinMax();
    }
    #endregion
}
