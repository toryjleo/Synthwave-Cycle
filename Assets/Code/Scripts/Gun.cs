using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{
    #region Used by All Guns
    /// <summary>
    /// Data object that defines this gun's stats
    /// </summary>
    [SerializeField] private EditorObject.GunStats gunStats = null;

    /// <summary>
    /// Manages the state of the gun
    /// </summary>
    GunState.StateController stateController = null;

    /// <summary>
    /// Location for the gun to spawn bullets
    /// </summary>
    [SerializeField] private GameObject BulletSpawn = null;

    private PlayerMovement player = null;

    private float nextTimeToFire = 0.0f;
    private int ammoCount = 0;

    #endregion

    [SerializeField] private ParticleSystem muzzleFlash = null;
    [SerializeField] private GameObject impactEffect = null;

    // TODO: Create impact particlesystem with flashing point light
    #region Bullet Instancing
    protected BulletPool bulletPool;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int bulletPoolSize = 200;
    #endregion

    #region Turret Members

    private TurretInputManager turretInputManager = null;

    /// <summary>
    /// Child gameObject with a SpriteRenderer of a crosshair
    /// </summary>
    [SerializeField] private GameObject crossHair;

    #endregion

    private float nextTimeToFireBurst = 0.0f;

    /// <summary>
    /// Returns true if this gun is firing this frame
    /// </summary>
    /// <returns>True if this gun is firing this frame</returns>
    private bool FireThisFrame
    {
        get
        {
            if (gunStats.IsAutomatic)
            {
                return Input.GetButton("Fire1") && stateController.CanShoot;
            }
            else
            {
                return Input.GetButtonDown("Fire1") && stateController.CanShoot;
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        GatherMemberReferences();

        HookUpListeners();

        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        turretInputManager.UpdateInputMethod();

        UpdateGun(Time.deltaTime);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            AddAmmo(1);
            Debug.Log("Adding 1 bullet");
        }
#endif
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gunStats.IsTurret && GameStateController.CanRunGameplay)
        {
            turretInputManager.FixedUpdate();
        }
    }

    /// <summary>
    /// Sets the state to the initial state at the beginning of a level
    /// </summary>
    public void Reset() 
    {
        ammoCount = gunStats.AmmoCount;
        stateController.Reset();
    }

    /// <summary>
    /// Finds all references this object needs
    /// </summary>
    private void GatherMemberReferences() 
    {
        turretInputManager = new TurretInputManager(this.transform, crossHair, gunStats.IsTurret);
        player = FindObjectOfType<PlayerMovement>();

        stateController = new GunState.StateController(gunStats.NumBurstShots);

        InitializeBulletPool();
    }

    /// <summary>
    /// Initializes the bullet pool
    /// </summary>
    protected void InitializeBulletPool() 
    {
        bulletPool = gameObject.GetComponent<BulletPool>();
        if (bulletPool == null)
        {
            bulletPool = gameObject.AddComponent<BulletPool>();
        }
        bulletPool.Init(gunStats, bulletPrefab, bulletPoolSize);
    }

    /// <summary>
    /// Hooks up listeners for gun state events
    /// </summary>
    private void HookUpListeners()
    {
        stateController.fireSingleShot.notifyListenersEnter += HandleFireSingleShotEnter;
        stateController.fireBurstShot.notifyListenersEnter += HandleFireBurstShotEnter;
        stateController.betweenShots.notifyListenersEnter += HandleBetweenShotsEnter;
    }

    /// <summary>
    /// Adds ammo to the ammo count
    /// </summary>
    /// <param name="amount">Amount of ammo to add</param>
    public void AddAmmo(int amount)
    {
        ammoCount = Mathf.Clamp(ammoCount + amount, 0, gunStats.AmmoCount);
        stateController.HandleTrigger(GunState.StateTrigger.AddAmmo);
    }

    /// <summary>
    /// Updates gun logic each frame
    /// </summary>
    /// <param name="deltaTime">Amount of time since last frame update</param>
    private void UpdateGun(float deltaTime) 
    {
        if (FireThisFrame)
        {
            if (gunStats.IsBurstFire) 
            { stateController.HandleTrigger(GunState.StateTrigger.FireBurstShot); }
            else 
            { stateController.HandleTrigger(GunState.StateTrigger.FireSingleShot); }
        }

        UpdateNextTimeToFire(deltaTime);

        UpdateNextTimeToBurstFire(deltaTime);
    }

    /// <summary>
    /// Updates the nextTimeToFire variable. Will trigger TimeToFireComplete when nextTimeToFire ticks down.
    /// </summary>
    /// <param name="deltaTime">Amount of time since last frame update</param>
    private void UpdateNextTimeToFire(float deltaTime) 
    {
        nextTimeToFire = Mathf.Clamp(nextTimeToFire - deltaTime, 0, float.MaxValue);

        if (nextTimeToFire == 0)
        {
            stateController.HandleTrigger(GunState.StateTrigger.TimeToFireComplete);
        }
    }

    /// <summary>
    /// Updates the nextTimeToFireBurst variable. Will trigger FireBurstShot when nextTimeToFireBurst ticks down.
    /// </summary>
    /// <param name="deltaTime">Amount of time since last frame update</param>
    private void UpdateNextTimeToBurstFire(float deltaTime)
    {
        nextTimeToFireBurst = Mathf.Clamp(nextTimeToFireBurst - deltaTime, 0, float.MaxValue);

        // Trigger another burst shot if currently ciring burst rounds
        if (nextTimeToFireBurst == 0 && stateController.FiringBurstRounds)
        {
            stateController.HandleTrigger(GunState.StateTrigger.FireBurstShot);
        }
    }

    /// <summary>
    /// Fires a single projectile from the bulletPool
    /// </summary>
    private void FireProjectile(Vector3 direction)
    {
        Bullet bullet = bulletPool.SpawnFromPool();

        bullet.Shoot(BulletSpawn.transform.position, direction, player.Velocity);
    }

    /// <summary>
    /// Fires a single ray
    /// </summary>
    private void FireHitScan(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(BulletSpawn.transform.position, direction, out hit, gunStats.Range)) 
        {
            Debug.Log(hit.transform.name);

            if ((hit.transform.tag == "Enemy" && gunStats.IsPlayerGun) ||
                (hit.transform.tag == "Player" && !gunStats.IsPlayerGun))
            {
                DealDamage(hit.transform.gameObject);
            }

            // TODO: create a objectpool for the impactEffect
            GameObject g = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            g.GetComponent<ParticleSystem>().Play();
            Destroy(g, 2f);

        }
        else 
        {
            // TODO: create a objectpool for the impactEffect
            GameObject g = Instantiate(impactEffect, transform.forward * 12, Quaternion.LookRotation(Vector3.up));
            g.GetComponent<ParticleSystem>().Play();
            Destroy(g, 2f);

        }


    }

    /// <summary>
    /// Reduces ammo by 1. Will trigger OutOfAmmo when ammoCount hits zero
    /// </summary>
    private void ReduceAmmo() 
    {
        ammoCount = gunStats.InfiniteAmmo ? ammoCount : ammoCount - 1;
        if (ammoCount == 0) 
        {
            stateController.HandleTrigger(GunState.StateTrigger.OutOfAmmo);
        }
    }

    /// <summary>
    /// Inflict gun damage on other
    /// </summary>
    /// <param name="other">Object with Health component</param>
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

    #region Event Handlers
    public void HandleFireSingleShotEnter()
    {
        FireSingleReduceAmmo();
        stateController.HandleTrigger(GunState.StateTrigger.EnterTimeBetween);
    }

    public void HandleFireBurstShotEnter()
    {
        FireSingleReduceAmmo();
        nextTimeToFireBurst = gunStats.TimeBetweenBurstShots;
    }

    public void HandleBetweenShotsEnter() 
    {
        nextTimeToFire = gunStats.TimeBetweenShots;
    }
    #endregion

    /// <summary>
    /// Fires either a projectile or hitscan and reduces ammo
    /// </summary>
    private void FireSingleReduceAmmo() 
    {

        Vector3 direction = BulletSpawn.transform.forward;
        // TODO: Update to fire multiple times
        FireInDirection(direction);
        muzzleFlash.Play();
        ReduceAmmo();
    }

    private void FireInDirection(Vector3 direction) 
    {
        switch (gunStats.BulletType)
        {
            case EditorObject.BulletType.Projectile:
                FireProjectile(direction);
                break;
            case EditorObject.BulletType.HitScan:
                FireHitScan(direction);
                break;
        }
    }
}
