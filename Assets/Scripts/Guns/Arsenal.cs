using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerGunType
{
    DefaultGun,
    OctoLMG,
    VulkanV64AutoCannons,
    INVALID
}

public class Arsenal : MonoBehaviour
{
    private Dictionary<PlayerGunType, Gun> guns;
    private Gun currentGun;
    private BikeScript playerBike;

    // Start is called before the first frame update
    void Start()
    {
        playerBike = GetComponentInParent<BikeScript>();

        guns = new Dictionary<PlayerGunType, Gun>();
        Gun[] arsenalWeapons = this.GetComponentsInChildren<Gun>();
        for (int i = 0; i < arsenalWeapons.Length; i++)
        {
            //arsenalWeapons[i] = Instantiate(arsenalWeapons[i], playerBike.movementComponent.transform.position, Quaternion.identity);
            arsenalWeapons[i].transform.parent = playerBike.movementComponent.bikeMeshParent.transform;
            arsenalWeapons[i].transform.rotation = playerBike.movementComponent.bikeMeshParent.transform.rotation;
            arsenalWeapons[i].transform.RotateAround(arsenalWeapons[i].transform.position, arsenalWeapons[i].transform.up, 180f);
            arsenalWeapons[i].gameObject.SetActive(false);
            guns.Add(arsenalWeapons[i].GetPlayerGunType(), arsenalWeapons[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.L))
        //{
        //    EquipGun(PlayerGunType.DefaultGun);
        //}
        //else if(Input.GetKeyDown(KeyCode.O))
        //{
        //    EquipGun(PlayerGunType.OctoLMG);
        //}
    }

    public void Init() 
    {

    }

    public void Shoot(Vector3 initialVelocity) 
    {
        if (currentGun != null)
        {
            currentGun.Shoot(initialVelocity);
        }
    }

    public void EquipGun(PlayerGunType gunType)
    {
        if(guns.ContainsKey(gunType))
        {
            if (currentGun != null)
            {
                DiscardGun(currentGun);
            }

            currentGun = guns[gunType];
            currentGun.Init();
            currentGun.BulletShot += playerBike.movementComponent.bl_ProcessCompleted;
            currentGun.gameObject.SetActive(true);
        }
    }

    public void DiscardGun(Gun gunToDiscard) 
    {
        // TODO: big gun shot is called
        // TODO: (after impl.) Level up a LevelledGun
        gunToDiscard.BulletShot -= playerBike.movementComponent.bl_ProcessCompleted;
        gunToDiscard.gameObject.SetActive(false);
    }
}
