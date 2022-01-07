using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusScript : MonoBehaviour
{

    public Vector3 cPosition;
    public int HP;

    
    
    public void grow(Vector3 spawn)
    {
        HP = 1;
        
        cPosition = spawn;
        transform.position = spawn; 
    }

    public void move(Vector3 offset)
    {

        //need to figure out how to turn this statement to take into account mocement of player 
        
        Vector3 move = new Vector3(-offset.x, 0, offset.z);
        cPosition += move;
        transform.position = cPosition;

        // transform.position = new Vector3(-offset.x, 0, offset.z);


    }

    public Vector2 getPosition()
    {
        return cPosition;
    }

    



}
