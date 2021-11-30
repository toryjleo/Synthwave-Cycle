using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public GameObject ground;
    public BikeScript bike;
    public GameObject Cactus;
    public CactusScript cacscrip;
    public GameObject[] cacti;
    public Vector3 preVec;
    public Vector3 curVec;

    // Start is called before the first frame update
    void Start()
    {

        

        //Spawn Cactai
        cacti = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {         
            //cacti[i] = Cactus;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BikePositionLastFrame();
        ApplyForces();
        UpdateLocations();
    }

    private void UpdateFloorLocation() //Update the floor mesh to reflect movement
    {
        Material groundMat = ground.GetComponent<Renderer>().material;
        groundMat.SetFloat("_XPos", -bike.GetPosition().x);
        groundMat.SetFloat("_YPos", bike.GetPosition().y);
        //Debug.Log(bike.GetPosition());
    }


    private void UpdateCactiLocation()
    {

        Debug.Log(curVec+" "+preVec);
        Vector3 offset = curVec - preVec;
        cacscrip.move(offset);
    }

    private void BikePositionLastFrame()
    {
        
    }

    private void ApplyForces()
    {
        
        bike.ApplyForces();
        
        
    }

    private void UpdateLocations()
    {
        
        
        preVec = new Vector3(bike.position.x, 0, bike.position.y);
        bike.UpdateLocations();
        curVec = new Vector3(bike.position.x, 0, bike.position.y);

        UpdateCactiLocation();
        UpdateFloorLocation();
    }
}
