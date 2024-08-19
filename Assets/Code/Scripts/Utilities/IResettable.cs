using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResettable
{
    /// <summary>
    /// Resets values so that they are the same as after the initial Start() call
    /// </summary>
    public void ResetGameObject();
}