using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitCondition : MonoBehaviour
{

    /// <summary>Overridden function that will trigger when the host AI is hit</summary>
    internal abstract void InflictEffect(Ai host, float damage);
}
