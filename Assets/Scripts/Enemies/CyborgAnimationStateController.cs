using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>CyborgAnimationStateController</c> A Unity Component controls the child animator component.</summary>
/// Requires child object to have an Animator component
public class CyborgAnimationStateController : MonoBehaviour
{
    Animator animator;


    string COMBAT_IDLE = "combat_idle";
    string WALKING = "<Blend Tree> Walking";

    int speedHash;
    int shootTriple;
    int shootSingle;
    int aimWhileWalking;

    int deathAHash;
    int deathBHash;
    int deathCHash;
    int isAliveHash;


    public float speed = 0;

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    private void Init() 
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Cyborg cannot find Animator Component");
        }
        else
        {
            speedHash = Animator.StringToHash("Speed");
            shootTriple = Animator.StringToHash("ShootTriple");
            shootSingle = Animator.StringToHash("ShootSingle");
            aimWhileWalking = Animator.StringToHash("IsAimingWhileWalking");
            deathAHash = Animator.StringToHash("Death_A");
            deathBHash = Animator.StringToHash("Death_B");
            deathCHash = Animator.StringToHash("Death_C");
            isAliveHash = Animator.StringToHash("isAlive");
        }
    }

    protected private void Update()
    {/*
        if (Input.GetKeyDown(KeyCode.Alpha7)) 
        {
            ShootSingle();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TriggerDeathB();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            TriggerDeathC();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetAlive(true);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            AimWhileWalking(true);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            AimWhileWalking(false);
        }

        SetSpeed(speed);*/
    }

    /// <summary>Updates the animation controller with the current speed. Should be called every frame when awake.</summary>
    /// <param name="speed">Value ranging from 0 to 1. 0 is 0% of the max speed and 1 is 100% of the max speed. The walking animation will only start when a speed of greater than .1f is met.</param>
    public void SetSpeed(float speed) 
    {
        animator.SetFloat(speedHash, speed);
    }

    /// <summary>Updates the state machine with the current "Aliveness" of the Enemy. Must be updated whenever the enemy's "Alive" status changes.</summary>
    /// <param name="isAlive">Bool telling the state machine if the AI is alive. "True" means it is alive</param>
    public void SetAlive(bool isAlive) 
    {
        animator.SetBool(isAliveHash, isAlive);
    }

    #region Shooting

    /// <summary>Will return true if the animator is in the idle blend tree.</summary>
    /// <returns>True if the animator is in the idle blend tree.</returns>
    public bool IsIdle() 
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(COMBAT_IDLE);
    }

    /// <summary>Will return true if the animator is in the walking blend tree.</summary>
    /// <returns>True if the animator is in the walking blend tree.</returns>
    public bool IsWalking() 
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(WALKING);
    }

    /// <summary>
    /// Sets a boolean to trigger an aiming while walking animation when moving
    /// </summary>
    /// <param name="useWalkAim">When set to true, will use the aiming whle walking animation when moving</param>
    public void AimWhileWalking(bool useWalkAim)
    {
        animator.SetBool(aimWhileWalking, useWalkAim);
    }

    /// <summary>
    /// This method plays the triple shot animation when the character is at idle
    /// </summary>
    public void ShootTriple()
    {
        if (IsIdle())
        {
            animator.SetTrigger(shootTriple);
        }
        else 
        {
            Debug.LogError("Tried to call ShootTriple while not at idle! Check your speed!");
        }
    }

    /// <summary>
    /// This method plays the single shot animation when the character is at idle
    /// </summary>
    public void ShootSingle()
    {
        if (IsIdle())
        {
            animator.SetTrigger(shootSingle);
        }
        else
        {
            Debug.LogError("Tried to call ShootSingle while not at idle! Check your speed!");
        }
    }
    #endregion

    #region DeathAnimations
    // Death Animations can run at any given state. Will Set "isAlive" status to "False"
    public void TriggerDeathA() 
    {
        SetAlive(false);
        animator.SetTrigger(deathAHash);
    }

    public void TriggerDeathB()
    {
        SetAlive(false);
        animator.SetTrigger(deathBHash);
    }

    public void TriggerDeathC()
    {
        SetAlive(false);
        animator.SetTrigger(deathCHash);
    }

    #endregion
}
