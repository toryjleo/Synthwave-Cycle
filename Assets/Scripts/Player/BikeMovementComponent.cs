using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the movement behavior our bike uses
/// </summary>
public class BikeMovementComponent : MovementComponent
{

    #region Forces
    /// <summary> Main method for controlling bike 
    /// Applies forces to Rigid body in relation to player input 
    /// </summary>
    public override void ApplyForces()
    {
        //Movement Forward and Back and applies velocity 
        appliedForce += ForwardVector().normalized * Acceleration * verticalInput * Time.fixedDeltaTime;


        //Steering Takes Horizontal Input and rotates both 
        float steerInupt = horizontalInput;
        MeshChild.transform.localRotation = Quaternion.Euler(maxLean * steerInupt, 0, 0);
        MeshParent.transform.Rotate(Vector3.up * steerInupt * (appliedForce.magnitude + 100) * Time.fixedDeltaTime);

        //Drag and MaxSpeed Limit to prevent infinit velocity  
        appliedForce *= dragCoefficient;

        // Debug lines
        Debug.DrawRay(rb.transform.position, ForwardVector().normalized * 30, Color.red);
        Debug.DrawRay(rb.transform.position, appliedForce.normalized * 30, Color.blue);

        appliedForce = Vector3.Lerp(appliedForce.normalized, ForwardVector().normalized, Traction * Time.fixedDeltaTime) * appliedForce.magnitude;
        rb.AddForce(appliedForce);
    }

    /// <summary>Responds to the gun'd NotifyShot event.</summary>
    /// <param name="forceOfBulletOnBike">The force of the bullet to apply to the bike.</param>
    public void bl_ProcessCompleted(Vector3 forceOfBulletOnBike)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(forceOfBulletOnBike.x, forceOfBulletOnBike.y, forceOfBulletOnBike.z));
    }

    #endregion
}
