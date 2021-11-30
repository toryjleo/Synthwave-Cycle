using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public GameObject ground;
    public BikeScript bike;
    public GameObject Cactus;
    public GameObject[] cacti;
    public Vector3 previousFrame;

    // Start is called before the first frame update
    void Start()
    {
        cacti = new GameObject[10];
        for (int i =0;i<10;i++)
        {
            GameObject go = Instantiate(Cactus, new Vector3(Random.Range(-60, 60), 0, Random.Range(-60, 60)), Quaternion.identity) as GameObject;
            go.transform.localScale = Vector3.one;
            cacti[i] = go;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyForces();
        UpdateLocations();
    }

    private void UpdateFloorLocation() //Update the floor mesh to reflect movement
    {
        Material groundMat = ground.GetComponent<Renderer>().material;
        groundMat.SetFloat("_XPos", -bike.GetPosition().x);
        groundMat.SetFloat("_YPos", bike.GetPosition().y);
        Debug.Log(bike.GetPosition());
    }


    private void UpdateCactiLocation()
    {

        for (int i = 0; i < 10; i++)
        {

            Vector3 boop = new Vector3(bike.GetPosition().x, 0, bike.GetPosition().y);
            boop =  previousFrame - boop;

                cacti[i].transform.position = new Vector3(
                cacti[i].transform.position.x-boop.x,
                0,
                cacti[i].transform.position.z-boop.z
                );

        }

    }

    private void UpdateNextMovement()
    {
        previousFrame = bike.transform.position;
        bike.UpdateNextMovement();
        bike.UpdateLocations();

    }

    private void ApplyForces()
    {
        bike.ApplyForces();

    }

    private void UpdateLocations()
    {
        UpdateFloorLocation();
        bike.UpdateLocations();
    }
}
