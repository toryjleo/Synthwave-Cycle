using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{


    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void OnDestroy()
    {
        DeInit();
    }
    /// <summary>Initializes veriables. Specifically must initialize lastFired and fireRate variables.</summary>
    public abstract void Init();

    /// <summary>Basically a destructor. Calls bulletPool.DeInit().</summary>
    public virtual void DeInit()
    {

    }

    //TODO: Add Code for Weapons and items being able to be picked up here! 



}
