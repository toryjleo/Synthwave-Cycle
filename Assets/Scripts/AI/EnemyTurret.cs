using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Script Controlls the turret attached to the turret bike. It requires a GameObject for the muzzle and keeps track of the mouse coordinates. 
/// </summary>
public class EnemyTurret : Gun
{
    public GameObject muzzle1;

    public GameObject target;


    public override void Init()
    {
        lastFired = 0;
        fireRate = 10;
        bulletPool = gameObject.AddComponent<BulletPool>();
        bulletPool.Init(bulletPrefab);
        target = GameObject.FindGameObjectWithTag("Player");
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
    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        // Logic for Having turret track the Mouse. 

        //mouse = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        //var angle = Mathf.Atan2(target.transform.position.x, target.transform.position.z) * Mathf.Rad2Deg;

        transform.LookAt(target.transform.position, Vector3.up);
        

    }
}
