using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCrate : MonoBehaviour
{
    public Gun crateWeapon;
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bonked with: " + collision);
        if(collision.gameObject.tag == "Player")
        {
            BikeScript playerBikeScript = (BikeScript)FindObjectOfType(typeof(BikeScript));
            playerBikeScript.EquipGun(crateWeapon);
            Destroy(this.gameObject);
        }
    }
}
