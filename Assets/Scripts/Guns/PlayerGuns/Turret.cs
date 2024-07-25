using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// This Script Controlls the turret attached to the turret bike. It requires a GameObject for the muzzle and keeps 
/// track of the mouse coordinates.
/// </summary>
public class Turret : Gun
{
    private class InputManager 
    {

        #region InputManagerStrings
        private const string RIGHT_STICK_HORIZONTAL = "RightStickHorizontal";
        private const string RIGHT_STICK_VERTICAL = "RightStickVertical";
        #endregion

        #region MouseVariables
        private Vector3 lastMouseCoordinate = Vector3.zero;
        /// <summary>
        /// How far mouse must move to switch to using mouse input
        /// </summary>
        private float minMouseMovementMagnitudeSqr = 4;
        #endregion

        #region ControllerVariables
        /// <summary>
        /// Used to keep the crosshair on screen for controller 
        /// </summary>
        private float maxCrosshairDistAcrossScreen = .95f;
        /// <summary>
        /// How far to maintain the crosshair behind player in world units
        /// </summary>
        private float distBehindPlayerToFollow = 1.5f;
        private float screenToWorldHeight = 0.0f;
        #endregion

        /// <summary>
        /// Plane used to detect raycast hits from camera
        /// </summary>
        Plane plane = new Plane(Vector3.up, 0);
        /// <summary>
        /// Tracking if we are currently using mouse input
        /// </summary>
        public bool isUsingMouse = true;

        public void Update()
        {
            Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;
            lastMouseCoordinate = Input.mousePosition;

            // Swap between mouse and controller input
            Vector2 controllerInput = new Vector2(Input.GetAxis(RIGHT_STICK_HORIZONTAL), Input.GetAxis(RIGHT_STICK_VERTICAL));

            if (controllerInput != Vector2.zero)
            {
                isUsingMouse = false;
            }
            else if (mouseDelta.sqrMagnitude > minMouseMovementMagnitudeSqr)
            {
                isUsingMouse = true;
            }
        }

        public Vector3 CrosshairPosition(Transform transform) 
        {
            if (isUsingMouse)
            {
                return Mouse(transform);
            }
            else
            {
                return Controller(transform.position, transform);
            }
        }

        #region Controller

        /// <summary>
        /// Needs to be called during FixedUpdate
        /// </summary>
        /// <param name="xPos"></param>
        private void UpdateScreenSize(float xPos)
        {
            float distance;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0f, 0f, 0f));
            if (plane.Raycast(ray, out distance))
            {
                // Get height of screen
                Vector3 bottomScreenWorldPos = ray.GetPoint(distance);
                this.screenToWorldHeight = Mathf.Abs(xPos - (bottomScreenWorldPos.x) * maxCrosshairDistAcrossScreen);
            }
        }

        private Vector2 GetControllerInput() 
        {
            return new Vector2(Input.GetAxis(RIGHT_STICK_HORIZONTAL),
                               Input.GetAxis(RIGHT_STICK_VERTICAL));
        }

        private Vector3 GetCrosshairPosition_Controller(Vector2 controllerInput, Vector3 position, Vector3 vehicleForward) 
        {
            if (controllerInput != Vector2.zero)
            {
                Vector3 desiredDirection = new Vector3(-controllerInput.y, 0, -controllerInput.x);
                float magnitude = Mathf.Clamp(desiredDirection.magnitude, 0, 1);
                Vector3 desiredDirectionNormalized = Vector3.Normalize(desiredDirection);
                Debug.DrawLine(position, position + desiredDirectionNormalized);

                // Move crosshair in to position
                Vector3 crossHairPos = position + (magnitude * screenToWorldHeight * desiredDirectionNormalized);
                return new Vector3(crossHairPos.x, position.y, crossHairPos.z);
            }
            else
            {
                // Default behind player
                return position - (vehicleForward * distBehindPlayerToFollow);
            }
        }

        public Vector3 Controller(Vector3 position, Transform transform)
        {
            UpdateScreenSize(position.x);

            Vector2 controllerInput = GetControllerInput();

#if UNITY_EDITOR
            //circleRenderer.DrawCircle(transform.position, 20, transform.position.x - bottomScreenWorldPos.x);
#endif

            return GetCrosshairPosition_Controller(controllerInput, position, transform.parent.parent.forward);

        }
        #endregion


        #region Mouse

        private Vector3 GetCrosshairPosition_Mouse(Transform transform) 
        {
            float distance;
            Vector3 mouseWorldPos = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                mouseWorldPos = ray.GetPoint(distance);
                
            }
            return new Vector3(mouseWorldPos.x,
                               transform.position.y,
                               mouseWorldPos.z);
        }

        public Vector3 Mouse(Transform transform)
        {

             return GetCrosshairPosition_Mouse(transform);
        }
        #endregion
    }

    private InputManager inputManager = null;

    /// <summary>
    /// Child gameObject with a SpriteRenderer of a crosshair
    /// </summary>
    [SerializeField] private GameObject crossHair;
    /// <summary>
    /// How far to spawn bullets down the forward vector
    /// </summary>
    private float distanceToBulletSpawn = .9f;

    #region Debug
    private CircleRendererScript circleRenderer;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        infiniteAmmo = true;
        circleRenderer = GetComponent<CircleRendererScript>();
        inputManager = new InputManager();
    }

    private void Update()
    {
        inputManager.Update();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameStateController.CanRunGameplay)
        {
            UpdateTurretDirection();
        }
    }

    public override void Init()
    {
        base.Init();
        lastFired = 0;
        fireRate = 10;
    }

    private void UpdateTurretDirection()
    {
        crossHair.transform.position = inputManager.CrosshairPosition(transform);
        transform.LookAt(crossHair.transform.position);
    }

    public override PlayerWeaponType GetPlayerWeaponType()
    {
        return PlayerWeaponType.PowerGlove;
    }

    public override void PrimaryFire(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            Bullet bullet = bulletPool.SpawnFromPool();

            Vector3 shotDir = transform.forward;
            bullet.Shoot(transform.position + transform.forward * distanceToBulletSpawn, shotDir, initialVelocity);

            //Debug.Log("MuzzleVelocity: " + bullet.MuzzleVelocity);
            //Debug.Log("Mass: " + bullet.Mass);
            //Debug.Log("shotDir: " + shotDir.ToString());
            // Gun specific
            
            OnBulletShot(shotDir * bullet.Mass * bullet.MuzzleVelocity);
        }


    }

    public override void ReleasePrimaryFire(Vector3 initialVelocity)
    {
    }

    public override void SecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }

    public override void ReleaseSecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }
}
