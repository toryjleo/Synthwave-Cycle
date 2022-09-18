using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovementComponent : MovementComponent
{
    public override void ApplyForces()
    {
        //Movement Forward and Back and applies velocity 
        appliedForce += ForwardVector().normalized * Acceleration * verticalInput * Time.fixedDeltaTime;


        //Steering Takes Horizontal Input and rotates both 
        float steerInupt = horizontalInput;
        MeshParent.transform.Rotate(Vector3.up * steerInupt * (appliedForce.magnitude + 100) * Time.fixedDeltaTime);

        //Drag and MaxSpeed Limit to prevent infinit velocity  
        appliedForce *= dragCoefficient;

        // Debug lines
        Debug.DrawRay(rb.transform.position, ForwardVector().normalized * 30, Color.red);
        Debug.DrawRay(rb.transform.position, appliedForce.normalized * 30, Color.blue);

        appliedForce = Vector3.Lerp(appliedForce.normalized, ForwardVector().normalized, Traction * Time.fixedDeltaTime) * appliedForce.magnitude;
        rb.AddForce(appliedForce);
    }

    public override void FixedUpdate()
    {
        ApplyForces();
    }

    public void control(float V, float H)
    {
        verticalInput = V;
        horizontalInput = H;
    }

    public override Vector3 ForwardVector()
    {
        return new Vector3(MeshParent.transform.forward.x, MeshParent.transform.forward.y, MeshParent.transform.forward.z);
    }
}
