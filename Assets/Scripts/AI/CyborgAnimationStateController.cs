using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyborgAnimationStateController : MonoBehaviour
{
    Animator animator;

    int velocityHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        velocityHash = Animator.StringToHash("Velocity");
        if (animator == null) 
        {
            Debug.LogError("Cyborg cannot find Animator Component");
        }
    }

    public void SetSpeed(float speed) 
    {
        animator.SetFloat(velocityHash, speed);
    }
}
