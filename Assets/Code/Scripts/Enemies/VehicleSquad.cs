using System.Diagnostics;
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
        foreach (VehicleAi ai in squadMembers)
        {
            Vector3 aimLoc = target.transform.position;
            aimLoc += target.transform.forward * 10;

            //ai.myGun?.transform.LookAt(aimLoc);

            if( Vector3.Distance(ai.transform.position, target.transform.position) <= ai.attackRange)
            {
                ai.Attack();
            }
        }
    }
}

