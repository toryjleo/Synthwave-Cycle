using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusScript : MonoBehaviour
{

    public Vector3 cPosition;
    public int HP;
    public bool alive; 
    
    
    public void grow(Vector2 spawnPoint)
    {
        HP = 1;
        alive = true;
        //Instantiate SPAWN SAID CACTUS 
    }

    public void move(Vector3 offset)
    {

        print(offset);
        cPosition -= offset;
        transform.position = cPosition;
    }

    public Vector2 getPosition()
    {
        return cPosition;
    }

    



}
