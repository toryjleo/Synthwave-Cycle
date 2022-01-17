using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyborgAnimationStateController : MonoBehaviour
{
    Animator animator;

    int velocityHash;
    int shootHash;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null) 
        {
            Debug.LogError("Cyborg cannot find Animator Component");
        }
        velocityHash = Animator.StringToHash("Velocity");
        shootHash = Animator.StringToHash("Shoot");
    }

    public void SetSpeed(float speed) 
    {
        animator.SetFloat(velocityHash, speed);
    }

    public void Shoot() 
    {
        animator.SetTrigger(shootHash);
    }
}
