using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerGunType
{
    OctoLMG,
    VulkanV64AutoCannons,
    DefaultGun
}

public class Arsenal : MonoBehaviour
{
    // TODO: Replace testGun with a hash map of PlayerGunType > gun gameobject
    public Gun testGun;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init() 
    {
        // TODO: Initializes all guns
    }

    public void Shoot(Vector3 initialVelocity) 
    {
        Debug.Log("Bang!");
        testGun.Shoot(initialVelocity);
    }

    public void EquipGun(PlayerGunType gunType)
    {
        // TODO: Does what BikScript does and inits gun   
    }

    public void DiscardGun(Gun gunToDiscard) 
    {
        // TODO: big gun shot is called
        gunToDiscard.gameObject.SetActive(false);
    }
}
