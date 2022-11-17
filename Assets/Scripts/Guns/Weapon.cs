using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the base abstract class that will be used for all weapons and guns. 
/// </summary>
public abstract class Weapon : MonoBehaviour
{

    /// <summary>
    /// The Awake Function, initializes the Weapon 
    /// </summary>
    protected virtual void Awake()
    {
        Init();
    }
    /// <summary>
    /// Used to deInit the weapon 
    /// </summary>
    protected virtual void OnDestroy()
    {
        DeInit();
    }
    /// <summary>Initializes veriables. Specifically must initialize lastFired and fireRate variables.</summary>
    public abstract void Init();

    /// <summary>Basically a destructor. Calls bulletPool.DeInit().</summary>
    public virtual void DeInit()
    {
       //TODO: add more featuers? Thoughts
    }

    //TODO: Add Code for Weapons and items being able to be picked up here! 



}
