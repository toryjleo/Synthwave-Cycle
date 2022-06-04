using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterManager : MonoBehaviour
{
    // Need to manually assign AI
    public Ai ai;
    public BloodSplatter bloodSplatter;


    // Start is called before the first frame update
    void Start()
    {
        if (ai != null) 
        { 
            ai.DeadEvent += ShowBlood;
            // Make sure that the blood splatters get turned off when the enemy despawns
            ai.Despawn += Init;
        }
        
        Init();
    }

    protected virtual void OnDestroy()
    {
        DeInit();
    }

    public void Init(SelfDespawn entity) 
    {
        Init();
    }

    public virtual void Init() 
    {
        bloodSplatter.Init();
    }

    public virtual void DeInit()
    {
        if (ai != null)
        {
            ai.DeadEvent -= ShowBlood;
        }
    }


    private void ShowBlood() 
    {
        Debug.Log("Blood sprays everywhere!");
        bloodSplatter.DisplayBlood();
    }
}
