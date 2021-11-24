using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeScript : MonoBehaviour
{
    public GameObject bikeMesh;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 acceleration;
    private float mass = 1f;
    private float engineForce = 1f;
    private float rotationSpeed = 30f;
    private float dragCoefficient = 10f;
    float speed = .004f;

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2(0, 0);
        velocity = new Vector2(0, 0);
        acceleration = new Vector2(0, 0);
    }

    public Vector2 GetPosition()
    {
        return position;
    }

    public void ApplyForce(Vector2 force)
    {
        if (force.sqrMagnitude != 0)
        {
            acceleration += force / mass;
        }
    }

    public void UpdateNextMovement()
    {
        Vector2 forward = new Vector2(-bikeMesh.transform.right.x, bikeMesh.transform.right.z);
        // Force of engine
        if (Input.GetKey(KeyCode.W))
        {
            ApplyForce(forward * engineForce);
            //ApplyForce(new Vector2(engineForce, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            ApplyForce(-forward * engineForce);
        }
        if (Input.GetKey(KeyCode.A))
        {
            RotateYAxis(-rotationSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            RotateYAxis(rotationSpeed * Time.fixedDeltaTime);
        }

        Vector2 dragForce = velocity * velocity * dragCoefficient / 2;
        dragForce = -velocity.normalized * dragForce;

        ApplyForce(dragForce);

        // Update velocity
        velocity += acceleration * Time.fixedDeltaTime;

        // Reset acceleration for next update
        acceleration = new Vector2(0, 0);
    }

    private void RotateYAxis(float speedAndDirection)
    {
        bikeMesh.transform.Rotate(0, speedAndDirection, 0, Space.Self);
    }


    public void UpdateLocations()
    {

        position += velocity * Time.fixedDeltaTime;

    }

    private void ScrollMovement()
    {
        if (Input.GetKey(KeyCode.W)) { position.y += 1 * speed; }
        if (Input.GetKey(KeyCode.S)) { position.y -= 1 * speed; }
        if (Input.GetKey(KeyCode.A)) { position.x -= 1 * speed; }
        if (Input.GetKey(KeyCode.D)) { position.x += 1 * speed; }
    }
}
