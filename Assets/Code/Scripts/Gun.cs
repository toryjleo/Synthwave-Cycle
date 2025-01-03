using UnityEngine;
using EditorObject;

namespace Gun
{

    /// <summary>
    /// Event to be called when ammo changes
    /// </summary>
    /// <param name="gun">Reference to the gun calling this event</param>
    public delegate void NotifyAmmo(Gun gun);

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
        /// Wrapper Class that manages the current accuracy
        /// </summary>
        private class AccuracyManager
        {
            private GunStats stats = null;
            /// <summary>
            /// Adjustable Accuracy
            /// </summary>
            private float m_currentAccuracy = 0;

            /// <summary>
            /// Firing radius
            /// </summary>
            public float Radius
            {
                get => m_currentAccuracy * stats.AngleBetweenProjectiles * (stats.ProjectilesReleasedPerShot - 1);
            }

            /// <summary>
            /// Random rotation within the projectile spread
            /// </summary>
            public float RandomDegreeRotation
            { 
                get => m_currentAccuracy * Random.Range(-stats.RandomSpreadPerProjectile, stats.RandomSpreadPerProjectile);
            }

            /// <summary>
            /// Distance between each projectile
            /// </summary>
            public float DistanceBetweenProjectiles
            {
                get => stats.AngleBetweenProjectiles * m_currentAccuracy;
            }

            /// <summary>
            /// True if the stats.MaxAccuracyChange is less than 100 percent
            /// </summary>
            private bool MaxLessThanOneHundredPercent 
            {
                get => stats.AccuracyArrivalOverTime < 1.0f;
            }

            public AccuracyManager(GunStats gunStats)
            {
                stats = gunStats;
                Reset();
            }

            /// <summary>
            /// Updates the m_currentAccuracy
            /// </summary>
            /// <param name="deltaTime">time since last frame</param>
            /// <param name="isShooting">true if the player is shooting</param>
            public void Update(float deltaTime, bool isShooting) 
            {

                if (isShooting) 
                {
                    if (MaxLessThanOneHundredPercent)
                    {
                        m_currentAccuracy -= (stats.DeltaWindUpSpreadPerSecond * deltaTime);
                    }
                    else
                    {
                        m_currentAccuracy += (stats.DeltaWindUpSpreadPerSecond * deltaTime);
                    }
                }
                else 
                {
                    if (MaxLessThanOneHundredPercent)
                    {
                        m_currentAccuracy += (stats.DeltaWindDownSpreadPerSecond * deltaTime);
                    }
                    else
                    {
                        m_currentAccuracy -= (stats.DeltaWindDownSpreadPerSecond * deltaTime);
                    }
                }

                float lowerBounds = 0;
                float upperBounds = 0;
                if (MaxLessThanOneHundredPercent)
                {
                    lowerBounds = stats.AccuracyArrivalOverTime;
                    upperBounds = 1.0f;
                }
                else
                {
                    lowerBounds = 1.0f;
                    upperBounds = stats.AccuracyArrivalOverTime;
                }

                m_currentAccuracy = Mathf.Clamp(m_currentAccuracy, lowerBounds, upperBounds);
            }

            public void Reset() 
            {
                m_currentAccuracy = 1.0f;
            }
        }

        /// <summary>
        /// Wrapper Class that manages the curren time between shots
        /// </summary>
        private class TimeBetweenShotsManager 
        {
            private GunStats stats = null;
            /// <summary>
            /// Adjustable TimeBetweenShots
            /// </summary>
            private float m_currentPercentTimeBetweenShots = 0;
            
            /// <summary>
            /// Time between each burst shot
            /// </summary>
            public float TimeBetweenBurstShots
            {
                get => stats.TimeBetweenBurstShots * m_currentPercentTimeBetweenShots;
            }

            /// <summary>
            /// Time between each consecutive shot
            /// </summary>
            public float TimeBetweenShots
            {
                get => stats.TimeBetweenShots * m_currentPercentTimeBetweenShots;
            }


            /// <summary>
            /// True if the stats.MaxPercentTimeBetweenShots is less than 100 percent
            /// </summary>
            private bool MaxLessThanOneHundredPercent 
            {
                get => stats.TimeBetweenShotsScalingOverTime < 1.0f;
            }
                        

