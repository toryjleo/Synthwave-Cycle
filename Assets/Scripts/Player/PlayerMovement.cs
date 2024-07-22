using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private class GearManager 
    {
        private enum GearType
        {
            Gear1,
            Gear2,
            Gear3,
        }

        List<EditorObject.Gear> gears;

        public float XScale { get { return xScale; } }
        public float YScale { get { return yScale; } }
        public float Theta { get { return theta; } }
        public float Drag { get { return drag; } }
        public float RotationSpeed { get { return rotationSpeed; } }
        public float GraphTraversalSpeed { get { return graphTraversalSpeed; } }

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
        [SerializeField] private float xScale = 1;
        /// <summary>
        /// Speeds up the time it takes to get to full velocity on the graph
        /// </summary>
        [SerializeField] private float graphTraversalSpeed = 1;


        private GearType maxGear;

        [SerializeField] private GearType currentGear;


        public GearManager(List<EditorObject.Gear> gears)
        {
            this.gears = gears;
            currentGear = GearType.Gear1;
        }

        public void HandleBarUpdate(PlayerHealth playerHealth)
        {
            maxGear = (GearType)playerHealth.CurrentBar;

            if (currentGear > maxGear)
            {
                currentGear = maxGear;
                ShiftGear();
            }
        }


        /// <summary>
        /// Changes the movement perameters of this object to match the current gear
        /// </summary>
        public void ShiftGear()
        {
            EditorObject.Gear gear = gears[(int)currentGear];

            yScale = gear.YScale;
            xScale = gear.XScale;
            theta = gear.Theta;
            drag = gear.Drag;
            rotationSpeed = gear.RotationSpeed;
            graphTraversalSpeed = gear.GraphTraversalSpeed;
        }

        public void HandleGearInput()
        {
            if (Input.GetKeyDown(KeyCode.Q) && currentGear > GearType.Gear1)
            {
                // Shift down
                currentGear--;
                ShiftGear();

            }
            else if (Input.GetKeyDown(KeyCode.E) && currentGear < maxGear)
            {
                // Shift up
                currentGear++;
                ShiftGear();
            }
        }

    }
    #region Gears

    [SerializeField] private List<EditorObject.Gear> gears;

    private GearManager gearManager = null;
    #endregion

    #region InputManagerStrings
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    #endregion

    #region NeverUpdated
    private Rigidbody rigidBody;
    private PlayerHealth playerHealth;
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

    #endregion

    #region Properties
    public float GetX {  get => motionFunction.GetXFromVelocity(Vector3.Dot(inputDirection.normalized, Velocity)/gearManager.YScale); }
    /// <summary>
    /// The velocity of the rigidbody this cycle
    /// </summary>
    public Vector3 Velocity { get => rigidBody.velocity; }
    public Vector3 CurrentAcceleration { get => currentAcceleration; }
    public float YScale { get => gearManager.YScale; }
    public MotionFunctions MotionFunctions { get { return motionFunction; } }
    #endregion


    void Awake()
    {
        transform.position = start_position;
        motionFunction = new Sigmoid1();
        gearManager = new GearManager(gears);

        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null) 
        {
            Debug.LogWarning("Could not find PlayerHealth on object!");
        }
        else 
        {
            playerHealth.barUpdated += gearManager.HandleBarUpdate;
        }

        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody == null)
        {
            Debug.LogWarning("Could not find RigidBody on object!");
        }
        else 
        {
            // We will manually assign drag
            rigidBody.drag = 0;
        }
        gearManager.HandleBarUpdate(playerHealth);
        gearManager.ShiftGear();
    }

    // Update is called once per frame
    void Update()
    {
        inputDirection = GetInputDir();

        gearManager.HandleGearInput();

        if (inputDirection != Vector3.zero) 
        {
            Quaternion newRotation = Quaternion.LookRotation(inputDirection, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * gearManager.RotationSpeed);
        }

        if ((Sigmoid1)motionFunction != null)
        {
            ((Sigmoid1)motionFunction).xScale = gearManager.XScale;
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
        Vector3 endLine1 = Quaternion.Euler(0, gearManager.Theta, 0) * transform.forward;
        Vector3 endLine2 = Quaternion.Euler(0, -gearManager.Theta, 0) * transform.forward;

        UnityEngine.Color color = UnityEngine.Color.red;

        if ((desiredDirection.sqrMagnitude > 0)) 
        {
            // If there is input
            float dot = Vector3.Dot(transform.forward, desiredDirection) / (transform.forward.magnitude * desiredDirection.magnitude);
            float angle = Mathf.Acos(dot);

            if (AngleLessThanTheta(angle, gearManager.Theta)) // Convert to radians
            {
                // Are pressing a direction and within that direction (can accelerate)
                color = UnityEngine.Color.green;

                // Apply acceleration
                currentAcceleration = motionFunction.Acceleration( GetX) * desiredDirection * gearManager.YScale;
                rigidBody.AddForce(currentAcceleration * Time.fixedDeltaTime * gearManager.GraphTraversalSpeed, ForceMode.Acceleration);
            }


        }
        else 
        {
            // Can Apply drag to current velocity
        }

        // Apply some drag to the forward vector if going over max velocity
        if (rigidBody.velocity.sqrMagnitude > (gearManager.YScale * gearManager.YScale))
        {
            ApplyDeceleration(transform.forward, gearManager.Drag);
        }

        // Apply drag to the perpendicular velocity of the desiredDirection Vector
        ApplyDeceleration(transform.right, gearManager.Drag);

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
