using UnityEngine;
using Unity.Mathematics;
using Generic;

namespace Gun
{
    /// <summary>
    /// Event to be called when ammo changes
    /// </summary>
    public delegate void NotifyAmmo();

    /// <summary>
    /// Event to be called when a bullet hits
    /// </summary>
    /// <param name="position"></param>
    public delegate void BulletHitHandler(Vector3 position);

    /// <summary>
    /// Class that implements all gun behavior
    /// </summary>
    public class Gun : MonoBehaviour
    {
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

        /// <summary>
        /// Reference to player
        /// </summary>
        private PlayerMovement player = null;

        /// <summary>
        /// Muzzle flash effect
        /// </summary>
        [SerializeField] private ParticleSystem muzzleFlash = null;


        #region Object Instancing

        private const int INFINITE_AMMO_COUNT = 200;

        // Projectile
        protected ProjectileObjectPool projectilePool = null;
        [SerializeField] private BulletProjectile bulletPrefab = null;

        // Area of Effect
        protected ProjectileObjectPool areaOfEffectPool = null;
        [SerializeField] private AreaOfEffect areaOfEffectPrefab = null;

        // HitScan
        protected HitScan hitScan = null;

        // Explosions
        protected Generic.ObjectPool explosionPool = null;
        [SerializeField] private Explosion explosionPrefab = null;

        // Impact Effect
        protected Generic.ObjectPool impactEffectPool = null;
        [SerializeField] private PooledParticle impactEffectPrefab = null;
        #endregion

        #region Turret Members

        private TurretInputManager turretInputManager = null;

        /// <summary>
        /// Child gameObject with a SpriteRenderer of a crosshair
        /// </summary>
        [SerializeField] private GameObject crossHair;

        #endregion


        /// <summary>
        /// Number of seconds until fire can be called again
        /// </summary>
        private float nextTimeToFire = 0.0f;
        /// <summary>
        /// Number of times this gun can fire
        /// </summary>
        private int ammoCount = 0;
        /// <summary>
        /// Amount of time until the next burst shot triggers
        /// </summary>
        private float nextTimeToFireBurst = 0.0f;
        /// <summary>
        /// How close the gun is to overheating. Overheats at 100%
        /// </summary>
        private float overHeatPercent = 0.0f;
        /// <summary>
        /// Random object
        /// </summary>
        private Unity.Mathematics.Random rand;
        /// <summary>
        /// Used by GunTester to automatically fire this gun
        /// </summary>
        public bool externalFire = false;


        /// <summary>
        /// Returns true if this gun is firing this frame
        /// </summary>
        /// <returns>True if this gun is firing this frame</returns>
        private bool FireThisFrame
        {
            get
            {
                if (GameStateController.CanRunGameplay)
                {
                    if (gunStats.IsAutomatic)
                    {
                        return (Input.GetButton("Fire1") || externalFire) && stateController.CanShoot;
                    }
                    else
                    {
                        return (Input.GetButtonDown("Fire1") || externalFire) && stateController.CanShoot;
                    }
                }
                else 
                {
                    return false;
                }
            }
        }

        #region Ammo Props for UI
        //Event for the UI to update ammo counter
        public NotifyAmmo onAmmoChange;
        public GunState.StateController GunStateController
        {
            get { return stateController; }
        }
        public bool IsInfiniteAmmo
        {
            get { return gunStats.InfiniteAmmo; }
        }

        public int MaxAmmo
        {
            get { return gunStats.AmmoCount; }
        }

        public int AmmoCount
        {
            get { return ammoCount; }
        }
        #endregion

        #region Overheat Props for UI
        public bool IsOverheat
        {
            get { return gunStats.CanOverheat; }
        }

        public float OverheatPercent
        {
            get { return overHeatPercent; }
        }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            GatherMemberReferences();

            HookUpListeners();

            Reset();
            rand.InitState();
        }

        // Update is called once per frame
        void Update()
        {
            turretInputManager.UpdateInputMethod();

            UpdateGun(Time.deltaTime);
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
            nextTimeToFireBurst = 0.0f;
            overHeatPercent = 0.0f;
            stateController.Reset();

            projectilePool.ResetGameObject();
            explosionPool.ResetGameObject();
            impactEffectPool.ResetGameObject();
        }

