using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraFollower : MonoBehaviour
{
    public GameObject player;
    public float cameraHeight = 35;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = player.transform.position;
        pos.y += cameraHeight;
        transform.position = pos;
    }
}
