using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Arsenal keeps track of guns that the player bike can equip, as well as handling the equip/dequip code
/// </summary>
public class Arsenal : MonoBehaviour, IResettable
{
    [SerializeField]
    public AudioSource weaponPickupSFX;

    private Dictionary<PlayerWeaponType, Weapon> weapons;
    private Weapon currentWeapon;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {

        playerMovement = GetComponentInParent<PlayerMovement>();

        weapons = new Dictionary<PlayerWeaponType, Weapon>();
        Weapon[] arsenalWeapons = this.GetComponentsInChildren<Weapon>();
        //iterate through all the Gun prefabs attached to the bike, initialize them, disable them, and register them in the dictionary
        for (int i = 0; i < arsenalWeapons.Length; i++)
        {
            arsenalWeapons[i].gameObject.transform.RotateAround(arsenalWeapons[i].transform.position, arsenalWeapons[i].transform.up, 180f);
            arsenalWeapons[i].gameObject.SetActive(false);
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
#if DEBUG
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
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //currentGun.Shoot(movementComponent.rb.velocity);
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

    private void DisableAllWeapons() 
    {
        currentWeapon = null;
        foreach (Weapon weapon in weapons.Values)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    //Tells the current gun to fire
    public void PrimaryFire(Vector3 initialVelocity) 
    {
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
