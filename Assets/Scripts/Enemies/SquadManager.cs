using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Class <c>SquadManager</c> The SquadManager holds a list of every squad and handles their interaction</summary>
/// SquadManager also spawns in new squads according to the danger level and will handle breaking up a squad into individual units when it shatters

public class SquadManager : MonoBehaviour, IResettable
{
    public SquadSpawner squadSpawner; //Reference to Script in charge of spawning Enemies    
    public DangerLevel dl; //Danger Level Timer

    public List<Ai> currentEnemies; //This is a list of Ai that are currently active in the scene. 

    public List<Squad> squads = new List<Squad>();

    public void ResetGameObject()
    {
        foreach(Ai ai in currentEnemies) 
        {
            ai.ResetGameObject();
        }
        currentEnemies = new List<Ai>();
        squads = new List<Squad>();
    }

    //Shatters a squad and sends the surviving members to join a new squad
    internal void DisbandInfantrySquad(InfantrySquad squad)
    {
        if(squads.Count > 1) //last squad will not break
        {
            squads.Remove(squad);
            Squad closest = null;
            foreach(Squad s in squads.Where(s => s is InfantrySquad)) 
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

    internal void RegisterSquad(Squad squad)
    {
        squads.Add(squad);
    }


    // Start is called before the first frame update
    void Start()
    {
        dl = DangerLevel.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = squads.Count-1; i >= 0; i--)
        {
            if (squads[i].HasMembers())
            {
                squads[i].Update();
            }
            else
            {
                squads.RemoveAt(i);
            }    
        }
    }
}
