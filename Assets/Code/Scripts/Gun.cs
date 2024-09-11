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

    // TODO: Create muzzle flash particlesystem with flashing point light
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



    // Start is called before the first frame update
    void Start()
    {
        GatherMemberReferences();

        HookUpListeners();

        // Turns on crosshair if the gun is a turret
        

        ResetGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (gunStats.IsTurret) 
        {
            turretInputManager.UpdateInputMethod();
        }

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

    public void ResetGameObject() 
    {
        ammoCount = gunStats.MagazineSize;
        stateController.Reset();
    }

    private void GatherMemberReferences() 
    {
        turretInputManager = new TurretInputManager(this.transform, crossHair, gunStats.IsTurret);
        player = FindObjectOfType<PlayerMovement>();

        stateController = new GunState.StateController(gunStats.NumBurstShots);

        InitializeBulletPool();
    }

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

    public void AddAmmo(int amount)
    {
        ammoCount = Mathf.Clamp(ammoCount + amount, 0, gunStats.MagazineSize);
        stateController.HandleTrigger(GunState.StateTrigger.AddAmmo);
    }

    private bool FireThisFrame() 
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

    private void UpdateGun(float deltaTime) 
    {
        
        if (FireThisFrame())
        {
            if (gunStats.IsBurstFire) 
            { stateController.HandleTrigger(GunState.StateTrigger.FireBurstShot); }
            else 
            { stateController.HandleTrigger(GunState.StateTrigger.FireSingleShot); }
        }


        nextTimeToFire = Mathf.Clamp(nextTimeToFire - deltaTime, 0, float.MaxValue);

        if (nextTimeToFire == 0) 
        {
            stateController.HandleTrigger(GunState.StateTrigger.TimeToFireComplete);
        }

        nextTimeToFireBurst = Mathf.Clamp(nextTimeToFireBurst - deltaTime, 0, float.MaxValue);

        // Trigger another burst shot if currently ciring burst rounds
        if (nextTimeToFireBurst == 0 && stateController.FiringBurstRounds)
        {
            stateController.HandleTrigger(GunState.StateTrigger.FireBurstShot);
        }
    }

    private void FireProjectile()
    {
        Bullet bullet = bulletPool.SpawnFromPool();
        Vector3 shotDir = BulletSpawn.transform.forward;

        bullet.Shoot(BulletSpawn.transform.position, shotDir, player.Velocity);
    }

    private void FireHitScan()
    {
        RaycastHit hit;
        if (Physics.Raycast(BulletSpawn.transform.position, BulletSpawn.transform.forward, out hit, gunStats.Range)) 
        {
            Debug.Log(hit.transform.name);

            if ((hit.transform.tag == "Enemy" && gunStats.IsPlayerBullet) ||
                (hit.transform.tag == "Player" && !gunStats.IsPlayerBullet))
            {
                DealDamage(hit.transform.gameObject);
            }

        }
        // TODO: Play muzzleflash particlesystem
    }

    private void ReduceAmmo() 
    {
        ammoCount = gunStats.InfiniteAmmo ? ammoCount : ammoCount - 1;
        if (ammoCount == 0) 
        {
            stateController.HandleTrigger(GunState.StateTrigger.OutOfAmmo);
        }
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

    private void FireSingleReduceAmmo() 
    {
        switch (gunStats.BulletType)
        {
            case EditorObject.BulletType.Projectile:
                FireProjectile();
                break;
            case EditorObject.BulletType.HitScan:
                FireHitScan();
                break;
        }
        ReduceAmmo();
    }
}
