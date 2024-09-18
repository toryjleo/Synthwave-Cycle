using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;


public delegate void NotifyReadyToDespawn(SelfDespawn entity);  // delegate

/// <summary>Class <c>SelfDespawn</c> An abstract class that contains despawn code.</summary>
public abstract class SelfDespawn : MonoBehaviour
{
    #region Has Finite Lifetime
    [SerializeField] protected bool hasFiniteLifetime = false;
    [SerializeField] protected float lifetime = float.MaxValue;
    protected float timeInWorld = 0;
    #endregion

    public event NotifyReadyToDespawn Despawn; // event

    private void Update()
    {
        UpdateLifetime();
    }

    protected virtual void OnDespawn() // protected virtual method
    {
        // if Despawn is not null then call delegate
        Despawn?.Invoke(this);
    }


    #region Has Finite Lifetime
    /// <summary>Code that applies changes to a bullet over its lifetime.</summary>
    protected virtual void UpdateLifetime()
    {
        // Check if the bullet's lifetime is up
        if (hasFiniteLifetime)
        {
            timeInWorld += Time.deltaTime;
            if (timeInWorld >= lifetime)
            {
                OnDespawn();
            }
        }
    }
    #endregion
}
