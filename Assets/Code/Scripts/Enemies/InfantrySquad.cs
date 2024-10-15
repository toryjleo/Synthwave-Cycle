using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>Class<c>InfantrySquad</c> 
/// Infantry Squad coordinates mutliple infantry units into one group
/// The squad handles forming up, moving to points and charging
public class InfantrySquad : Squad
{
    [SerializeField]
    public int minimumSize = 5; //if the squad is reduced to fewer than the minimum size, it will disband (will be replaced by confidence)

    [SerializeField]
    public int attackSize = 6; //if the squad has at least this many units, it will charge

    [SerializeField]
    public float weakPlayerHealthAmount = 1000; //If the player's energy falls below this number, they will be considered weak and AI will attack

    //pot shot variables
    [SerializeField]
    float timeBetweenPotShotsMS = 2000; //how many MS between pot shots +/- variance
    [SerializeField]
    float potShotVarianceMS = 1000; //range of +/- MS between pot shots (so they aren't on an exact interval)
    float nextShot = 0; //the time for the next shot, is reset to the current time in MS when a pot shot is taken

    public InfantrySquad(SquadManager _manager) : base(_manager) { }


    internal override void HandleAction()
    {
        switch (currentAction)
        {
            //try to move in between the current position and the target
            case SquadAction.Following:
                movementLoc = target.transform.position;
                if (IsConfident())
                {
                    currentAction = SquadAction.Attacking;
                }
                break;
            //rush the target
            case SquadAction.Attacking:
                if (!IsConfident())
                {
                    currentAction = SquadAction.Following;
                }
                movementLoc = target.transform.position;
                break;
            case SquadAction.Wandering:
                break;
        }
    }
    //Returns true if the squad is ready to charge
    private bool IsConfident()
    {
        return squadMembers.Count >= attackSize ||
               GetHitPoints(target) < weakPlayerHealthAmount;
    }
    internal override void HandleMovement()
    {
        foreach (Ai ai in squadMembers)
        {
            if (target == null)
            {
                // ai.Wander();
            }
            else
            {
                //Handle Movement
                Vector3 desiredVec = ai.transform.position - movementLoc;
                ai.Move(movementLoc);
                //if attacking and in range, fire at will
                if (currentAction == SquadAction.Attacking && desiredVec.magnitude < ai.attackRange)
                {
                    ai.Aim(target.transform.position);
                    ai.Attack();
                }
                // if following, take a pot shot
                // else if (currentAction == SquadAction.Following && Time.time > nextShot)
                // {
                //     ai.Aim(target.transform.position);
                //     ai.Attack();
                //     nextShot = Time.time + ((timeBetweenPotShotsMS + UnityEngine.Random.Range(-potShotVarianceMS, potShotVarianceMS)) / 1000);
                // }
            }
        }
    }

}
