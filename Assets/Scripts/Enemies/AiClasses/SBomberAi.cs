using System;
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

    [SerializeField]
    GameObject AttackTelegraph; // this object appears and disappears when the target is preparing to attack

    public override void Attack()
    {
        if (timeByTarget >= TIME_BY_TARGET_TO_ATTACK && IsAlive())
        {
            //Dive bomb the player
            vehicleController.enabled = false;
            this.applyForce(Vector3.Normalize(target.transform.position - this.transform.position) * 1000f);
        }
    }

    public override Enemy GetEnemyType()
    {
        return Enemy.SBomber;
    }

    //S-Bomber hovers near player building confidence before charging and exploding
    public override void UpdateMovementLocation()
    {
        if(timeByTarget >= TIME_BY_TARGET_TO_ATTACK - 3) // Telegraph attack 3 seconds before charge
        {
            AttackTelegraph.SetActive(true);
        }
        else
        {
            AttackTelegraph.SetActive(false);
        }
        if (target != null)
        {
            //have we hovered by the player long enough to attack?
            movementTargetPosition.transform.position = TRAIL_DISTANCE * Vector3.Normalize(this.transform.position - target.transform.position) + target.transform.position;
            vehicleController.target = movementTargetPosition.transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.collider.gameObject == target) 
        {
            target.GetComponent<PlayerHealth>().TakeDamage(ExplosionDamage);
            this.Die();
        }
    }
}
