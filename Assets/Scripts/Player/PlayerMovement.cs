using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region InputManagerStrings
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    #endregion

    [SerializeField] private Rigidbody rigidBody;

    private Vector3 start_position = new Vector3(0, 1, 0);
    private Vector3 inputDirection = Vector3.zero;

    public float rotationSpeed = 5;
    public float theta = 20;
    public float forceToApplyPerSecond = 100;

    public MotionFunctions motionFunction;

    public float yScale = 20.0f;
    public Vector3 currentAcceleration = Vector3.zero;


    public float GetX {  get => motionFunction.GetXFromVelocity(Vector3.Dot(inputDirection.normalized, Velocity / yScale)); }
    /// <summary>
    /// The velocity of the rigidbody this cycle
    /// </summary>
    public Vector3 Velocity { get => rigidBody.velocity; }


    // Start is called before the first frame update
    void Start()
    {
        transform.position = start_position;
        motionFunction = new Sigmoid1();
        rigidBody.drag = 0;
    }

    // Update is called once per frame
    void Update()
    {
        inputDirection = GetInputDir();

        if (inputDirection != Vector3.zero) 
        {
            Quaternion newRotation = Quaternion.LookRotation(inputDirection, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
        }



    }
    private void FixedUpdate()
    {

        DrawTheta(inputDirection);
        //rigidBody.AddForce(inputDirection);

    }

    private Vector3 GetInputDir() 
    {
        // Note: using vertical axis to represent +x and horizontal axis to represent -z
        float horizontalAxis = Input.GetAxis(HORIZONTAL);
        float verticalAxis = Input.GetAxis(VERTICAL);

        Vector3 desiredDirection = new Vector3(verticalAxis, 0, -horizontalAxis);
        Vector3 desiredDirectionNormalized = Vector3.Normalize(desiredDirection);
        float magnitude = Mathf.Clamp(desiredDirection.magnitude, 0, 1);


        Debug.DrawLine(transform.position, transform.position + transform.forward, UnityEngine.Color.red);
        Debug.DrawLine(transform.position, transform.position + (desiredDirectionNormalized * magnitude), UnityEngine.Color.magenta);

        return desiredDirectionNormalized * magnitude;
    }

    private void DrawTheta(Vector3 desiredDirection) 
    {
        Vector3 endLine1 = Quaternion.Euler(0, theta, 0) * transform.forward;
        Vector3 endLine2 = Quaternion.Euler(0, -theta, 0) * transform.forward;

        UnityEngine.Color color = UnityEngine.Color.red;

        if (inputDirection != Vector3.zero) 
        {
            // If there is input
            float dot = Vector3.Dot(transform.forward, desiredDirection) / (transform.forward.magnitude * desiredDirection.magnitude);
            float angle = Mathf.Acos(dot);

            if (AngleLessThanTheta(angle, theta)) // Convert to radians
            {
                // Are pressing a direction and within that direction (can accelerate)
                color = UnityEngine.Color.green;
            }

            if ((desiredDirection.sqrMagnitude > 0)) 
            {
                
                if (AngleLessThanTheta(angle, theta)) 
                {
                    // Apply acceleration
                    currentAcceleration = motionFunction.Acceleration(GetX) * desiredDirection;
                    rigidBody.AddForce(currentAcceleration * Time.fixedDeltaTime * yScale, ForceMode.Acceleration);
                }
            }
            else 
            {
                // decelerate quickly
                //Vector3 dir = rigidBody.velocity;
                //rigidBody.AddForce(-dir * forceToApplyPerSecond * Time.fixedDeltaTime);
            }

            // Apply drag to the perpendicular velocity of the inputDirection Vector
            Vector3 inputRight = Quaternion.AngleAxis(90, Vector3.up) * inputDirection.normalized;
            ApplyDeceleration(inputRight);


        }
        else 
        {
            // Apply drag to current velocity
        }

        Debug.DrawLine(transform.position, transform.position + endLine1, color);
        Debug.DrawLine(transform.position, transform.position + endLine2, color);
    }

    /// <summary>
    /// Decelerates along the normalized axis, axisToDecelerate. Call on FixedUpdate
    /// </summary>
    /// <param name="axisToDecelerate">Axis to decelerate normalized</param>
    /// <param name="decelerationScale">A linear scale amount to decelerate</param>
    private void ApplyDeceleration(Vector3 axisToDecelerate, float decelerationScale = 35) 
    {
        float currentSpeedOnAxis = Vector3.Dot(axisToDecelerate, rigidBody.velocity);
        rigidBody.AddForce(-axisToDecelerate * currentSpeedOnAxis * decelerationScale * Time.fixedDeltaTime, ForceMode.Acceleration);

        Debug.DrawLine(transform.position, transform.position + axisToDecelerate, UnityEngine.Color.blue);
    }

    /// <summary>
    /// Returns if a given angle, in radians, is less than theta in degrees.
    /// </summary>
    /// <param name="angle">In radians</param>
    /// <param name="theta">in Degrees</param>
    /// <returns>A boolean stating if angle < theta</returns>
    private bool AngleLessThanTheta(float angle, float theta) 
    {
        if (Mathf.Abs(angle) < (theta * Mathf.PI / 180)) // Convert to radians
        {
            return true;
        }
        return false;
    }
}
