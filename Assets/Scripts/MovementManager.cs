using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {

        

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

        //UpdateCactiLocation();
        //UpdateFloorLocation();
    }
}
