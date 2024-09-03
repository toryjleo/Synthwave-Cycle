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


    // Start is called before the first frame update
    void Start()
    {
        
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

    private void FireProjectile()
    {
        Debug.Log("Firing Projectile");
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
