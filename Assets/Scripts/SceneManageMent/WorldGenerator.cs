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
    private int starting_ground_tiles = 9;
    private float groundTileSpawnHeight = -3.12f;

    private int ground_array_size;


    private GameObject[,] newgroundTiles;
    [SerializeField] private int new_ground_size = 12;

    
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        ground_array_size = starting_ground_tiles;
        BikeScript[] bikeScripts = Object.FindObjectsOfType<BikeScript>();
        if (bikeScripts.Length <= 0) 
        {
            Debug.LogError("WorldGenerator did not find any BikeScripts in scene");
        }
        else 
        {
            bike = bikeScripts[0];
        }

        InitializeGround(ground_array_size);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdateNumberofGroundTiles();
        }

        CheckUpdateGroundTiles();
        
  
    }

    #region ground
    /// <summary>
    /// Spawns in ground gameObjects into groundTiles array and initializes the WorldBounds class
    /// </summary>
    private void InitializeGround(int GroundArraySize)
    {
        WorldBounds.groundTileSize = ground.GetComponent<Renderer>().bounds.size;
        float groundWidth = WorldBounds.GroundTileWidth;
        float groundHeight = WorldBounds.GroundTileHeight;
        groundTiles = new GameObject[GroundArraySize, GroundArraySize];

        // Initialize tiles at origin
        for (int i = 0; i < GroundArraySize; i++)
        {
            for (int j = 0; j < GroundArraySize; j++)
            {
                GameObject goundTile = Instantiate(ground);
                Vector3 spawnLocation = new Vector3(groundHeight * (i - 1), groundTileSpawnHeight, groundWidth * (j - 1));
                goundTile.transform.position = spawnLocation;
                groundTiles[i, j] = goundTile;
            }
        }
        UpdateMiddleTileMinMax(GroundArraySize, GroundArraySize);
        UpdateWorldMinMax();
    }

    private void UpdateNumberofGroundTiles()
    {
        for (int i = 0; i < ground_array_size ; i++)
        {
            for (int j= 0; j < ground_array_size; j++)
            {
                Destroy(groundTiles[i,j].gameObject);
            }
        }
        ground_array_size = new_ground_size;

        InitializeGround(ground_array_size);
    }

    /// <summary>
    /// Returns the center tile (the one the player is on)
    /// </summary>
    private GameObject GetMiddleTile(int groundarraywidth, int groundarrayheight)
    {
        int middleHeightIdx = GetMiddleTileVerticalIdx(groundarraywidth);
        int middleWidthIdx = GetMiddleTileHorizontalIdx(groundarrayheight);
        return groundTiles[middleHeightIdx, middleWidthIdx];
    }

    /// <summary>
    /// Returns the vertical index of the middle tile inside the groundTiles array
    /// </summary>
    private int GetMiddleTileVerticalIdx(int vertical_index)
    {
        return (vertical_index - 1) / 2;
    }

    /// <summary>
    /// Returns the horizontal index of the middle tile inside the groundTiles array
    /// </summary>
    private int GetMiddleTileHorizontalIdx(int horizontal_index)
    {
        return (horizontal_index - 1) / 2;
    }

    /// <summary>
    /// Caches the Min/Max of the tile the player is currently on inside WorldBounds
    /// </summary>
    private void UpdateMiddleTileMinMax(int ground_array_width, int ground_array_height)
    {
        Vector3 currentTileLocation = GetMiddleTile(ground_array_width, ground_array_height).transform.position;
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
        if (bike != null)
        {
            Vector3 bikePosition = bike.transform.position;
            float bikeHorizontalPos = bikePosition.x;
            float bikeVerticalPos = bikePosition.z;
            if (bikeHorizontalPos > WorldBounds.currentTileHorizontalMinMax.y && bikeVerticalPos > WorldBounds.currentTileVericalMinMax.y) { MoveGroundTiles(1, 1, ground_array_size); } // Upper Right
            else if (bikeHorizontalPos > WorldBounds.currentTileHorizontalMinMax.y && bikeVerticalPos < WorldBounds.currentTileVericalMinMax.x) { MoveGroundTiles(1, -1, ground_array_size); } // Lower Right
            else if (bikeHorizontalPos < WorldBounds.currentTileHorizontalMinMax.x && bikeVerticalPos > WorldBounds.currentTileVericalMinMax.y) { MoveGroundTiles(-1, 1, ground_array_size); } // Upper Left
            else if (bikeHorizontalPos < WorldBounds.currentTileHorizontalMinMax.x && bikeVerticalPos < WorldBounds.currentTileVericalMinMax.x) { MoveGroundTiles(-1, -1, ground_array_size); } // Lower Left
            else if (bikeHorizontalPos > WorldBounds.currentTileHorizontalMinMax.y) { MoveGroundTiles(1, 0, ground_array_size); } // Right Quyadrant
            else if (bikeHorizontalPos < WorldBounds.currentTileHorizontalMinMax.x) { MoveGroundTiles(-1, 0, ground_array_size); } // Left Quadrant
            else if (bikeVerticalPos > WorldBounds.currentTileVericalMinMax.y) { MoveGroundTiles(0, 1, ground_array_size); } // Upper Quadrant
            else if (bikeVerticalPos < WorldBounds.currentTileVericalMinMax.x) { MoveGroundTiles(0, -1, ground_array_size); } // Lower Quadrant
        }
    }

    /// <summary>
    /// Adjusts the location of the ground tiles in the x and y directions
    /// </summary>
    /// <param name="xDiff">A value representing the direction and quantity of ground tile widths the grid must be shifted in the x direction.</param>
    /// <param name="yDiff">A value representing the direction and quantity of ground tile widths the grid must be shifted in the y direction.</param>
    private void MoveGroundTiles(int xDiff, int yDiff, int ground_array_size)
    {
        for (int i = 0; i < ground_array_size; i++)
        {
            for (int j = 0; j < ground_array_size; j++)
            {
                Vector3 newPosition = groundTiles[i, j].transform.position + new Vector3(xDiff * WorldBounds.groundTileSize.x, 0, yDiff * WorldBounds.groundTileSize.z);
                groundTiles[i, j].transform.position = newPosition;
            }
        }
        // Update WorldBounds object
        UpdateMiddleTileMinMax(ground_array_size, ground_array_size);
        UpdateWorldMinMax();
    }


    #endregion
}
