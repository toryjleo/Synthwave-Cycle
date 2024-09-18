using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPoolable : SelfWorldBoundsDespawn
{
    public abstract void Init(EditorObject.GunStats gunStats);

    // TODO: look into Unity reset()
    public abstract void Reset();
}


/// <summary>Class <c>ObjectPool</c> A Unity Component works as an object pool for bullets.</summary>
public class ObjectPool : MonoBehaviour, IResettable
{
    private const int INFINITE_AMMO_COUNT = 200;

    private EditorObject.GunStats gunStats;
    private IPoolable bulletPrefab;
    private int bulletStartAmnt = 0;

    private Queue<IPoolable> bulletQueue;
    private ArrayList usedBullets;

    /// <summary>
    /// Initialize this class's variables. A replacement for a constructor.
    /// </summary>
    /// <param name="gunStats">Gun statistics to query data from</param>
    /// <param name="bulletPrefab">The template object for this pool.</param>
    public void Init(EditorObject.GunStats gunStats, IPoolable bulletPrefab) 
    {
        if (gunStats.InfiniteAmmo) 
        {
            bulletStartAmnt = INFINITE_AMMO_COUNT * gunStats.ProjectileCountPerShot;
        }
        else 
        {
            bulletStartAmnt = gunStats.AmmoCount * gunStats.ProjectileCountPerShot;
        }

        this.gunStats = gunStats;
        this.bulletPrefab = bulletPrefab;

        if (bulletQueue == null)
        {
            bulletQueue = new Queue<IPoolable>();
        }
        if (usedBullets == null) 
        {
            usedBullets = new ArrayList();
        }
        while (bulletQueue.Count < bulletStartAmnt)
        {
            IPoolable newBullet = CreateNewBullet();
            bulletQueue.Enqueue(newBullet);
        }
    }

    /// <summary>Should handle all initialization for a new bullet instance.</summary>
    /// <returns>A new gameObject created from bulletPrefab.</returns>
    private IPoolable CreateNewBullet()
    {
        IPoolable newObject = Instantiate(bulletPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newObject.Init(gunStats);
        newObject.gameObject.SetActive(false);
        newObject.Despawn += DespawnBullet;
        return newObject;
    }

    /// <summary>Returns an instance of a bullet from the pool if there is an unused bullet.</summary>
    /// <returns>A currently unused bullet.</returns>
    public IPoolable SpawnFromPool() 
    {
        if (bulletQueue.Count == 0)
        {
            IPoolable newBullet = CreateNewBullet();
            bulletQueue.Enqueue(newBullet);
        }


        IPoolable objectToSpawn = bulletQueue.Dequeue();
        usedBullets.Add(objectToSpawn);

        // Check if object already in world
        objectToSpawn.Reset();
        objectToSpawn.gameObject.SetActive(true);
        // Add the object to the end of the queue

        return objectToSpawn;

    }

    /// <summary>Handles newObject.Despawn. Sets bullet to inactive and adds it to queue.</summary>
    /// <param name="bullet">The object which called Despawn that passes itself into the Despawn method that needs to 
    /// be added back to the pool.</param>
    public void DespawnBullet(SelfDespawn bullet)
    {
        bullet.gameObject.SetActive(false);
        usedBullets.Remove((bullet as IPoolable));
        bulletQueue.Enqueue(bullet as IPoolable);
    }

    public void ResetGameObject()
    {
        for (int i = 0; i < usedBullets.Count; i++) 
        {
            IPoolable b = (IPoolable) usedBullets[i];
            b.Reset();
            usedBullets.Remove(b);
            bulletQueue.Enqueue(b);
        }
    }
}
