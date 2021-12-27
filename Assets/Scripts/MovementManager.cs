using UnityEngine;

public delegate void NotifyPlayerPosition(Vector3 currentPlayerPosition);  // delegate

public class MovementManager : MonoBehaviour
{
    public GameObject ground;
    public BikeScript bike;
    public GameObject Cactus;
    public CactusScript cacscrip;
    public GameObject[] cacti;
    public Vector3 curVec;
    private float uvScrollSpeed = .013f;
    private Vector3 floorOffset;

    public Gun InitialPlayerGun;




    public event NotifyPlayerPosition playerPositionUpdate; // event


    private GameObject[,] groundTiles;
    private const int GROUND_ARRAY_WIDTH  = 3;
    private int GROUND_ARRAY_HEIGHT = 3;
    private Vector3 groundSize;
    private float groundTileSpawnHeight = -3.12f;
    private Vector2 currentTileHorizontalMinMax;
    private Vector2 currentTileVericalMinMax;
    

    private void Awake()
    {
        InitialPlayerGun.Init(playerPositionUpdate);
    }

    // Start is called before the first frame update
    void Start()
    {
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

        floorOffset = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyForces();
        UpdateLocations();
    }

    private void Update()
    {
        CheckUpdateGroundTiles();
    }

    private void InitializeGround() 
    {
        groundSize = ground.GetComponent<Renderer>().bounds.size;
        float groundWidth = groundSize.x;
        float groundHeight = groundSize.z;
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
        float halfTileWidth = groundSize.x / 2;
        currentTileHorizontalMinMax = new Vector2(currentTileLocation.x - halfTileWidth, currentTileLocation.x + halfTileWidth);
        float halfTileHeight = groundSize.z / 2;
        currentTileVericalMinMax = new Vector2(currentTileLocation.z - halfTileHeight, currentTileLocation.z + halfTileHeight);
    }

    private void CheckUpdateGroundTiles() 
    {
        Vector3 bikePosition = bike.transform.position;
        float bikeHorizontalPos = bikePosition.x;
        float bikeVerticalPos = bikePosition.z;
        if (bikeHorizontalPos > currentTileHorizontalMinMax.y && bikeVerticalPos > currentTileVericalMinMax.y) { MoveGroundTiles(1, 1); } // Upper Right
        else if (bikeHorizontalPos > currentTileHorizontalMinMax.y && bikeVerticalPos < currentTileVericalMinMax.x) { MoveGroundTiles(1, -1); } // Lower Right
        else if (bikeHorizontalPos < currentTileHorizontalMinMax.x && bikeVerticalPos > currentTileVericalMinMax.y) { MoveGroundTiles(-1, 1); } // Upper Left
        else if (bikeHorizontalPos < currentTileHorizontalMinMax.x && bikeVerticalPos < currentTileVericalMinMax.x) { MoveGroundTiles(-1, -1); } // Lower Left
        else if (bikeHorizontalPos > currentTileHorizontalMinMax.y) { MoveGroundTiles(1, 0); } // Right Quyadrant
        else if (bikeHorizontalPos < currentTileHorizontalMinMax.x) { MoveGroundTiles(-1, 0); } // Left Quadrant
        else if (bikeVerticalPos > currentTileVericalMinMax.y) { MoveGroundTiles(0, 1); } // Upper Quadrant
        else if (bikeVerticalPos < currentTileVericalMinMax.x) { MoveGroundTiles(0, -1); } // Lower Quadrant
    }

    private void MoveGroundTiles(int xDiff, int yDiff) 
    {
        for (int i = 0; i < GROUND_ARRAY_HEIGHT; i++)
        {
            for (int j = 0; j < GROUND_ARRAY_WIDTH; j++)
            {
                Vector3 newPosition = groundTiles[i, j].transform.position + new Vector3(xDiff * groundSize.x, 0, yDiff * groundSize.z);
                groundTiles[i, j].transform.position = newPosition;
            }
        }
        UpdateMiddleTileMinMax();
    }

    /// <summary>Updates the floor mesh to reflect Player movement.</summary>
    private void UpdateFloorLocation()
    {
        Vector3 deltaPosition = bike.DeltaPosition;
        floorOffset.x = (floorOffset.x - (deltaPosition.x  * uvScrollSpeed)) % 1;
        floorOffset.z = (floorOffset.z + (deltaPosition.z * uvScrollSpeed)) % 1;

        // Edit the UV's coords of the ground shader
        Material groundMat = ground.GetComponent<Renderer>().material;
        groundMat.SetFloat("_XPos", floorOffset.x);
        groundMat.SetFloat("_YPos", floorOffset.z);
    }


    private void UpdateCactiLocation()
    {
        
        for(int i = 0; i<10; i++)
        {
            cacti[i].GetComponent<CactusScript>().move(curVec);
            
        }

    }

    /// <summary>Applies all forces to the Player bike this frame before position is updated.</summary>
    private void ApplyForces()
    {      
        bike.ApplyForces();       
    }

    /// <summary>Update the locations of all objects in the scene at once.</summary>
    private void UpdateLocations()
    {
        bike.UpdateLocations();
        curVec = bike.DeltaPosition; // Distance Player bike has moved this frame

        OnPlayerMove();

        //UpdateCactiLocation();
        //UpdateFloorLocation();
    }

    protected virtual void OnPlayerMove() //protected virtual method
    {
        Vector3 bikePosition = bike.transform.position;
        playerPositionUpdate?.Invoke(bikePosition);
    }
}
