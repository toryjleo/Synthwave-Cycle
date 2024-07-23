using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Script Controlls the turret attached to the turret bike. It requires a GameObject for the muzzle and keeps track of the mouse coordinates. 
/// </summary>
public class Turret : Gun
{
    [SerializeField] private bool isMainScene = true;
    [SerializeField] private float distanceToBulletSpawn = .2f;

    // Start is called before the first frame update
    void Start()
    {
        infiniteAmmo = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateController.CanRunGameplay)
        {


            Vector3 mouse = Vector3.zero;
            // Logic for Having turret track the Mouse. 
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit raycastHit))
            {
                mouse = raycastHit.point;

                mouse -= this.transform.position;
            }

            var angle = Mathf.Atan2(mouse.x, mouse.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
    }

    public override void Init()
    {
        base.Init();
        lastFired = 0;
        fireRate = 10;
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
