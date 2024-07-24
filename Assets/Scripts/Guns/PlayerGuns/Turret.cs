using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Script Controlls the turret attached to the turret bike. It requires a GameObject for the muzzle and keeps track of the mouse coordinates. 
/// </summary>
public class Turret : Gun
{
    private const string RIGHT_STICK_HORIZONTAL = "RightStickHorizontal";
    private const string RIGHT_STICK_VERTICAL = "RightStickVertical";

    private float distanceToBulletSpawn = .9f;
    [SerializeField] private GameObject crossHair;

    [SerializeField] private bool usingMouse = true;

    #region MouseVariables
    private Vector3 lastMouseCoordinate = Vector3.zero;
    private float minMouseMovementMagnitudeSqr = 4;
    Plane plane = new Plane(Vector3.up, 0);
    #endregion

    #region ControllerVariables
    /// <summary>
    /// Normalized direction of input
    /// </summary>
    private Vector3 inputDirection = Vector3.zero;
    /// <summary>
    /// Magnitude of the non-normalized inputDirection
    /// </summary>
    private float inputMagnitude = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        infiniteAmmo = true;
    }

    private void Update()
    {
        Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;
        lastMouseCoordinate = Input.mousePosition;

        // Swap between mouse and controller input
        Vector2 controllerInput = new Vector2(Input.GetAxis(RIGHT_STICK_HORIZONTAL), Input.GetAxis(RIGHT_STICK_VERTICAL));

        if (controllerInput != Vector2.zero) 
        {
            usingMouse = false;
        }
        else if (mouseDelta.sqrMagnitude > minMouseMovementMagnitudeSqr) 
        {
            usingMouse = true;
        }
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
        Debug.Log(RIGHT_STICK_HORIZONTAL + ": " + Input.GetAxis(RIGHT_STICK_HORIZONTAL));
        Debug.Log(RIGHT_STICK_VERTICAL + ": " + Input.GetAxis(RIGHT_STICK_VERTICAL));


        if (usingMouse)
        {
            PlaneCheck();
        }
        else 
        {
            Controller();
        }

    }

    private void Controller() 
    {
        // Get direction right stick is pointing,
        /*
        Vector3 desiredDirection = new Vector3(verticalAxis, 0, -horizontalAxis);
        float magnitude = Mathf.Clamp(desiredDirection.magnitude, 0, 1);
        Vector3 desiredDirectionNormalized = Vector3.Normalize(desiredDirection);*/
    }

    private void PlaneCheck() 
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

    public override PlayerWeaponType GetPlayerWeaponType()
    {
        return PlayerWeaponType.INVALID;
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
