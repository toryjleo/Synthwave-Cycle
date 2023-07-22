using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>Class<c>VehicleSquad</c> 
/// Holds the information for a squad of vehicles, 
/// Handles Vehicle's strategy (different from Infantry Squads)
public class VehicleSquad : Squad
{
    public VehicleSquad(SquadManager _manager) : base(_manager) { }

    internal override void HandleAction()
    {
        switch (currentAction)
        {
            //try to move in between the current position and the target
            case SquadAction.Following:
                movementLoc = (squadCenter + target.transform.position) / 2;
             //   if (IsConfident())
                {
                    currentAction = SquadAction.Attacking;
                }
                break;
            //rush the target
            case SquadAction.Attacking:
              //  if (!IsConfident())
                {
                    currentAction = SquadAction.Following;
                }
                movementLoc = target.transform.position;
                break;
        }
    }

    internal override void HandleMovement()
    {
        BikeScript bikeScript = target.GetComponent<BikeScript>();
        foreach (Ai ai in squadMembers)
        {
            float distanceToTrackerFR = (ai.transform.position - bikeScript.movementComponent.trackerFR.transform.position).sqrMagnitude;
            float distanceToTrackerFL = (ai.transform.position - bikeScript.movementComponent.trackerFL.transform.position).sqrMagnitude;
            if(distanceToTrackerFR < distanceToTrackerFL) 
            {
                ai.SetTarget(bikeScript.movementComponent.trackerFR);
            }
            else
            {
                ai.SetTarget(bikeScript.movementComponent.trackerFL);
            }
        }
    }
}

