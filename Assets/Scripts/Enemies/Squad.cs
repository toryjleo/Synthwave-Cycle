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

/// <summary>Class <c>Squad</c> A Squad is a group of AI that will work and move together</summary>
/// Squads will determine how aggressive they are based on their own strength compared to the player's

public class Squad
{
    [SerializeField]
    public int minimumSize = 5; //if the squad is reduced to fewer than the minimum size, it will disband (will be replaced by confidence)
    
    [SerializeField]
    public int attackSize = 6; //if the squad has at least this many units, it will charge

    [SerializeField]
    public float weakPlayerHealthAmount = 75; //If the player's energy falls below this number, they will be considered weak and AI will attack

    [SerializeField]
    List<Ai> squadMembers = new List<Ai>();
    private GameObject target;
    private Vector3 movementLoc; //this is the location squad members will try to surround
    private Vector3 squadCenter;
    SquadManager manager;
    SquadAction currentAction = SquadAction.Following;

    //pot shot variables
    [SerializeField]
    float timeBetweenPotShotsMS = 2000;
    [SerializeField]
    float potShotVarianceMS = 1000;

    float nextShot = 0;


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
        switch (currentAction)
        {
            //try to move in between the current position and the target
            case SquadAction.Following:
                movementLoc = (squadCenter + target.transform.position) / 2;
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
        }
        Debug.DrawLine(squadCenter, movementLoc, currentAction==SquadAction.Attacking?Color.yellow:Color.cyan);
        foreach (Ai ai in squadMembers)
        {
            Debug.DrawLine(ai.transform.position, squadCenter, Color.red);
            //handle AI death
            if(!ai.IsAlive())
            {
                squadMembers.Remove(ai);
                manager.currentEnemies.Remove(ai);
                manager.scoreKeeper.AddToScore((int)ai.GetScore());
                if(squadMembers.Count < minimumSize)
                {
                    manager.DisbandSquad(this);
                }
                break;
            }
            if (target == null || !GameStateController.IsGamePlaying())
            {
                ai.Wander();
            }
            else
            {
                //Handle Movement
                Vector3 desiredVec = ai.transform.position - movementLoc;
                ai.Move(movementLoc, currentAction == SquadAction.Attacking);
                //if attacking and in range, fire at will
                if (currentAction == SquadAction.Attacking && desiredVec.magnitude < ai.attackRange)
                {
                    ai.Aim(target.transform.position);
                    ai.Attack();
                }
                //if following, take a pot shot
                else if (currentAction == SquadAction.Following && Time.time > nextShot)
                {
                    ai.Aim(target.transform.position);
                    ai.Attack();
                    nextShot = Time.time + ((timeBetweenPotShotsMS + UnityEngine.Random.Range(-potShotVarianceMS, potShotVarianceMS)) / 1000);
                }
            }
        }
    }


    private float GetHitPoints(GameObject target)
    {
        BikeScript bikeScript = target.GetComponent<BikeScript>();
        if(bikeScript != null) 
        {
            return bikeScript.Energy;
        }
        else
        {
            return float.MaxValue;
        }
    }

    //Returns true if the squad is ready to charge
    private bool IsConfident()
    {
        return squadMembers.Count >= attackSize ||
               GetHitPoints(target) < weakPlayerHealthAmount;
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void AddToSquad(Ai newMember)
    {
        squadMembers.Add(newMember);
        newMember.SetTarget(target);
        Debug.Log("Joined a squad! Current Size: " + squadMembers.Count);
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
