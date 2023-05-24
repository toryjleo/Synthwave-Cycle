using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAI : Ai
{
    ArcadeAiVehicleController vehicleController;

    //A vehicle currently does not attack, it only intercepts
    public override void Attack() { }

    public override void Init()
    {
        vehicleController = GetComponent<ArcadeAiVehicleController>();
    }

    public override void SetTarget(GameObject targ)
    {
        //vehicleController.enabled = true;
        vehicleController.target = targ.transform;
        base.SetTarget(targ);
    }
}
