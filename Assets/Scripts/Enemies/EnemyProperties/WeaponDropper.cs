using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropper : MonoBehaviour
{

    [SerializeField]
    private GameObject weaponDropPrefab;


    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            DropWeapon();
        }
    }

    void DropWeapon()
    {
        Debug.Log("Dropped weapon");
    
    }
}
