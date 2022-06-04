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
        Init();
    }

    protected virtual void OnDestroy()
    {
        DeInit();
    }

    public virtual void Init() 
    {
        if (ai != null) { ai.DeadEvent += SplatterBlood; }
    }

    public virtual void DeInit()
    {
        if (ai != null)
        {
            ai.DeadEvent -= SplatterBlood;
        }
    }


    private void SplatterBlood() 
    {
        Debug.Log("Blood sprays everywhere!");
    }
}
