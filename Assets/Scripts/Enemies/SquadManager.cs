using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public SquadSpawner squadSpawner; //Reference to Script in charge of spawning Enemies    
    public DLevel dl; //Danger Level Timer 
    public ScoreTracker scoreKeeper;

    public List<Ai> currentEnemies; //This is a list of Ai that are currently active in the scene. 

    public List<Squad> squads = new List<Squad>();

    internal void DisbandSquad(Squad squad)
    {
        squads.Remove(squad);
        if(squads.Count > 0 )
        {
            Squad closest = null;
            foreach(Squad s  in squads ) 
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach(Squad squad in squads )
            {
                squad.SetAction(SquadAction.Following);
            }
        }
        else if(Input.GetKeyDown(KeyCode.K)) 
        {
            foreach (Squad squad in squads)
            {
                squad.SetAction(SquadAction.Attacking);
            }
        }
        foreach (Squad s in squads) 
        {
            s.Update();
        }
        while (squads.Count - 1 <= dl.dangerLevel / 5)
        {
            squads.Add(squadSpawner.SpawnSquad());
        }
    }
}
