using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackingScript : MonoBehaviour
{
    public GameObject player;

    public GameObject[] TPoints;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("TracerMeshParent");

        TPoints = GameObject.FindGameObjectsWithTag("TrackerChild");

        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
