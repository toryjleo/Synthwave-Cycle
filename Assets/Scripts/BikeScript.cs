using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BikeScript</c> A Unity Component which holds the logic for the Player movement.</summary>
///
public class BikeScript : MonoBehaviour
{
    public GameObject bikeMeshParent; // Parent of the bike mesh. This is used to get the forward vector of the bike. 
    // The forward vector of the bike will change as we alter the rotation of this variable
    public GameObject bikeMeshChild; // The gameObject that holds the bike mesh. This will only be used for animations.
    public Vector3 deltaPosition; // How far the bike has moved in world coordinates this frame
    public Vector2 velocity; // The velocity of the bike
    public Vector2 acceleration; // The acceleration of the bike this frame

    private float mass = 8f; // The mass of the bike
    private float engineForce = 800f; // The force of the engine
    private float rotationSpeed = 120f; // A linear scale of how fast the bike will turn
    private float dragCoefficient = 1f; // A linear scale of how much drag will be applied to the bike

    private float maxLean = 40.0f;


    public Gun currentGun;


    // Start is called before the first frame update
    void Start()
    {
        EquipGun(currentGun);
        // The bike will begin at rest
        velocity = new Vector2(0, 0);
        acceleration = new Vector2(0, 0);
    }


    /// <summary>Property for distance travelled this frame in world coordinates.</summary>
    public Vector3 DeltaPosition
    {
        get => deltaPosition;
    }

    /// <summary>Updates the acceleration of the bike this frame.</summary>
    /// /// <param name="force">A vector containing both the direction and magnatude of the force to be 
    /// applied.</param>
    public void ApplyForce(Vector2 force)
    {
        if (force.sqrMagnitude != 0)
        {
            acceleration += force / mass;
        }
    }

    /// <summary>Clears the rotation of the child mesh object.</summary>
    public void ClearRotation()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>Applies a force in the direction of the bike's forward vector.</summary>
    public void MoveForward()
    {
        Vector2 forward = ForwardVector().normalized;
        ApplyForce(forward * engineForce);
    }

    /// <summary>Applies a force in the opposite direction of the bike's forward vector.</summary>
    public void MoveBackward()
    {
        Vector2 forward = ForwardVector().normalized;
        ApplyForce(-forward * engineForce);
    }

    /// <summary>Rotates the bike's mesh in a clockwise fashion.</summary>
    public void TurnRight()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(-maxLean, 0, 0);
        RotateYAxis(-rotationSpeed * Time.fixedDeltaTime); // A simple animation
    }

    /// <summary>Rotates the bike's mesh in a counter-clockwise fashion.</summary>
    public void TurnLeft()
    {
        bikeMeshChild.transform.localRotation = Quaternion.Euler(maxLean, 0, 0);
        RotateYAxis(rotationSpeed * Time.fixedDeltaTime); // A simple animation
    }

    /// <summary>This method gets the direction the bike's mesh is currently facing in world coordinates.</summary>
    /// <returns>A Vector2 of the bike's forward vector in world coordinates. The ector's x represents the x direction 
    /// in world coordinates and the vector's y represents the z direction in world coordinates.</returns>
    private Vector2 ForwardVector()
    {
        return new Vector2(-bikeMeshParent.transform.right.x, bikeMeshParent.transform.right.z);
    }

    private Vector2 RightVector() 
    {
        return new Vector2(-bikeMeshParent.transform.forward.x, bikeMeshParent.transform.forward.z);
    }

    /// <summary>Applies all of the bike's internaly applied forces. Also gets player input.</summary>
    public void ApplyForces()
    {
        ClearRotation();
        // Apply player input
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

        // Apply Drag
        Vector2 dragForce = velocity * velocity * dragCoefficient / 2;
        dragForce = -velocity.normalized * dragForce;

        ApplyForce(dragForce);

        // Update velocity based on the acceleration this frame
        velocity += acceleration * Time.fixedDeltaTime;

        // Reset acceleration for next update
        acceleration = new Vector2(0, 0);
    }

    private void Shoot() 
    {
    
    }

    /// <summary>Sets the bikeMeshParent's local yAngle to the unput float.</summary>
    /// <param name="speedAndDirection">The yAngle at which to set bikeMeshParent</param>
    private void RotateYAxis(float speedAndDirection)
    {
        bikeMeshParent.transform.Rotate(0, speedAndDirection, 0, Space.Self);
    }

    /// <summary>Applies the net velocity of this bike to get the distance travelled this frame.</summary>
    public void UpdateLocations()
    {
        deltaPosition = new Vector3(velocity.x, 0, velocity.y) * Time.fixedDeltaTime;
    }

    public void bl_ProcessCompleted(Vector3 forceOfBulletOnBike)
    {
        ApplyForce(new Vector2(forceOfBulletOnBike.x, forceOfBulletOnBike.z));
        velocity += acceleration * Time.fixedDeltaTime;

        // Reset acceleration for next update
        acceleration = new Vector2(0, 0);
    }

    public void EquipGun(Gun gunToEquip) 
    {
        // Make gun child of TracerMeshParent
        gunToEquip.transform.parent = bikeMeshParent.transform;

        // Hook up event
        gunToEquip.BulletShot += bl_ProcessCompleted;
    }
}