        /// <summary>
        /// Finds all references this object needs
        /// </summary>
        private void GatherMemberReferences()
        {
            turretInputManager = new TurretInputManager(this.transform, crossHair, gunStats.IsTurret);
            player = FindObjectOfType<PlayerMovement>();

            stateController = new GunState.StateController(gunStats.NumBurstShots, gunStats.PrintDebugState);

            if (projectilePool == null)
            {
                int instantiateCount = gunStats.InfiniteAmmo ? INFINITE_AMMO_COUNT * gunStats.ProjectileCountPerShot :
                                                               gunStats.AmmoCount * gunStats.ProjectileCountPerShot;
                projectilePool = new ProjectileObjectPool(gunStats, bulletPrefab, HandleBulletHit);
                projectilePool.PoolObjects(instantiateCount);
            }
            if (areaOfEffectPool == null) 
            {
                int instantiateCount = gunStats.InfiniteAmmo ? INFINITE_AMMO_COUNT * gunStats.ProjectileCountPerShot :
                                                   gunStats.AmmoCount * gunStats.ProjectileCountPerShot;
                areaOfEffectPool = new ProjectileObjectPool(gunStats, areaOfEffectPrefab, HandleBulletHit);
                areaOfEffectPool.PoolObjects(instantiateCount);
            }
            if (hitScan == null) 
            {
                hitScan = new HitScan(gunStats);
            }
            if (explosionPool == null) 
            {
                int numberOfHits = gunStats.BulletPenetration + 1; // Needs to be 1 more than penetration to get bullet hit number
                int instantiateCount = gunStats.InfiniteAmmo ? 
                                       INFINITE_AMMO_COUNT * gunStats.ProjectileCountPerShot * numberOfHits :
                                       gunStats.AmmoCount * gunStats.ProjectileCountPerShot * numberOfHits;
                explosionPool = new Generic.ObjectPool(gunStats, explosionPrefab);
                explosionPool.PoolObjects(instantiateCount);
            }
            if (impactEffectPool == null)
            {
                int instantiateCount = gunStats.InfiniteAmmo ? INFINITE_AMMO_COUNT * gunStats.ProjectileCountPerShot :
                                                               gunStats.AmmoCount * gunStats.ProjectileCountPerShot;
                impactEffectPool = new Generic.ObjectPool(gunStats, impactEffectPrefab);
                impactEffectPool.PoolObjects(instantiateCount);
            }
        }

        /// <summary>
        /// Hooks up listeners for gun state events
        /// </summary>
        private void HookUpListeners()
        {
            stateController.fireSingleShot.notifyListenersEnter += HandleFireSingleShotEnter;
            stateController.fireBurstShot.notifyListenersEnter += HandleFireBurstShotEnter;
            stateController.betweenShots.notifyListenersEnter += HandleBetweenShotsEnter;

            hitScan.notifyListenersHit += HandleBulletHit;

        }

        /// <summary>
        /// Applies effects when a bullet hits something
        /// </summary>
        /// <param name="hitPoint">The location where the bullet hit</param>
        public void HandleBulletHit(Vector3 hitPoint) 
        {
            if (gunStats.IsExplosive)
            {
                if (!explosionPrefab)
                {
                    Debug.LogError("Need explosion prefab reference");
                }
                else
                {
                    Explosion explosion = explosionPool.SpawnFromPool() as Explosion;
                    if (!explosion)
                    {
                        Debug.Log("Explosion pool does not contain explosions");
                    }
                    else
                    {
                        explosion.transform.localPosition = hitPoint;
                        explosion.TriggerExplosiveAbility();
                    }
                }
            }
            
        }

        /// <summary>
        /// Adds ammo to the ammo count
        /// </summary>
        /// <param name="amount">Amount of ammo to add</param>
        public void AddAmmo(int amount)
        {
            ammoCount = Mathf.Clamp(ammoCount + amount, 0, gunStats.AmmoCount);
            stateController.HandleTrigger(GunState.StateTrigger.AddAmmo);
            onAmmoChange.Invoke();
        }

        #region Frame Update
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

