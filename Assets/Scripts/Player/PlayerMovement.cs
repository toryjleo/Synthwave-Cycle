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

    #region NeverUpdated
    [SerializeField] private Rigidbody rigidBody;
    /// <summary>
    /// Where to start at game start
    /// </summary>
    private Vector3 start_position = new Vector3(0, 1, 0);
    #endregion

    #region UpdatedOnCycle
    private Vector3 inputDirection = Vector3.zero;
    private Vector3 currentAcceleration = Vector3.zero;
    #endregion

    #region Tweakable
    /// <summary>
    /// The motion functions defining the velocity and acceleration
    /// </summary>
    [SerializeField] private MotionFunctions motionFunction;
    /// <summary>
    /// How fast to rotate the player when turning
    /// </summary>
    [SerializeField] private float rotationSpeed = 5;
    /// <summary>
    /// Amount of drag to apply to the tangent of movement
    /// </summary>
    [SerializeField] private float drag = 35;
    /// <summary>
    /// Angle off of inputDirection in which to start applying force upon forward vector aligning
    /// </summary>
    [SerializeField] private float theta = 20;
    /// <summary>
    /// Linear scale of velocity and acceleration
    /// </summary>
    [SerializeField] private float yScale = 20.0f;
    /// <summary>
    /// Linear scale of graph's x
    /// </summary>
    [SerializeField] private float xScale = 0;
    #endregion




    public float GetX {  get => motionFunction.GetXFromVelocity(Vector3.Dot(inputDirection.normalized, Velocity / yScale)); }
    /// <summary>
    /// The velocity of the rigidbody this cycle
    /// </summary>
    public Vector3 Velocity { get => rigidBody.velocity; }
    public Vector3 CurrentAcceleration { get => currentAcceleration; }
    public float YScale { get => yScale; }
    public MotionFunctions MotionFunctions { get { return motionFunction; } }


    // Start is called before the first frame update
    void Start()
    {
        transform.position = start_position;
        motionFunction = new Sigmoid1();
        // We will manually assign drag
        rigidBody.drag = 0;

        if ((Sigmoid1)motionFunction != null) 
        {
            xScale = ((Sigmoid1)motionFunction).xScale;
        }

        
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

        if ((Sigmoid1)motionFunction != null)
        {
            ((Sigmoid1)motionFunction).xScale = xScale;
        }

    }

    private void FixedUpdate()
    {

        ApplyAcceleration(inputDirection);

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

    private void ApplyAcceleration(Vector3 desiredDirection) 
    {
        Vector3 endLine1 = Quaternion.Euler(0, theta, 0) * transform.forward;
        Vector3 endLine2 = Quaternion.Euler(0, -theta, 0) * transform.forward;

        UnityEngine.Color color = UnityEngine.Color.red;

        if ((desiredDirection.sqrMagnitude > 0)) 
        {
            // If there is input
            float dot = Vector3.Dot(transform.forward, desiredDirection) / (transform.forward.magnitude * desiredDirection.magnitude);
            float angle = Mathf.Acos(dot);

            if (AngleLessThanTheta(angle, theta)) // Convert to radians
            {
                // Are pressing a direction and within that direction (can accelerate)
                color = UnityEngine.Color.green;

                // Apply acceleration
                currentAcceleration = motionFunction.Acceleration(GetX) * desiredDirection;
                rigidBody.AddForce(currentAcceleration * Time.fixedDeltaTime * yScale, ForceMode.Acceleration);
            }


        }
        else 
        {
            // Can Apply drag to current velocity
        }

        // Apply drag to the perpendicular velocity of the desiredDirection Vector
        ApplyDeceleration(transform.right, drag);

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
