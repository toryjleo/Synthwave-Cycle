using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

enum SquadAction
{
    Wandering,
    Following,
    Attacking
}


public class Squad
{
    [SerializeField]
    public int minimumSize; //if the squad is reduced to fewer than the minimum size, it will disband (will be replaced by confidence)

    [SerializeField]
    List<Ai> squadMembers = new List<Ai>();
    private GameObject target;
    private Vector3 movementLoc; //this is the location squad members will try to surround
    private Vector3 squadCenter;
    SquadManager manager;
    SquadAction currentAction = SquadAction.Following;

    public Squad(SquadManager _manager)
    {
        this.manager = _manager;
    }

    // Update is called once per frame
    internal void Update()
    {
        squadCenter = Vector3.zero;
        foreach(Ai ai in squadMembers)
        {
            squadCenter.x += ai.transform.position.x;
            squadCenter.z += ai.transform.position.z;
        }
        squadCenter.x /= squadMembers.Count;
        squadCenter.z /= squadMembers.Count;
        switch(currentAction)
        {
            case SquadAction.Following:
                movementLoc = (squadCenter + target.transform.position) / 2;
                    break;
            case SquadAction.Attacking:
                movementLoc = target.transform.position;
                break;
        }
        foreach (Ai ai in squadMembers)
        {
            if(!ai.IsAlive())
            {
                squadMembers.Remove(ai);
                manager.currentEnemies.Remove(ai);
                manager.scoreKeeper.AddToScore((int)ai.GetScore());
                if(squadMembers.Count < 3)
                {
                    manager.DisbandSquad(this);
                }
                break;
            }
            ai.Seperate(squadMembers);
            if (target == null || !GameStateController.IsGamePlaying())
            {
                ai.Wander();
            }
            else
            {
                Vector3 desiredVec = ai.transform.position - movementLoc;
                if (currentAction == SquadAction.Attacking && desiredVec.magnitude < ai.attackRange)
                {
                    ai.Move(ai.transform.position);
                    ai.Aim(target.transform.position);
                    ai.Attack();
                    ai.SetAnimationSpeed(0);
                }
                else
                {
                    ai.Move(movementLoc);
                    ai.SetAnimationSpeed(ai.rb.velocity.magnitude / 40); //this devides them by a constant to allow for slower enemies to walk slower.
                }
            }
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void SetDestination(Vector3 destination)
    {
        movementLoc = destination;
    }

    public void AddToSquad(Ai newMember)
    {
        squadMembers.Add(newMember);
        newMember.SetTarget(target);
    }

    internal Vector3 GetCenter()
    {
        return movementLoc;
    }

    internal IEnumerable<Ai> GetSquadMembers()
    {
        return squadMembers;
    }

    internal void SetAction(SquadAction action)
    {
        currentAction = action;
    }
}
