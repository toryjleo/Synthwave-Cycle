using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoolRegion
{
    struct PoolBounds
    {
        public Vector2 xMinMax;
        public Vector2 zMinMax;
    }

    //private int numPoolsToSpawn = 3;
    private PoolBounds poolBounds;

    List<HealthPool> poolObjects;

    public HealthPoolRegion(List<HealthPool> poolObjects, float xMin, float xMax, float zMin, float zMax) 
    {
        poolBounds.xMinMax = new Vector2(xMin, xMax);
        poolBounds.zMinMax = new Vector2(zMin, zMax);

        this.poolObjects = poolObjects;
        for (int i = 0; i < poolObjects.Count; i++) 
        {
            Vector3 newLocation = new Vector3(Random.Range(xMin, xMax), 0, Random.Range(zMin, zMax));
            poolObjects[i].transform.position = newLocation;
        }
    }

}