            public TimeBetweenShotsManager(GunStats gunStats)
            {
                stats = gunStats;
                Reset();
            }

            /// <summary>
            /// Updates the m_currentPercentTimeBetweenShots
            /// </summary>
            /// <param name="deltaTime">time since last frame</param>
            /// <param name="isShooting">true if the player is shooting</param>
            public void Update(float deltaTime, bool shootThisFrame)
            {
                if (shootThisFrame)
                {
                    if (MaxLessThanOneHundredPercent)
                    {
                        m_currentPercentTimeBetweenShots -= (stats.DeltaWindupPercentTimeBetweenShots * deltaTime);
                    }
                    else
                    {
                        m_currentPercentTimeBetweenShots += (stats.DeltaWindupPercentTimeBetweenShots * deltaTime);
                    }
                }
                else
                {
                    if (MaxLessThanOneHundredPercent)
                    {
                        m_currentPercentTimeBetweenShots += (stats.DeltaWindDownPercentTimeBetweenShots * deltaTime);
                    }
                    else
                    {
                        m_currentPercentTimeBetweenShots -= (stats.DeltaWindDownPercentTimeBetweenShots * deltaTime);
                    }
                }

                float lowerBounds = 0;
                float upperBounds = 0;
                if (MaxLessThanOneHundredPercent)
                {
                    lowerBounds = stats.TimeBetweenShotsScalingOverTime;
                    upperBounds = 1.0f;
                }
                else
                {
                    lowerBounds = 1.0f;
                    upperBounds = stats.TimeBetweenShotsScalingOverTime;
                }

                m_currentPercentTimeBetweenShots = Mathf.Clamp(m_currentPercentTimeBetweenShots, lowerBounds, upperBounds);
            }

            public void Reset()
            {
                m_currentPercentTimeBetweenShots = 1;
            }
        }

        private AccuracyManager m_accuracy = null;
        private TimeBetweenShotsManager m_timeBetweenShots = null;
        /// <summary>
        /// Data object that defines this gun's stats
        /// </summary>
        [SerializeField] private EditorObject.GunStats gunStats = null;

        /// <summary>
        /// Manages the state of the gun
        /// </summary>
        GunState.StateController stateController = null;

        private ImpactManager impactManager = null;

        /// <summary>
        /// Location for the gun to spawn bullets
        /// </summary>
        [SerializeField] private GameObject BulletSpawn = null;

        [SerializeField] private Renderer barrel;

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
        [SerializeField] protected PooledHitScanBulletTrail hitScanBulletTrailPrefab = null;

        // Explosions
        protected Generic.ObjectPool explosionPool = null;
        [SerializeField] private Explosion explosionPrefab = null;
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
        private bool externalFire = false;

        [SerializeField] private AmmoCount ammoCount = null;

        /// <summary>
        /// Triggered when this gun runs out of ammo
        /// </summary>
        public NotifyAmmo onOutOfAmmo;

        #region Properties

        public bool ExternalFire 
        {
            get { return externalFire; } set {  externalFire = value; }
        }

