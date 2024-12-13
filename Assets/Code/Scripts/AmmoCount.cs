using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to track ammo amounts for a given object
/// </summary>
[Serializable]
public class AmmoCount
{
    [SerializeField] private bool infiniteAmmo = false;
    [SerializeField] private int maxAmmo = 0;
    [SerializeField] private int ammoCount = 0;

    public int MaxAmmo
    {
        get { return maxAmmo; }
    }

    public bool AtMaxAmmo
    {
        get => ((ammoCount == MaxAmmo) || IsInfiniteAmmo);
    }

    public bool IsInfiniteAmmo
    {
        get { return infiniteAmmo; }
    }

    public int Count
    {
        get => ammoCount;
    }

    public AmmoCount(EditorObject.GunStats gunStats)
    {
        infiniteAmmo = gunStats.InfiniteAmmo;
        maxAmmo = gunStats.AmmoCount;
        ammoCount = gunStats.AmmoCount;
    }

    /// <summary>
    /// Adds ammo to the ammo count
    /// </summary>
    /// <param name="amount">Amount of ammo to add</param>
    public int AddAmmo(int amount)
    {
        int newCount = ammoCount + amount;
        if (newCount > MaxAmmo)
        {
            ammoCount = MaxAmmo;
            return newCount - MaxAmmo;
        }
        else
        {
            ammoCount = newCount;
            return 0;
        }
    }

    /// <summary>
    /// Reduces ammo by 1. Will trigger OutOfAmmo when ammoCount hits zero
    /// </summary>
    public void ReduceAmmo()
    {
        ammoCount = infiniteAmmo ? ammoCount : ammoCount - 1;
    }

    /// <summary>
    /// Sets the ammo to this gun
    /// </summary>
    /// <param name="amount">Value to set the ammo to</param>
    public void SetAmmo(int amount)
    {
        ammoCount = Mathf.Clamp(amount, 0, MaxAmmo);
    }

    /// <summary>
    /// Sets the ammo to this gun
    /// </summary>
    /// <param name="amount">Value to set the ammo to</param>
    public void SetMaxAmmo()
    {
        ammoCount = MaxAmmo;
    }
}
