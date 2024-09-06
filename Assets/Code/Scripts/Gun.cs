using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{

    /// <summary>
    /// Manages user inputs for the turret
    /// </summary>
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
        private Plane plane = new Plane(Vector3.up, 0);
        /// <summary>
        /// Tracking if we are currently using mouse input
        /// </summary>
        private bool isUsingMouse = true;

        /// <summary>
        /// Gets called every update
        /// </summary>
        public void UpdateInputMethod()
        {
            Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;
            lastMouseCoordinate = Input.mousePosition;

            Vector2 controllerInput = GetControllerInput();

            // Swap between mouse and controller input
            if (controllerInput != Vector2.zero)
            {
                isUsingMouse = false;
            }
            else if (mouseDelta.sqrMagnitude > minMouseMovementMagnitudeSqr)
            {
                isUsingMouse = true;
            }
        }

        /// <summary>
        /// Gets the crosshair's desired position this cycle
        /// </summary>
        /// <param name="transform">Turrer's transform</param>
        /// <returns>A vector3 of the desired crosshair position in world coordinates</returns>
        public Vector3 CrosshairPosition(in Transform transform)
        {
            if (isUsingMouse)
            {
                return Mouse(transform);
            }
            else
            {
                return Controller(transform);
            }
        }

        #region Controller
        /// <summary>
        /// Returns the crosshair's world position this cycle for controller
        /// </summary>
        /// <param name="transform">The turret's transform</param>
        /// <returns>the crosshair's world position this cycle</returns>
        private Vector3 Controller(in Transform transform)
        {
            UpdateScreenSize(transform.position.x);

            Vector2 controllerInput = GetControllerInput();

#if UNITY_EDITOR
            //circleRenderer.DrawCircle(transform.position, 20, transform.position.x - bottomScreenWorldPos.x);
#endif

            return GetCrosshairPosition_Controller(controllerInput, transform.position, transform.parent.parent.forward);

        }

        /// <summary>
        /// Needs to be called during FixedUpdate
        /// </summary>
        /// <param name="xPos">x position of turret in world coordinates</param>
        private void UpdateScreenSize(float xPos)
        {
            float distance;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0f, 0f, 0f));
            if (plane.Raycast(ray, out distance))
            {
                // Get height of screen
                Vector3 bottomScreenWorldPos = ray.GetPoint(distance);
                this.screenToWorldHeight = Mathf.Abs(xPos - bottomScreenWorldPos.x) * maxCrosshairDistAcrossScreen;
            }
        }

        /// <summary>
        /// Gets the input this cycle for controller
        /// </summary>
        /// <returns>A vector representing horizontal and vertical input</returns>
        private Vector2 GetControllerInput()
        {
            return new Vector2(Input.GetAxis(RIGHT_STICK_HORIZONTAL),
                               Input.GetAxis(RIGHT_STICK_VERTICAL));
        }

        /// <summary>
        /// Returns the crosshair's world position this cycle for controller.
        /// </summary>
        /// <param name="controllerInput">Input from the controller this cycle</param>
        /// <param name="position">position in world coordinates of the player vehicle</param>
        /// <param name="vehicleForward">Forward vector of the player vehicle</param>
        /// <returns>Vector3 of the crosshair's desired position</returns>
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
        #endregion


        #region Mouse
        /// <summary>
        /// Returns the crosshair's world position this cycle for controller. Is a wrapper
        /// </summary>
        /// <param name="transform">The turret's transform</param>
        /// <returns>the crosshair's world position this cycle</returns>
        private Vector3 Mouse(in Transform transform)
        {

            return GetCrosshairPosition_Mouse(transform);
        }

        /// <summary>
        /// Returns the crosshair's world position this cycle for controller
        /// </summary>
        /// <param name="transform">The turret's transform</param>
        /// <returns>the crosshair's world position this cycle</returns>
        private Vector3 GetCrosshairPosition_Mouse(in Transform transform)
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
        #endregion
    }


    private InputManager inputManager = null;

    /// <summary>
    /// Child gameObject with a SpriteRenderer of a crosshair
    /// </summary>
    [SerializeField] private GameObject crossHair;

    [SerializeField] private GameObject BulletSpawn;

    [SerializeField] private EditorObject.GunStats gunStats;

    private float nextTimeToFire = 0.0f;

    private int ammoCount = 0;


    // TODO: Create muzzle flash particlesystem with flashing point light
    // TODO: Create impact particlesystem with flashing point light
    #region Bullet Instancing
    protected BulletPool bulletPool;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int bulletPoolSize = 200;
    #endregion

    private PlayerMovement player;


    // Start is called before the first frame update
    void Start()
    {
        inputManager = new InputManager();
        InitializeBulletPool();
        player = FindObjectOfType<PlayerMovement>();


        crossHair.SetActive(gunStats.IsTurret);
        ResetGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (gunStats.IsTurret) 
        {
            inputManager.UpdateInputMethod();
        }

        UpdateGun();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gunStats.IsTurret && GameStateController.CanRunGameplay)
        {
            UpdateTurretDirection();
        }
    }

    public void ResetGameObject() 
    {
        ammoCount = gunStats.MagazineSize;
    }

    // TODO: have a method to add ammo and return remainder

    protected void InitializeBulletPool() 
    {
        bulletPool = gameObject.GetComponent<BulletPool>();
        if (bulletPool == null)
        {
            bulletPool = gameObject.AddComponent<BulletPool>();
        }
        bulletPool.Init(gunStats, bulletPrefab, bulletPoolSize);
    }

    private void UpdateGun() 
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire )
        {
            if (ammoCount > 0 || gunStats.InfiniteAmmo) 
            {
                nextTimeToFire = Time.time + (1f / gunStats.FireRate);
                switch (gunStats.BulletType)
                {
                    case EditorObject.BulletType.Projectile:
                        FireProjectile();
                        break;
                    case EditorObject.BulletType.HitScan:
                        FireHitScan();
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Updates the turret's and crosshair's transforms. Must be called on FixedUpdate()
    /// </summary>
    private void UpdateTurretDirection()
    {
        crossHair.transform.position = inputManager.CrosshairPosition(transform);
        transform.LookAt(crossHair.transform.position);
    }

    private void FireProjectile()
    {
        Bullet bullet = bulletPool.SpawnFromPool();
        Vector3 shotDir = BulletSpawn.transform.forward;

        bullet.Shoot(BulletSpawn.transform.position, shotDir, player.Velocity);

        UpdateAmmo();
    }

    private void FireHitScan()
    {
        RaycastHit hit;
        if (Physics.Raycast(BulletSpawn.transform.position, BulletSpawn.transform.forward, out hit, gunStats.Range)) 
        {
            Debug.Log(hit.transform.name);

            if ((hit.transform.tag == "Enemy" && gunStats.PlayerBullet) ||
                (hit.transform.tag == "Player" && !gunStats.PlayerBullet))
            {
                DealDamage(hit.transform.gameObject);
            }

        }
        // TODO: Play muzzleflash particlesystem
        UpdateAmmo();
    }

    private void UpdateAmmo() 
    {
        ammoCount = gunStats.InfiniteAmmo ? ammoCount : ammoCount - 1;
    }

    private void DealDamage(GameObject other)
    {
        Health otherHealth = other.GetComponentInChildren<Health>();
        if (otherHealth == null)
        {
            Debug.LogError("Object does not have Health component: " + gameObject.name);
        }
        else
        {
            otherHealth.TakeDamage(gunStats.DamageDealt);

            // TODO: instantiate and play impact effect particlesystem at hit.point and rotate to normal
            // https://www.youtube.com/watch?v=THnivyG0Mvo
        }

    }
}
