using EditorObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Arsenal keeps track of guns that the player bike can equip, as well as handling the equip/dequip code
/// </summary>
public class Arsenal : MonoBehaviour, IResettable
{
    EditorObject.Arsenal savedData;

    [SerializeField] private Gun.Gun selected;


    private void Update()
    {
        if (selected != null) 
        {
            selected.ExternalFire = CheckCanShootGun();
        }
    }

    public void Init(GameSave gameSave) 
    {
        this.savedData = gameSave.arsenal;
        // TODO: instantiate all gun prefabs
        // TODO: Assign selected
    }

    public void ResetGameObject()
    {
        // TODO: Reset to scriptableobject's state
    }

    private bool CheckCanShootGun() 
    {
        if (selected.IsAutomatic && Input.GetButton("Fire1"))
        {
            // Automatic fire case
            return true;
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            // Semi-Automatic fire case
            return true;
        }
        else 
        { 
            return false;
        }
    }
}
