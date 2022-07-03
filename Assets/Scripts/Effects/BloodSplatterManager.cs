using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages blood splatters for enemy AI's. Enemy AI's WILL NOT reference this script.
/// </summary>
public class BloodSplatterManager : MonoBehaviour
{
    // Need to manually assign AI
    [SerializeField] private Ai ai;
    [SerializeField] private BloodSplatter bloodSplatter;


    // Start is called before the first frame update
    void Start()
    {
        if (ai != null) 
        { 
            // Hook up events
            ai.DeadEvent += ShowBlood;
            // Make sure that the blood splatters get turned off when the enemy despawns
            ai.Despawn += Init;
        }
        
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Init();
            bloodSplatter.DisplayBlood();
        }
    }

    protected virtual void OnDestroy()
    {
        DeInit();
    }

    /// <summary>
    /// Initialization function to be used by Despawn calls.
    /// </summary>
    /// <param name="entity">Entity getting despawned.</param>
    public void Init(SelfDespawn entity) 
    {
        Init();
    }

    public virtual void Init() 
    {
        bloodSplatter.Init();
    }

    /// <summary>
    /// Deinitializes this instance.
    /// </summary>
    public virtual void DeInit()
    {
        if (ai != null)
        {
            ai.DeadEvent -= ShowBlood;
        }
    }

    /// <summary>
    /// Displays blood upon enemy death.
    /// </summary>
    private void ShowBlood() 
    {
        bloodSplatter.DisplayBlood();
    }
}
