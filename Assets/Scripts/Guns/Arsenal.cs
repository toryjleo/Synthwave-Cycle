using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Arsenal keeps track of guns that the player bike can equip, as well as handling the equip/dequip code
/// </summary>
public class Arsenal : MonoBehaviour, IResettable
{
    #region InputManagerStrings
    private const string FIRE1 = "Fire1";
    #endregion

    [SerializeField]
    public AudioSource weaponPickupSFX;

    private Dictionary<PlayerWeaponType, Weapon> weapons;
    private Weapon currentWeapon = null;
    private Turret turret = null;
    private PlayerMovement playerMovement = null;

    // Start is called before the first frame update
    void Start()
    {

        playerMovement = GetComponentInParent<PlayerMovement>();

        weapons = new Dictionary<PlayerWeaponType, Weapon>();
        Weapon[] arsenalWeapons = this.GetComponentsInChildren<Weapon>();

        // iterate through all the Gun prefabs attached to the bike, initialize them, disable them, and register them in the dictionary
        for (int i = 0; i < arsenalWeapons.Length; i++)
        {
            switch (arsenalWeapons[i].GetPlayerWeaponType())
            {
                // Disable all weapons except for turret
                case PlayerWeaponType.PowerGlove:
                    arsenalWeapons[i].gameObject.SetActive(true);
                    turret = arsenalWeapons[i] as Turret;
                    break;
                case PlayerWeaponType.PinkMist:
                    PlayerHealth playerHealth = GetComponentInParent<PlayerHealth>();
                    if (playerHealth == null)
                    {
                        Debug.LogWarning("Arsenal not child of object with PlayerHealth Component");
                    }
                    else
                    {
                        playerHealth.onBarUpdate += ((PinkMist)arsenalWeapons[i]).HandleBarUpdate;
                    }
                    break;
                default:
                    arsenalWeapons[i].gameObject.SetActive(false);
                    break;

            }

            // Add weapon to dictionary
            weapons.Add(arsenalWeapons[i].GetPlayerWeaponType(), arsenalWeapons[i]);


            if (arsenalWeapons[i] is Gun)
            {
                ((Gun)arsenalWeapons[i]).BulletShot += playerMovement.ApplyShotForce;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            EquipGun(PlayerWeaponType.DefaultGun);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            EquipGun(PlayerWeaponType.OctoLMG);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            EquipGun(PlayerWeaponType.Shotty);
        }
#endif
    }

    private void FixedUpdate()
    {

        if (GameStateController.CanRunGameplay)
        {
            // Handle primary and secondary fire inputs
            if (Input.GetAxis(FIRE1) > .1f)
            {
                PrimaryFire(playerMovement.Velocity);
            }
            else
            {
                ReleasePrimaryFire(playerMovement.Velocity);
            }

            // Handle Secondary Fire Input
            if (Input.GetKey(KeyCode.Mouse1))
            {
                SecondaryFire(playerMovement.Velocity);
            }
            else
            {
                ReleaseSecondaryFire(playerMovement.Velocity);
            }
        }
    }

    /// <summary>
    /// Disable all weapons except for turret
    /// </summary>
    private void DisableAllWeapons() 
    {
        currentWeapon = null;
        foreach (Weapon weapon in weapons.Values)
        {
            if (weapon != turret) 
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }

    //Tells the current gun to fire
    public void PrimaryFire(Vector3 initialVelocity) 
    {
        turret.PrimaryFire(initialVelocity);

        if (currentWeapon != null)
        {
            currentWeapon.PrimaryFire(initialVelocity);
        }
    }

    /// <summary>
    /// Calls weapon's implementation of ReleasePrimaryFire.
    /// </summary>
    /// <param name="initialVelocity">Current velocity of the bike</param>
    public void ReleasePrimaryFire(Vector3 initialVelocity)
    {
        turret.ReleasePrimaryFire(initialVelocity);
        if (currentWeapon != null)
        {
            currentWeapon.ReleasePrimaryFire(initialVelocity);
        }
    }

    /// <summary>
    /// Calls weapon's implementation of SecondaryFire.
    /// </summary>
    /// <param name="initialVelocity">Current velocity of the bike</param>
    public void SecondaryFire(Vector3 initialVelocity)
    {
        if (currentWeapon != null)
        {
            currentWeapon.SecondaryFire(initialVelocity);
        }
    }

    /// <summary>
    /// Calls weapon's implementation of ReleaseSecondaryFire.
    /// </summary>
    /// <param name="initialVelocity">Current velocity of the bike</param>
    public void ReleaseSecondaryFire(Vector3 initialVelocity)
    {
        if (currentWeapon != null)
        {
            currentWeapon.ReleaseSecondaryFire(initialVelocity);
        }
    }

    //Discards current gun and inits/equips new gun
    public void EquipGun(PlayerWeaponType gunType)
    {
        if(weapons.ContainsKey(gunType))
        {
            if (currentWeapon != null)
            {
                DiscardGun(currentWeapon);
            }

            currentWeapon = weapons[gunType];
            currentWeapon.Init();
            weaponPickupSFX.clip = currentWeapon.PickupSound;
            weaponPickupSFX.Play();
            currentWeapon.gameObject.SetActive(true);
        }
    }

    //Un-registers shooting event and disables current gun
    public void DiscardGun(Weapon gunToDiscard) 
    {
        if(currentWeapon is LeveledGun)
        {
            ((LeveledGun)currentWeapon).BigBoom();
            ((LeveledGun)currentWeapon).LevelUp();
        }
        gunToDiscard.gameObject.SetActive(false);
    }

    public void ResetGameObject()
    {
        DisableAllWeapons();
    }
}
