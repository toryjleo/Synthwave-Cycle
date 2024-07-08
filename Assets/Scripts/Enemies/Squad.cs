using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

enum SquadAction
{
    Wandering,
    Following,
    Attacking
}

/// <summary>Class <c>Squad</c> A Squad is a group of AI that will work and move together</summary>
/// Squads will determine how aggressive they are based on their own strength compared to the player's

public abstract class Squad
{

    [SerializeField]
    internal List<Ai> squadMembers = new List<Ai>();
    internal GameObject target;
    internal Vector3 movementLoc; //this is the location squad members will try to surround
    internal Vector3 squadCenter;
    internal SquadManager manager;
    internal SquadAction currentAction = SquadAction.Following;

    public Squad(SquadManager _manager)
    {
        this.manager = _manager;

        GameStateController.playerDead.notifyListenersEnter += HandleDeadEnter;
        GameStateController.playerDead.notifyListenersExit += HandleDeadExit;
    }

    private void HandleDeadEnter() 
    {
        currentAction = SquadAction.Wandering;

        target = null;
    }
    private void HandleDeadExit() 
    {
        currentAction = SquadAction.Attacking;
    }

    // Update is called once per frame
    internal void Update()
    {
        AdjustSquadCenter();

        HandleAction();

        HandleLife();

        HandleMovement();

        Debug.DrawLine(squadCenter, movementLoc, currentAction==SquadAction.Attacking?Color.yellow:Color.cyan);
    }

    //Updates the state of the squad based on the action
    internal abstract void HandleAction();

    //removes dead AI and shatters squad if too small
    private void HandleLife()
    {
        foreach (Ai ai in squadMembers)
        {
            Debug.DrawLine(ai.transform.position, squadCenter, Color.red);
            //handle AI death
            if (!ai.IsAlive())
            {
                squadMembers.Remove(ai);
                manager.currentEnemies.Remove(ai);
                manager.scoreKeeper.AddToScore((int)ai.GetScore());
                DLevel.Instance.IncreaseDangerLevel((int)ai.dlScore);
                break;
            }

        }
    }
    //controls the movement of all the squad members
    internal abstract void HandleMovement();

    private void AdjustSquadCenter()
    {
        squadCenter = Vector3.zero;
        foreach (Ai ai in squadMembers)
        {
            squadCenter.x += ai.transform.position.x;
            squadCenter.z += ai.transform.position.z;
        }
        squadCenter.x /= squadMembers.Count;
        squadCenter.z /= squadMembers.Count;
    }

    internal float GetHitPoints(GameObject target)
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

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
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

    internal bool HasMembers()
    {
        return squadMembers.Count > 0;
    }
}
