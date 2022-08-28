using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TickCondition : MonoBehaviour
{
    protected float tickRate = 2f;
    private float lastInflicted = 0f;
    //protected Ai hostAi;

    /// <summary>Checks to see if the tick timer has been triggered</summary>
    public bool CheckTick()
    {
        if (Time.time - lastInflicted > 1 / tickRate)
        {
            lastInflicted = Time.time;
            return true;
        }
        return false;
    }

    /// <summary>Overridden function that will trigger every interval to effect the HostAi</summary>
    internal abstract void InflictEffect(Ai host);
}
