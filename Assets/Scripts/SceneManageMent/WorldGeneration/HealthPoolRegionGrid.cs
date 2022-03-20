using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoolRegionGrid : IWorldGenerationObject
{
    public HealthPool poolPrefab;

    HealthPoolRegion hp;

    public override void Init()
    {
        int numPoolsToSpawn = 1;
        List<HealthPool> poolMeshes = new List<HealthPool>();
        for (int i = 0; i < numPoolsToSpawn; i++) 
        {
            HealthPool poolObject = GameObject.Instantiate(poolPrefab);
            poolObject.Init();
            poolMeshes.Add(poolObject);
        }
        hp = new HealthPoolRegion(poolMeshes, WorldBounds.currentTileHorizontalMinMax.x, WorldBounds.currentTileHorizontalMinMax.y, WorldBounds.currentTileVericalMinMax.x, WorldBounds.currentTileVericalMinMax.y);
    }

    public override void WorldUpdated()
    {
        throw new System.NotImplementedException();
    }
}
