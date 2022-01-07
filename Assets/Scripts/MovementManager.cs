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



public class MovementManager : MonoBehaviour
{
    public ScoreTracker scoreTracker;
    public BikeScript bike;
    public GameObject Cactus;
    public CactusScript cacscrip;
    public GameObject[] cacti;
    public Vector3 curVec;

    public Gun InitialPlayerGun;

    #region ground
    public GameObject ground;
    private GameObject[,] groundTiles;
    private const int GROUND_ARRAY_WIDTH  = 3;
    private int GROUND_ARRAY_HEIGHT = 3;
    private float groundTileSpawnHeight = -3.12f;
    #endregion

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreTracker = FindObjectOfType<ScoreTracker>(); // Find the single instance of ScoreTracker in the scene
        InitializeGround();

        bike.EquipGun(InitialPlayerGun);
        //Spawn Cactai 
        cacti = new GameObject[10];

        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnP = new Vector3(Random.Range(-80, 80), -2, Random.Range(-80, 80));
            cacti[i] = Instantiate(Cactus, spawnP, Quaternion.identity);
            cacti[i].GetComponent<CactusScript>().grow(spawnP);

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyForces();
    }

    private void Update()
    {
        CheckUpdateGroundTiles();
    }

    #region ground
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

    private GameObject GetMiddleTile() 
    {
        int middleHeightIdx = GetMiddleTileVerticalIdx();
        int middleWidthIdx = GetMiddleTileHorizontalIdx();
        return groundTiles[middleHeightIdx, middleWidthIdx];
    }
    
    private int GetMiddleTileVerticalIdx()
    {
        return (GROUND_ARRAY_HEIGHT - 1) / 2;
    }

    private int GetMiddleTileHorizontalIdx() 
    {
        return (GROUND_ARRAY_WIDTH - 1) / 2;
    }

    private void UpdateMiddleTileMinMax() 
    {
        Vector3 currentTileLocation = GetMiddleTile().transform.position;
        float halfTileWidth = WorldBounds.groundTileSize.x / 2;
        WorldBounds.currentTileHorizontalMinMax = new Vector2(currentTileLocation.x - halfTileWidth, currentTileLocation.x + halfTileWidth);
        float halfTileHeight = WorldBounds.groundTileSize.z / 2;
        WorldBounds.currentTileVericalMinMax = new Vector2(currentTileLocation.z - halfTileHeight, currentTileLocation.z + halfTileHeight);
    }

    private void UpdateWorldMinMax() 
    {
        WorldBounds.worldBoundsHorizontalMinMax = new Vector2(WorldBounds.currentTileHorizontalMinMax.x - WorldBounds.GroundTileWidth, WorldBounds.currentTileHorizontalMinMax.y + WorldBounds.GroundTileWidth);
        WorldBounds.worldBoundsVericalMinMax = new Vector2(WorldBounds.currentTileVericalMinMax.x - WorldBounds.GroundTileHeight, WorldBounds.currentTileVericalMinMax.y + WorldBounds.GroundTileHeight);
    }

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
        UpdateMiddleTileMinMax();
        UpdateWorldMinMax();
    }
    #endregion

    /// <summary>Applies all forces to the Player bike this frame before position is updated.</summary>
    private void ApplyForces()
    {      
        bike.ApplyForces();       
    }

}
