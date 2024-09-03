using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{
    [SerializeField] EditorObject.GunStats gunStats;

    public float range = 100f;
    public float fireRate = 15f;

    private float nextTimeToFire = 0.0f;


    // TODO: Create muzzle flash particlesystem with flashing point light
    // TODO: Create impact particlesystem with flashing point light

    protected BulletPool bulletPool;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int bulletPoolSize = 200;

    private PlayerMovement player;


    // Start is called before the first frame update
    void Start()
    {
        InitializeBulletPool();
        player = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1f / fireRate);
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

    public void Init(EditorObject.GunStats gunStats)
    {
        this.gunStats = gunStats;
    }

    protected void InitializeBulletPool() 
    {
        bulletPool = gameObject.GetComponent<BulletPool>();
        if (bulletPool == null)
        {
            bulletPool = gameObject.AddComponent<BulletPool>();
        }
        bulletPool.Init(bulletPrefab, bulletPoolSize);
    }

    private void FireProjectile()
    {
        Debug.Log("Firing Projectile");
        Bullet bullet = bulletPool.SpawnFromPool();
        Vector3 shotDir = transform.forward;

        bullet.Shoot(transform.position, shotDir, player.Velocity);
    }

    private void FireHitScan()
    {
        Debug.Log("Firing Hitscan");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range)) 
        {
            Debug.Log(hit.transform.name);
            // TODO: Check if it is player or enemy
            // TODO: instantiate and play impact effect particlesystem at hit.point and rotate to normal
            // https://www.youtube.com/watch?v=THnivyG0Mvo

        }
        // TODO: Play muzzleflash particlesystem
    }
}
