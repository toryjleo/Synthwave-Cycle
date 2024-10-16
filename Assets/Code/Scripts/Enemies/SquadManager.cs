using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Class <c>SquadManager</c> The SquadManager holds a list of every squad and handles their interaction</summary>
/// SquadManager also spawns in new squads according to the danger level and will handle breaking up a squad into individual units when it shatters
public class SquadManager : MonoBehaviour, IResettable
{
    public List<Ai> currentEnemies; //This is a list of Ai that are currently active in the scene. 

    public List<Squad> squads = new List<Squad>();

    public void ResetGameObject()
    {
        foreach (Ai ai in currentEnemies)
        {
            ai.ResetGameObject();
        }
        currentEnemies = new List<Ai>();
        squads = new List<Squad>();
    }

    /// <summary>
    /// Called by squad spawner when enemies are spawned, keeping track of a list of squads?
    /// </summary>
    /// <param name="squad"></param>
    internal void RegisterSquad(Squad squad)
    {
        squads.Add(squad);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = squads.Count - 1; i >= 0; i--)
        {
            if (squads[i].HasMembers())
            {
                //  squads[i].Update();
            }
            else
            {
                squads.RemoveAt(i);
            }
        }
    }
}
