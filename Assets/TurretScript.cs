using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : Gun
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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         mouse = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
         var angle = Mathf.Atan2(mouse.x, mouse.y) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        //transform.Rotate(Vector3.up, steerRate);

    }
}
