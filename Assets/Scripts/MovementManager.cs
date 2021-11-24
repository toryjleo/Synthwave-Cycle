using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public GameObject ground;
    public BikeScript bike;
    public GameObject Cactus;
    public GameObject[] cacti;

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
        UpdateNextMovement();
        UpdateLocations();
    }

    private void UpdateFloorLocation()
    {
        Material groundMat = ground.GetComponent<Renderer>().material;
        groundMat.SetFloat("_XPos", -bike.GetPosition().x);
        groundMat.SetFloat("_YPos", bike.GetPosition().y);
        Debug.Log(bike.GetPosition());
    }

    private void UpdateNextMovement()
    {
        bike.UpdateNextMovement();
        bike.UpdateLocations();
    }

    private void UpdateLocations()
    {
        UpdateFloorLocation();
    }
}