                externalFire = false;
            }

            UpdateNextTimeToFire(deltaTime);

            UpdateNextTimeToBurstFire(deltaTime);

            UpdateOverHeat(deltaTime);
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
        /// Cools down the overheat function and triggers OverHeatComplete if the gun is overheated.
        /// </summary>
        /// <param name="deltaTime">Amount of time since last frame update</param>
        private void UpdateOverHeat(float deltaTime)
        {
            overHeatPercent = Mathf.Clamp(overHeatPercent - (deltaTime * gunStats.CoolDownPerSecond), 0, 100);

            if (overHeatPercent < gunStats.CoolDownBarrier && stateController.IsOverHeated)
            {
                stateController.HandleTrigger(GunState.StateTrigger.OverHeatComplete);
            }
        }
        #endregion

        #region Gun Firing Methods
        /// <summary>
        /// Fires a single projectile from the bulletPool
        /// </summary>
        /// <param name="direction">Direction to move in</param>
        private void FireProjectile(Vector3 direction)
        {
            Projectile bullet = projectilePool.SpawnFromPool() as Projectile;

            bullet.Shoot(BulletSpawn.transform.position, direction, player ? player.Velocity : Vector3.zero);
        }

        /// <summary>
        /// Fires a single ray
        /// </summary>
        /// <param name="direction">Direction to move in</param>
        private void FireHitScan(Vector3 direction)
        {
            hitScan.Shoot(BulletSpawn.transform.position, direction, impactEffectPool);
        }

        /// <summary>
        /// Fires a single area of effect
        /// </summary>
        /// <param name="direction">Direction to move in</param>
        private void FireAreaOfEffect(Vector3 direction)
        {
            AreaOfEffect bullet = areaOfEffectPool.SpawnFromPool() as AreaOfEffect;

            bullet.Shoot(BulletSpawn.transform.position, direction, player ? player.Velocity : Vector3.zero);
        }

        /// <summary>
        /// Reduces ammo by 1. Will trigger OutOfAmmo when ammoCount hits zero
        /// </summary>
        private void ReduceAmmo()
        {
            ammoCount = gunStats.InfiniteAmmo ? ammoCount : ammoCount - 1;
            if (ammoCount == 0)
            {
                // This must be called before OverHeated trigger
                stateController.HandleTrigger(GunState.StateTrigger.OutOfAmmo);
            }
            onAmmoChange?.Invoke();
        }


        /// <summary>
        /// Fires single or multiple projectiles or hitscans and reduces ammo
        /// </summary>
        private void FireOnce()
        {
            FireAllBulletsThisShot();
            muzzleFlash.Play();
            ReduceAmmo();

            if (gunStats.CanOverheat)
            {
                overHeatPercent = Mathf.Clamp(overHeatPercent + gunStats.OverHeatPercentPerShot, 0.0f, 100.0f);
                if (overHeatPercent == 100)
                {
                    stateController.HandleTrigger(GunState.StateTrigger.OverHeated);
                }
            }
        }

        /// <summary>
        /// Fires all the bullets needed for this shot
        /// </summary>
        private void FireAllBulletsThisShot()
        {
            Vector3 initialForward = BulletSpawn.transform.forward;

            // Rotate the BulletSpawn to the initial firing position
            float radius = gunStats.DistanceBetweenProjectiles * (gunStats.ProjectileCountPerShot - 1);
            float angleStart = radius / 2;
            Quaternion rotationToApply = Quaternion.AngleAxis(-angleStart, Vector3.up);
            BulletSpawn.transform.rotation = BulletSpawn.transform.rotation * rotationToApply;

            Quaternion rotationPerIteration = Quaternion.AngleAxis(gunStats.DistanceBetweenProjectiles, Vector3.up);

            for (int i = 0; i < gunStats.ProjectileCountPerShot; i++)
            {
                float randDegreeRot = rand.NextFloat(-gunStats.ProjectileSpread, gunStats.ProjectileSpread);
                Quaternion randomRotation = Quaternion.AngleAxis(randDegreeRot, Vector3.up);

                if (gunStats.ProjectileCountPerShot == 1)
                {
                    // Case for if we are only shooting once and want to apply a random rotation
                    BulletSpawn.transform.rotation = BulletSpawn.transform.rotation * randomRotation;
                }

                Vector3 direction = BulletSpawn.transform.forward;
                FireSingleInDirection(direction);
                BulletSpawn.transform.rotation = BulletSpawn.transform.rotation * rotationPerIteration * randomRotation;
            }
            // Return BulletSpawn to its initial position
            BulletSpawn.transform.forward = initialForward;
        }

        /// <summary>
        /// Shoots either a single hitscan or projectile in specified direction
        /// </summary>
        /// <param name="direction">Direction of this shot</param>
        private void FireSingleInDirection(Vector3 direction)
        {
            switch (gunStats.BulletType)
            {
                case EditorObject.BulletType.BulletProjectile:
                    FireProjectile(direction);
                    break;
                case EditorObject.BulletType.HitScan:
                    FireHitScan(direction);
                    break;
                case EditorObject.BulletType.AreaOfEffect:
                    FireAreaOfEffect(direction);
                    break;
            }
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Listens to fireSingleShot.notifyListenersEnter
        /// </summary>
        public void HandleFireSingleShotEnter()
        {
            FireOnce();
            stateController.HandleTrigger(GunState.StateTrigger.EnterTimeBetween);
        }

        /// <summary>
        /// Listens to fireBurstShot.notifyListenersEnter
        /// </summary>
        public void HandleFireBurstShotEnter()
        {
            FireOnce();
            nextTimeToFireBurst = gunStats.TimeBetweenBurstShots;
        }

        /// <summary>
        /// Listens to betweenShots.notifyListenersEnter
        /// </summary>
        public void HandleBetweenShotsEnter()
        {
            nextTimeToFire = gunStats.TimeBetweenShots;
        }
        #endregion

    }
}