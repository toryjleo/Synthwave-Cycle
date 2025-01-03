using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using static PlayerHealth;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerMovement : MonoBehaviour
{
    #region TypeDefinition
    public enum GearType
    {
        Gear1,
        Gear2,
        Gear3,
        GearTypeInvalid
    }

    /// <summary>
    /// Handles tweakable variables which impacts movement
    /// </summary>
    private class Engine 
    {
        /// <summary>
        /// Copied over from editor. Scriptable object defining the gears
        /// </summary>
        List<EditorObject.Gear> gears;

        public float XScale { get { return gears[(int)currentGear].XScale; } }
        public float YScale { get { return gears[(int)currentGear].YScale; } }
        public float Theta { get { return gears[(int)currentGear].Theta; } }
        public float TangentDrag { get { return gears[(int)currentGear].TangentDrag; } }
        public float ForwardDrag { get { return gears[(int)currentGear].ForwardDrag; } }
        public float RotationSpeed { get { return gears[(int)currentGear].RotationSpeed; } }
        public float GraphTraversalSpeed { get { return gears[(int)currentGear].GraphTraversalSpeed; } }

        public GearType CurrentGear { get => currentGear; }


        private GearType maxGear;

        [SerializeField] private GearType currentGear;


        public Engine(List<EditorObject.Gear> gears, PlayerHealth playerHealth)
        {
            if (gears.Count != 3) 
            {
                Debug.LogError("Please check GameObject with PlayerMovement component to assure there are exactly 3 gears!");
            }
            this.gears = gears;
            currentGear = GearType.Gear1;
            playerHealth.onBarUpdate += HandleBarUpdate;
            HandleBarUpdate(playerHealth.CurrentBar, playerHealth.CurrentBar);
        }

        /// <summary>
        /// Updates the maxGear when the player health updates its bars
        /// </summary>
        /// <param name="playerHealth">A reference to player health</param>
        private void HandleBarUpdate(BarMax oldMax, BarMax newMax, bool hpIsOverBarMax3 = false)
        {
            maxGear = (GearType)newMax;

            if (currentGear > maxGear)
            {
                currentGear = maxGear;
            }
        }

        /// <summary>
        /// Called on Update to handle player input
        /// </summary>
        public void HandleGearInput()
        {
            if (Input.GetKeyDown(KeyCode.Q) && currentGear > GearType.Gear1)
            {
                // Shift down
                currentGear--;

            }
            else if (Input.GetKeyDown(KeyCode.E) && currentGear < maxGear)
            {
                // Shift up
                currentGear++;
            }
        }

        /// <summary>
        /// Get the top speed of a given gear
        /// </summary>
        /// <param name="gear">Gear to query</param>
        /// <returns>The top speed of the specified gear</returns>
        public float GetSpeedForGearAt(GearType gear)
        {
            if (gears.Count != 3) 
            {
                Debug.LogError("Not enough gears specified in PlayerMovement");
            }
            return gears[(int)gear].YScale;
        }

    }
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
    [SerializeField] private Vector3 start_position = new Vector3(0, 0.5f, 0);
    /// <summary>
    /// The motion functions defining the velocity and acceleration
    /// </summary>
    [SerializeField] private MotionFunctions motionFunction;
    #endregion

    #region UpdatedAtAwake
    private Engine gearManager = null;
    #endregion

    #region DefinedInPrefab
    /// <summary>
    /// Need exactly 3 Gear ScriptableObjects.
    /// </summary>
    [SerializeField] private List<EditorObject.Gear> gears;
    #endregion

    #region UpdatedOnCycle
    /// <summary>
    /// Normalized direction of input
    /// </summary>
    private Vector3 inputDirection = Vector3.zero;
    /// <summary>
    /// Magnitude of the non-normalized inputDirection
    /// </summary>
    private float inputMagnitude = 0;
    private Vector3 currentAcceleration = Vector3.zero;
    #endregion

    #region Properties
    public float GetX {  get => motionFunction.GetXFromVelocity(Vector3.Dot(inputDirection, Velocity)/gearManager.YScale); }
    /// <summary>
    /// The velocity of the rigidbody this cycle
    /// </summary>
    public Vector3 Velocity { get => rigidBody.velocity; }
    public Vector3 CurrentAcceleration { get => currentAcceleration; }
    public float YScale { get => gearManager.YScale; }
    public MotionFunctions MotionFunctions { get { return motionFunction; } }
    public GearType CurrentGear { 
        get { 
            if (gearManager != null) 
            {
                return gearManager.CurrentGear;
            }
            else 
            {
                return GearType.GearTypeInvalid;
            }
            
        } 
    }

    public float TopSpeedMovementGroup1
    {
        get { return gearManager.GetSpeedForGearAt(GearType.Gear1); }
    }

    public float TopSpeedMovementGroup2
    {
        get { return gearManager.GetSpeedForGearAt(GearType.Gear2); }
    }

    public float TopSpeedMovementGroup3
    {
        get { return gearManager.GetSpeedForGearAt(GearType.Gear3); }
    }
    #endregion

    #region MonoBehavior
    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null) 
        {
            Debug.LogWarning("Could not find PlayerHealth on object!");
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

        ApplyInitialState();
        motionFunction = new Sigmoid1();
        gearManager = new Engine(gears, playerHealth);
    }

    private void Start()
    {
        if (GameStateController.StateExists)
        {
            GameStateController.resetting.notifyListenersEnter += ApplyInitialState;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateController.CanRunGameplay)
        {
            UpdateInputDir();

            gearManager.HandleGearInput();

            if (inputMagnitude != 0)
            {
                Quaternion newRotation = Quaternion.LookRotation(inputDirection, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * gearManager.RotationSpeed);
            }

            if ((Sigmoid1)motionFunction != null)
            {
                ((Sigmoid1)motionFunction).xScale = gearManager.XScale;
            }
        }
    }

    private void FixedUpdate()
    {

        ApplyMovementForces(inputDirection, inputMagnitude);

    }
    #endregion

    private void UpdateInputDir() 
    {
        // Note: using vertical axis to represent +x and horizontal axis to represent -z
        float horizontalAxis = Input.GetAxis(HORIZONTAL);
        float verticalAxis = Input.GetAxis(VERTICAL);

        Vector3 desiredDirection = new Vector3(verticalAxis, 0, -horizontalAxis);
        float magnitude = Mathf.Clamp(desiredDirection.magnitude, 0, 1);
        Vector3 desiredDirectionNormalized = Vector3.Normalize(desiredDirection);


        Debug.DrawLine(transform.position, transform.position + transform.forward, UnityEngine.Color.red);
        Debug.DrawLine(transform.position, transform.position + (desiredDirectionNormalized * magnitude), UnityEngine.Color.magenta);

        inputDirection = desiredDirectionNormalized;
        inputMagnitude = magnitude;
    }

    private void ApplyMovementForces(Vector3 desiredDirection, float desiredDirectionMagnitude) 
    {
        Vector3 endLine1 = Quaternion.Euler(0, gearManager.Theta, 0) * transform.forward;
        Vector3 endLine2 = Quaternion.Euler(0, -gearManager.Theta, 0) * transform.forward;

        UnityEngine.Color color = UnityEngine.Color.red;

        if (desiredDirectionMagnitude > 0)
        {
            // If there is input
            float dot = Vector3.Dot(transform.forward, desiredDirection) / (transform.forward.magnitude * desiredDirectionMagnitude);
            float angle = Mathf.Acos(dot);

            if (AngleLessThanTheta(angle, gearManager.Theta)) // Convert to radians
            {
                // Are pressing a direction and within that direction (can accelerate)
                color = UnityEngine.Color.green;

                // Apply acceleration
                currentAcceleration = motionFunction.Acceleration( GetX) * desiredDirection * desiredDirectionMagnitude * gearManager.YScale;
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
            Debug.Log("Applying resestance");
            ApplyDeceleration(transform.forward, gearManager.ForwardDrag);
        }

        // Apply drag to the perpendicular velocity of the desiredDirection Vector
        ApplyDeceleration(transform.right, gearManager.TangentDrag);

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

    private void ApplyInitialState() 
    {
        transform.position = start_position;
        transform.rotation = Quaternion.identity;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.Sleep();
    }
}
