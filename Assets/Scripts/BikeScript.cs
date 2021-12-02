using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeScript : MonoBehaviour
{
    public GameObject bikeMeshParent;
    public GameObject bikeMeshChild;
    public Vector2 position;
    public Vector3 deltaPosition;
    public Vector2 velocity;
    public Vector2 acceleration;
    private float mass = 1f; //mass of bike (DO NOT CHANGE)
    private float engineForce = 800f; // force that the engine 
    private float rotationSpeed = 80f;
    private float dragCoefficient = 2f;
    float speed = 1f;

    private float maxLean = 40.0f;

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2(0, 0);
        velocity = new Vector2(0, 0);
        acceleration = new Vector2(0, 0);
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public Vector3 GetDeltaPosition() 
    {
        return deltaPosition;
    }

    public void ApplyForce(Vector2 force)
    {
        if (force.sqrMagnitude != 0)
        {
            acceleration += force / mass;
        }
    }

    public void ClearRotation()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void MoveForward()
    {
        Vector2 forward = ForwardVector();
        ApplyForce(forward * engineForce);
    }

    public void MoveBackward()
    {
        Vector2 forward = ForwardVector();
        ApplyForce(-forward * engineForce);
    }

    public void TurnRight()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(-maxLean, 0, 0);
        RotateYAxis(-rotationSpeed * Time.fixedDeltaTime);
    }

    public void TurnLeft()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(maxLean, 0, 0);
        RotateYAxis(rotationSpeed * Time.fixedDeltaTime);
    }

    private Vector2 ForwardVector()
    {
        return new Vector2(-bikeMeshParent.transform.right.x, bikeMeshParent.transform.right.z);
    }

    public void ApplyForces()
    {
        ClearRotation();
        if (Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveBackward();
        }
        if (Input.GetKey(KeyCode.A))
        {
            TurnRight();
        }
        if (Input.GetKey(KeyCode.D))
        {
            TurnLeft();
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
        bikeMeshParent.transform.Rotate(0, speedAndDirection, 0, Space.Self);
    }


    public void UpdateLocations()
    {

        position += velocity * Time.fixedDeltaTime;
        deltaPosition = new Vector3(velocity.x, 0, velocity.y) * Time.fixedDeltaTime;
    }
}
