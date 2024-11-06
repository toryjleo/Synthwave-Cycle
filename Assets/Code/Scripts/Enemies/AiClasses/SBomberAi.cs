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

    [SerializeField]
    GameObject AttackTelegraph; // this object appears and disappears when the target is preparing to attack

    public override void Initialize()
    {

    }

    public override void Attack()
    {
        if (timeByTarget >= stats.TimeToAttack)
        {
            //Dive bomb the player
            vehicleController.enabled = false;
            rb.AddForce(Vector3.Normalize(target.transform.position - this.transform.position) * 1000f * Time.fixedDeltaTime);
        }
    }

    //S-Bomber hovers near player building confidence before charging and exploding
    public override void UpdateMovementLocation()
    {
        if (timeByTarget >= stats.TimeToAttack - 3) // Telegraph attack 3 seconds before charge
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

    public override void Chase(Vector3 target, float fixedDeltaTime)
    {
        throw new NotImplementedException();
    }

    public override void Wander(Vector3 wanderDirection, float fixedDeltaTime)
    {
        throw new NotImplementedException();
    }

    public override void Separate(ArrayList pool, float fixedDeltaTime)
    {
        throw new NotImplementedException();
    }

    public override void Group(ArrayList pool, float fixedDeltaTime)
    {
        throw new NotImplementedException();
    }
}
