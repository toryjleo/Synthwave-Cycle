using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>interface <c>IWorldGenrationObject</c> An object that generates information dependant on the world. </summary>
public abstract class IWorldGenerationObject : MonoBehaviour
{
    public abstract void Init();

    public abstract void WorldUpdated();
}
