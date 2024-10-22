using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EditorObject;
using Generic;
using UnityEngine;

/// <summary>Class<c>InfantryAI</c> 
/// Infantry AI is the abstract base class for all enemy footsoldiers
/// This handles their movement, combat, animation, and respawning
public abstract class InfantryAI : Ai
{
    public CyborgAnimationStateController animationStateController;

    public override void Attack()
    {
        if (myGun != null && myGun.CanShootAgain() && alive)
        {
            this.myGun.PrimaryFire(target.transform.position);
            animationStateController.AimWhileWalking(true);
        }
    }

    public override void Update()
    {
        SetAnimationSpeed(rb.velocity.magnitude);
        base.Update();
    }

    public override void Init(IPoolableInstantiateData stats)
    {
        TestAi aiStats = stats as TestAi;
        if (!aiStats)
        {
            Debug.LogWarning("InfantryAi stats are not readable as TestAi!");
        }

        alive = true;

        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody>();
        animationStateController = GetComponent<CyborgAnimationStateController>();
        this.Despawn += op_ProcessCompleted;
        hp.Init(aiStats.Health);

        myGun.Init();

        // Error checking
        if (animationStateController == null)
        {
            Debug.LogError("This object needs a CyborgAnimationStateController component");
        }
        if (rb == null)
        {
            Debug.LogError("This object needs a rigidBody component");
        }
        if (hp == null)
        {
            Debug.LogError("This object needs a health component");
        }

        base.Init(stats);
    }

    /// <summary>
    /// This Metod is used to set the animation speed without causing errors or Ai that do not have an animation state controller.
    /// </summary>
    /// <param name="animationSpeed"></param>
    public virtual void SetAnimationSpeed(float animationSpeed)
    {
        if (animationStateController != null)
        {
            animationStateController.SetSpeed(animationSpeed);
        }
    }

    public override void Die()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;

        if (alive)
        {
            animationStateController.TriggerDeathA();//TODO: add catch

            animationStateController.TriggerDeathA();

            rb.detectCollisions = false;
            animationStateController.SetAlive(false);

        }
        base.Die();
    }

    public override void NewLife()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        animationStateController.SetAlive(true);
        base.NewLife();
    }
}
