using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    protected float tickRate = 2f;
    private float lastInflicted = 0f;
    protected Ai hostAi;

    public void Tick()
    {
        if (Time.time - lastInflicted > 1 / tickRate)
        {
            lastInflicted = Time.time;
            InflictEffect();
        }
    }

    public void SetHostAi(Ai host)
    {
        hostAi = host;
    }

    internal abstract void InflictEffect();
}
