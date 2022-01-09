using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void NotifyReadyToDespawn(SelfDespawn entity);  // delegate

/// <summary>Class <c>SelfDespawn</c> An abstract class that contains despawn code.</summary>
public abstract class SelfDespawn : MonoBehaviour
{
    public event NotifyReadyToDespawn Despawn; // event


    public abstract void Init();

    protected virtual void OnDespawn() // protected virtual method
    {
        // if Despawn is not null then call delegate
        Despawn?.Invoke(this);
    }

    /// <summary>Returns true if the object is out of the world bounds.</summary>
    /// <returns>A bool. "True" insicating the object should despawn, "False" indicates the object should not 
    /// despawn.</returns>
    protected bool IsOutOfWorldBounds()
    {
        Vector3 objectPosition = transform.position;
        float bikeHorizontalPos = objectPosition.x;
        float bikeVerticalPos = objectPosition.z;
        if      (bikeHorizontalPos > WorldBounds.worldBoundsHorizontalMinMax.y) { return true; }  // Right Quyadrant
        else if (bikeHorizontalPos < WorldBounds.worldBoundsHorizontalMinMax.x) { return true; }  // Left Quadrant
        else if (bikeVerticalPos > WorldBounds.worldBoundsVericalMinMax.y)      { return true; }  // Upper Quadrant
        else if (bikeVerticalPos < WorldBounds.worldBoundsVericalMinMax.x)      { return true; }  // Lower Quadrant
        else                                                                    { return false; } // Inside world bounds
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (IsOutOfWorldBounds())
        {
            // Return to object pool
            OnDespawn();
        }
    } 
}
