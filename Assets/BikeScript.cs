using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeScript : MonoBehaviour
{

    Vector2 position;
    float speed = .001f;

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) { position.y += 1 * speed; }
        if (Input.GetKey(KeyCode.S)) { position.y -= 1 * speed; }
        if (Input.GetKey(KeyCode.A)) { position.x -= 1 * speed; }
        if (Input.GetKey(KeyCode.D)) { position.x += 1 * speed; }
    }

    public Vector2 GetPosition() 
    {
        return position;
    }
}
