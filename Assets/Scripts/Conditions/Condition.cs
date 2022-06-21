using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Conditions are applied to EnemyAis. On set tick intervals, they can cause different effects</summary>
public abstract class Condition : MonoBehaviour
{
    protected float tickRate = 2f;
    private float lastInflicted = 0f;
    protected Ai hostAi;

    /// <summary>Checks to see if the tick timer has been triggered</summary>
    public void Tick()
    {
        if (Time.time - lastInflicted > 1 / tickRate)
        {
            lastInflicted = Time.time;
            InflictEffect();
        }
    }

    /// <summary>Applies this condition to an Ai</summary>
    public void SetHostAi(Ai host)
    {
        hostAi = host;
    }
    /// <summary>Overridden function that will trigger every interval to effect the HostAi</summary>
    internal abstract void InflictEffect();
}
