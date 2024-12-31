using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SBomberAi : VehicleAi
{
    [SerializeField] float ExplosionDamage = 50f;

    private float timer = 0f;
    private bool timerCountdown = false;

    //TODO: Spawn explosion when they are killed (heath.Kill())

    public override void ManualUpdate(ArrayList enemies, Vector3 wanderDirection, float fixedDeltaTime)
    {
        if (timerCountdown && !stateController.isDead)
        {
            timer += fixedDeltaTime;
            if (timer >= stats.TimeToDie)
            {
                addDlScore = false;
                health.Kill();
                timerCountdown = false;
            }
        }

        base.ManualUpdate(enemies, wanderDirection, fixedDeltaTime);
    }

    public override void Attack()
    {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();

        vehicleController.enabled = false;

        Vector3 direction = ((pm.Velocity + target.transform.position) - transform.position).normalized;
        rb.AddForce(direction * 200f, ForceMode.Impulse);

        timerCountdown = true;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!stateController.isDead)
        {
            if (collision.gameObject.tag == "Player")
            {
                addDlScore = false;
                target.GetComponent<PlayerHealth>().TakeDamage(ExplosionDamage);
                health.Kill();
            }
            else if (collision.gameObject.tag == "Enemy")
            {
                stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
            }
        }
    }

    public override void Reset()
    {
        timer = 0f;
        timerCountdown = false;
        base.Reset();
    }
}
