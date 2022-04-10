using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>Class <c>SelfWorldBoundsDespawn</c> An abstract class that contains despawn code specific to gameObjects
/// that are supposed to despawn when outside of the world bounds.</summary>
public abstract class SelfWorldBoundsDespawn : SelfDespawn
{

    /// <summary>Returns true if the object is out of the world bounds.</summary>
    /// <returns>A bool. "True" insicating the object should despawn, "False" indicates the object should not 
    /// despawn.</returns>
    protected bool IsOutOfWorldBounds()
    {
        Vector3 objectPosition = transform.position;
        float bikeHorizontalPos = objectPosition.x;
        float bikeVerticalPos = objectPosition.z;
        if (bikeHorizontalPos > WorldBounds.worldBoundsHorizontalMinMax.y) { return true; }  // Right Quyadrant
        else if (bikeHorizontalPos < WorldBounds.worldBoundsHorizontalMinMax.x) { return true; }  // Left Quadrant
        else if (bikeVerticalPos > WorldBounds.worldBoundsVericalMinMax.y) { return true; }  // Upper Quadrant
        else if (bikeVerticalPos < WorldBounds.worldBoundsVericalMinMax.x) { return true; }  // Lower Quadrant
        else { return false; } // Inside world bounds
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