        private bool IsPlayerGun 
        {
            get => gunStats.IsPlayerGun;
        }

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
                    return externalFire && stateController.CanShoot;
                }
                else 
                {
                    return false;
                }
            }
        }

        public bool IsAutomatic
        {
            get => gunStats.IsAutomatic;
        }

        public bool AtMaxAmmo 
        {
            get => ammoCount.AtMaxAmmo;
        }

        #endregion

        #region Ammo Props for UI
        //Event for the UI to update ammo counter
        public NotifyAmmo onAmmoChange;
        public GunState.StateController GunStateController
        {
            get { return stateController; }
        }

        public int AmmoCount
        {
            get { return ammoCount == null ? 0 : ammoCount.Count; }
        }

        public bool IsInfiniteAmmo 
        {
            get => ammoCount.IsInfiniteAmmo;
        }

        public int MaxAmmo 
        {
            get => ammoCount.MaxAmmo;
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

        #region Initialization

        private void TestSceneInit() 
        {
            if (stateController == null)
            {
                Init();
            }
        }

        public void Init(EditorObject.GunStats stats) 
        {
            if (stats == null) 
            {
                Debug.LogError("Gun was passed a null gunstats");
            }
            else 
            {
                this.gunStats = stats;
                Init();
            }
        }

        private void Init() 
        {
            GatherMemberReferences();

            HookUpListeners();

            Reset();
            rand.InitState();
        }

        /// <summary>
        /// Sets the state to the initial state at the beginning of a level
        /// </summary>
        public void Reset()
        {
            ammoCount = new AmmoCount(gunStats);
            nextTimeToFireBurst = 0.0f;
            overHeatPercent = 0.0f;

            m_accuracy.Reset();
            m_timeBetweenShots.Reset();
            stateController.Reset();
            hitScan.Reset();

            projectilePool.ResetGameObject();
            explosionPool.ResetGameObject();
            areaOfEffectPool.ResetGameObject();
        }



        /// <summary>
        /// Finds all references this object needs
        /// </summary>
        private void GatherMemberReferences()
        {
            if (gunStats  == null) 
            {
                Debug.LogError("Gunstats is null");
            }
            else 
            {
                m_accuracy = new AccuracyManager(gunStats);
                m_timeBetweenShots = new TimeBetweenShotsManager(gunStats);
                if (IsPlayerGun) 
                {
                    turretInputManager = new TurretInputManager(this.transform, crossHair, gunStats.IsTurret);
                }
                player = FindObjectOfType<PlayerMovement>();

                stateController = new GunState.StateController(gunStats.ShotBurstCount, gunStats.PrintDebugState);
                impactManager = FindObjectOfType<ImpactManager>();
                if (impactManager == null)
                {
                    Debug.LogError("There is no impactManager in the scene!");
                }

                if (projectilePool == null)
                {

                    // TODO: Make this prediction number a single method

                    int instantiateCount = gunStats.HasInfiniteAmmo ? INFINITE_AMMO_COUNT * gunStats.ProjectilesReleasedPerShot :
                                                                   gunStats.AmmoCount * gunStats.ProjectilesReleasedPerShot;
                    projectilePool = new ProjectileObjectPool(gunStats, bulletPrefab, HandleBulletHit);
                    projectilePool.PoolObjects(instantiateCount);
                }
                if (areaOfEffectPool == null)
                {
                    int instantiateCount = gunStats.HasInfiniteAmmo ? INFINITE_AMMO_COUNT * gunStats.ProjectilesReleasedPerShot :
                                                       gunStats.AmmoCount * gunStats.ProjectilesReleasedPerShot;
                    areaOfEffectPool = new ProjectileObjectPool(gunStats, areaOfEffectPrefab, HandleBulletHit);
                    areaOfEffectPool.PoolObjects(instantiateCount);
                }
                if (hitScan == null)
                {
                    int instantiateCount = gunStats.HasInfiniteAmmo ? INFINITE_AMMO_COUNT * gunStats.ProjectilesReleasedPerShot :
                                                                  gunStats.AmmoCount * gunStats.ProjectilesReleasedPerShot;
                    hitScan = new HitScan(gunStats, hitScanBulletTrailPrefab, instantiateCount);
                }
                if (explosionPool == null)
                {
                    int numberOfHits = gunStats.BulletPenetration + 1; // Needs to be 1 more than penetration to get bullet hit number
                    int instantiateCount = gunStats.HasInfiniteAmmo ?
                                           INFINITE_AMMO_COUNT * gunStats.ProjectilesReleasedPerShot * numberOfHits :
                                           gunStats.AmmoCount * gunStats.ProjectilesReleasedPerShot * numberOfHits;
                    explosionPool = new Generic.ObjectPool(gunStats, explosionPrefab);
                    explosionPool.PoolObjects(instantiateCount);
                }
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
            stateController.outOfAmmo.notifyListenersEnter += HandleOutOfAmmoEnter;

            hitScan.notifyListenersHit += HandleBulletHit;

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
            nextTimeToFireBurst = m_timeBetweenShots.TimeBetweenBurstShots;
        }

        /// <summary>
        /// Listens to betweenShots.notifyListenersEnter
        /// </summary>
        public void HandleBetweenShotsEnter()
        {
            nextTimeToFire = 0;
        }

        /// <summary>
        /// Listens to stateController.outOfAmmo.notifyListenersEnter
        /// </summary>
        public void HandleOutOfAmmoEnter()
        {
            onOutOfAmmo?.Invoke(this);
        }
        #endregion

        #region Monobehavior

        // Start is called before the first frame update
        void Start()
        {
            TestSceneInit();
        }

        // Update is called once per frame
        void Update()
        {
            if (IsPlayerGun) 
            {
                turretInputManager.UpdateInputMethod();
            }

            UpdateGun(Time.deltaTime);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (gunStats.IsTurret && IsPlayerGun && GameStateController.CanRunGameplay)
            {
                turretInputManager.FixedUpdate();
            }
        }
        #endregion

        #region Utility

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
        /// Verifies if this gun uses the specified GunStats
        /// </summary>
        /// <param name="other">GunStats to check</param>
        /// <returns>True if this gun is using the specified GunStats</returns>
        public bool IsThisGun(GunStats other)
        {
            return other == gunStats;
        }

        /// <summary>
        /// Adds ammo to the ammo count
        /// </summary>
        /// <param name="amount">Amount of ammo to add</param>
        public int AddAmmo(int amount)
        {
            int overflow = ammoCount.AddAmmo(amount);
            stateController.HandleTrigger(GunState.StateTrigger.AddAmmo);
            onAmmoChange?.Invoke(this);
            return overflow;
        }

        /// <summary>
        /// Reduces ammo by 1. Will trigger OutOfAmmo when ammoCount hits zero
        /// </summary>
        private void ReduceAmmo()
        {
            ammoCount.ReduceAmmo();
            if (ammoCount.Count <= 0)
            {
                // This must be called before OverHeated trigger
                stateController.HandleTrigger(GunState.StateTrigger.OutOfAmmo);
            }
            onAmmoChange?.Invoke(this);
        }

        /// <summary>
        /// Sets the ammo to this gun
        /// </summary>
        /// <param name="amount">Value to set the ammo to</param>
        public void SetAmmo(int amount) 
        {
            ammoCount.SetAmmo(amount);
            if (ammoCount.Count > 0) 
            {
                stateController.HandleTrigger(GunState.StateTrigger.AddAmmo);
                onAmmoChange?.Invoke(this);
            }
        }

        /// <summary>
        /// Sets the ammo to this gun
        /// </summary>
        public void SetMaxAmmo()
        {
            ammoCount.SetMaxAmmo();
            stateController.HandleTrigger(GunState.StateTrigger.AddAmmo);
            onAmmoChange?.Invoke(this);
        }

        #endregion

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
            m_accuracy.Update(deltaTime, ExternalFire);
            m_timeBetweenShots.Update(deltaTime, ExternalFire);
        }

        /// <summary>
        /// Sets a unique visual for the gun
        /// </summary>
        /// <param name="color">Color to set the barrel to</param>
        public void UpdateBarrelColor(Color color) 
        {
            if (gunStats.IsPlayerGun) 
            {
                Material newMaterial = new Material(barrel.material);
                newMaterial.color = color;
                barrel.material = newMaterial;
            }
        }

        /// <summary>
        /// Updates the nextTimeToFire variable. Will trigger TimeToFireComplete when nextTimeToFire ticks down.
        /// </summary>
        /// <param name="deltaTime">Amount of time since last frame update</param>
        private void UpdateNextTimeToFire(float deltaTime)
        {
            nextTimeToFire = Mathf.Clamp(nextTimeToFire + deltaTime, 0, float.MaxValue);

            if (nextTimeToFire > m_timeBetweenShots.TimeBetweenShots)
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

            if (overHeatPercent < gunStats.CoolDownPercentageBarrier && stateController.IsOverHeated)
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
            hitScan.Shoot(BulletSpawn.transform.position, direction);
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
            float angleStart = m_accuracy.Radius / 2;
            Quaternion rotationToApply = Quaternion.AngleAxis(-angleStart, Vector3.up);
            BulletSpawn.transform.rotation = BulletSpawn.transform.rotation * rotationToApply;

            Quaternion rotationPerIteration = Quaternion.AngleAxis(m_accuracy.DistanceBetweenProjectiles, Vector3.up);

            for (int i = 0; i < gunStats.ProjectilesReleasedPerShot; i++)
            {   
                Quaternion randomRotation = Quaternion.AngleAxis(m_accuracy.RandomDegreeRotation, Vector3.up);

                if (gunStats.ProjectilesReleasedPerShot == 1)
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


    }
}