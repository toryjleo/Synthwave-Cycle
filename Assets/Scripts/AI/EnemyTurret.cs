using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Script Controlls the turret attached to the turret bike. It requires a GameObject for the muzzle and keeps track of the mouse coordinates. 
/// </summary>
public class EnemyTurret : Gun
{
    public GameObject muzzle1;

    public Vector3 mouse;


    public override void Init()
    {
        lastFired = 0;
        fireRate = 10;
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
    }

    public override void Shoot(Vector3 initialVelocity)
    {
        if (CanShootAgain())
        {
            lastFired = Time.time;
            Bullet bullet = bulletPool.SpawnFromPool();

            Vector3 shotDir = muzzle1.transform.right;
            bullet.Shoot(muzzle1.transform.position, shotDir, initialVelocity);


            // Gun specific

            OnBulletShot(shotDir * bullet.Mass * bullet.MuzzleVelocity);
            ApplyRecoil(shotDir, bullet);
        }


    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Logic for Having turret track the Mouse. 
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out RaycastHit raycastHit))
        {
            mouse = raycastHit.point;

            //Debug.Log(mouse +"   "+this.transform.position);

            mouse -= this.transform.position;
        }
        //mouse = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(mouse.x, mouse.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        //transform.Rotate(Vector3.up, steerRate);

    }
}
