using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SBomberAi : VehicleAi
{
    //How far the S-bomber will stay before deciding to dash into the player
    static float TRAIL_DISTANCE = 2f;

    [SerializeField]
    float ExplosionDamage = 50f;

    public override void Initialize()
    {

    }

    public override void Attack()
    {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        Debug.Log("S Bomber attacking!!!");
        // SetTarget(playerHealth.gameObject);
        vehicleController.enabled = false;
        rb.AddForce((pm.Velocity - transform.position).normalized * 250f, ForceMode.Impulse);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // if (collision != null && collision.collider.gameObject == target)
        // {
        //     target.GetComponent<PlayerHealth>().TakeDamage(ExplosionDamage);
        //     this.Die();
        // }

        if (collision.gameObject.tag == "Player")
        {
            addDlScore = false;
            target.GetComponent<PlayerHealth>().TakeDamage(ExplosionDamage);
            this.Die();
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            stateController.HandleTrigger(AIState.StateTrigger.FollowAgain);
        }
    }
}
