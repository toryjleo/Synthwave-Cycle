using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Script Controlls the turret attached to the turret bike. It requires a GameObject for the muzzle and keeps track of the mouse coordinates. 
/// </summary>
public class Turret : Gun
{
    private float distanceToBulletSpawn = .9f;
    [SerializeField] private GameObject crossHair;

    bool usingMouse = true;

    Plane plane = new Plane(Vector3.up, 0);

    // Start is called before the first frame update
    void Start()
    {
        infiniteAmmo = true;
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
        if (usingMouse)
        {
            PlaneCheck();
        }
        else 
        {
            // TODO: Implement controller input
        }

    }

    private void PlaneCheck() 
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);
            Debug.Log(mouseWorldPos);
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
