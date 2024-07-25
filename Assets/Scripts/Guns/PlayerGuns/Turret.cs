using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Script Controlls the turret attached to the turret bike. It requires a GameObject for the muzzle and keeps 
/// track of the mouse coordinates.
/// </summary>
public class Turret : Gun
{
    private class InputManager 
    {
        #region outside
        /// <summary>
        /// Child gameObject with a SpriteRenderer of a crosshair
        /// </summary>
        [SerializeField] private GameObject crossHair;
        #endregion


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
        #endregion

        /// <summary>
        /// Plane used to detect raycast hits from camera
        /// </summary>
        Plane plane = new Plane(Vector3.up, 0);
        /// <summary>
        /// Tracking if we are currently using mouse input
        /// </summary>
        public bool isUsingMouse = true;


        public InputManager(GameObject crosshair) 
        {
            this.crossHair = crosshair;
        }

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

        public void Controller(Vector3 position, Transform transform)
        {

            float distance;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0f, 0f, 0f));
            if (plane.Raycast(ray, out distance))
            {
                // Get height of screen
                Vector3 bottomScreenWorldPos = ray.GetPoint(distance);
                float screenToWorldHeight = Mathf.Abs(position.x - (bottomScreenWorldPos.x) * maxCrosshairDistAcrossScreen);

                // Get controller input
                Vector2 controllerInput = new Vector2(Input.GetAxis(RIGHT_STICK_HORIZONTAL),
                                          Input.GetAxis(RIGHT_STICK_VERTICAL));

                if (controllerInput != Vector2.zero)
                {
                    Vector3 desiredDirection = new Vector3(-controllerInput.y, 0, -controllerInput.x);
                    float magnitude = Mathf.Clamp(desiredDirection.magnitude, 0, 1);
                    Vector3 desiredDirectionNormalized = Vector3.Normalize(desiredDirection);
                    Debug.DrawLine(position, position + desiredDirectionNormalized);

                    // Move crosshair in to position
                    Vector3 crossHairPos = position + (magnitude * screenToWorldHeight * desiredDirectionNormalized);
                    crossHair.transform.position = new Vector3(crossHairPos.x,
                                                    position.y,
                                                    crossHairPos.z);
                }
                else
                {
                    crossHair.transform.position = position - (transform.parent.parent.forward * distBehindPlayerToFollow);
                }

#if UNITY_EDITOR
                //circleRenderer.DrawCircle(transform.position, 20, transform.position.x - bottomScreenWorldPos.x);
#endif
                transform.LookAt(crossHair.transform.position);
            }

        }


        public void Mouse(Transform transform)
        {
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                Vector3 mouseWorldPos = ray.GetPoint(distance);
                crossHair.transform.position = new Vector3(mouseWorldPos.x,
                                                            transform.position.y,
                                                            mouseWorldPos.z);
                Vector3 playerToMouse = mouseWorldPos - transform.position;
                var angle = Mathf.Atan2(playerToMouse.x, playerToMouse.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }
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
        inputManager = new InputManager(crossHair);
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

        if (inputManager.isUsingMouse)
        {
            inputManager.Mouse(transform);
        }
        else 
        {
            inputManager.Controller(transform.position, transform);
        }

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
