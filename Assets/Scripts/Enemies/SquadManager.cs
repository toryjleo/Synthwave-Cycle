using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>SquadManager</c> The SquadManager holds a list of every squad and handles their interaction</summary>
/// SquadManager also spawns in new squads according to the danger level and will handle breaking up a squad into individual units when it shatters

public class SquadManager : MonoBehaviour
{
    public SquadSpawner squadSpawner; //Reference to Script in charge of spawning Enemies    
    public DLevel dl; //Danger Level Timer 
    public ScoreTracker scoreKeeper;

    public List<Ai> currentEnemies; //This is a list of Ai that are currently active in the scene. 

    public IList<InfantrySquad> infantrySquads = new List<InfantrySquad>();
    public IList<VehicleSquad> vehicleSquads = new List<VehicleSquad>();

    //Shatters a squad and sends the surviving members to join a new squad
    internal void DisbandSquad(Squad squad)
    {
        List<Squad> squadsToCheck;
        if (squad is InfantrySquad)
        {
            squadsToCheck = infantrySquads as List<Squad>;
        }
        else
        {
            squadsToCheck = vehicleSquads as List<Squad>;
        }
        squadsToCheck.Remove(squad);
        if(squadsToCheck.Count > 0 )
        {
            Squad closest = null;
            foreach(Squad s  in squadsToCheck) 
            {
                if(closest == null || (s.GetCenter() - squad.GetCenter()).sqrMagnitude < (closest.GetCenter() - squad.GetCenter()).sqrMagnitude)
                {
                    closest = s;
                }
            }
            foreach(Ai ai in squad.GetSquadMembers())
            {
                closest.AddToSquad(ai);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dl = DLevel.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (InfantrySquad s in infantrySquads) 
        {
            s.Update();
        }
        foreach (VehicleSquad s in vehicleSquads)
        {
            s.Update();
        }
        //TODO: make this smarter, perhaps use the Wave.cs class
        //Currently makes sure a few squads always exist based on danger level
        //33% vehicle squad chance and 66% infantry (rifleman) squad chance
        while (vehicleSquads.Count + infantrySquads.Count - 1 <= dl.dangerLevel / 5)
        {
            switch (UnityEngine.Random.Range(0, 2))
            {
                case 0:
                    vehicleSquads.Add(squadSpawner.SpawnVehicleSquad());
                    break;
                case 1:
                case 2:
                    infantrySquads.Add(squadSpawner.SpawnInfantrySquad());
                    break;
            }
        }
    }
}
