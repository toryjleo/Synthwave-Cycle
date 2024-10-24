using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void NotifyReadyToDespawn(SelfDespawn entity);  // delegate

/// <summary>Class <c>SelfDespawn</c> An abstract class that contains despawn code.</summary>
public abstract class SelfDespawn : MonoBehaviour
{
    public event NotifyReadyToDespawn Despawn; // event

    public abstract void Initialize();

    protected virtual void OnDespawn() // protected virtual method
    {
        // if Despawn is not null then call delegate
        Despawn?.Invoke(this);
    }
}
