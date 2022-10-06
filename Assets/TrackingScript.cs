using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Class is attached to the tracker prefab and tracks the player. This prefab has several "Tracker Points" that can be used by the AI to Move around and infront of the player to simulate driving alongside the player. 
/// </summary>
[System.Serializable]
public class TrackingScript : MonoBehaviour
{
    public GameObject player; //The Player Parent Object
    public GameObject playerMesh; //The Bike Mesh 
    public GameObject[] TPoints; //A list of the tracking points the AI can use. These can be easily added to by editing the prefab


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMesh = GameObject.FindGameObjectWithTag("PlayerMesh");
        TPoints = GameObject.FindGameObjectsWithTag("TrackerChild");
    }

    // Update is called once per frame
    void Update()
    {
        //Keeps Track of the Players Location, Make sure that Player is tagged Properly
        this.transform.position = player.transform.position;
        this.transform.rotation = playerMesh.transform.rotation;       
    }
}
