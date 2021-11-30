using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusScript : MonoBehaviour
{

    public Vector2 cPosition;
    protected Vector2 bPosition; 


    public Vector3 spawnPoint;
    
   
    

    public Vector2 getPosition()
    {
        return cPosition;
    }

    // Start is called before the first frame update
    void Start()
    {

        cPosition = transform.position;
        

    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
