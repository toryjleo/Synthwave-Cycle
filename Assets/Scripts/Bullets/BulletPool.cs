using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BulletPool</c> A Unity Component works as an object pool for bullets.</summary>
public class BulletPool : MonoBehaviour, IResettable
{
    private Bullet bulletPrefab;
    private int bulletStartAmnt = 0;

    private Queue<Bullet> bulletQueue;

    /// <summary>Initialize this class's variables. A replacement for a constructor.</summary>
    /// <param name="bulletPrefab">The template object for this pool.</param>
    public void Init(Bullet bulletPrefab, int bulletPoolSize = 100) 
    {
        bulletStartAmnt = bulletPoolSize;
        this.bulletPrefab = bulletPrefab;
        if (bulletQueue == null)
        {
            bulletQueue = new Queue<Bullet>();
        }
        while (bulletQueue.Count < bulletStartAmnt)
        {
            Bullet newBullet = CreateNewBullet();
            bulletQueue.Enqueue(newBullet);
        }
    }

    /// <summary>Basically a destructor. Clears the queue.</summary>
    public void DeInit()
    {
        while(bulletQueue.Count > 0) 
        {
            Bullet bullet = bulletQueue.Dequeue();
            bullet.Despawn -= bl_ProcessCompleted;
        }
        bulletStartAmnt = 0;
        bulletPrefab = null;
    }

    /// <summary>Should handle all initialization for a new bullet instance.</summary>
    /// <returns>A new gameObject created from bulletPrefab.</returns>
    private Bullet CreateNewBullet() 
    {
        Bullet newObject = Instantiate(bulletPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newObject.Init();
        newObject.gameObject.SetActive(false);
        newObject.Despawn += bl_ProcessCompleted;
        return newObject;
    }

    /// <summary>Returns an instance of a bullet from the pool if there is an unused bullet.</summary>
    /// <returns>A currently unused bullet.</returns>
    public Bullet SpawnFromPool() 
    {
        if (bulletQueue.Count == 0)
        {
            Debug.LogError("Trying to spawn object already in world!");
            return null;
        }
        else
        {

            Bullet objectToSpawn = bulletQueue.Dequeue();
            //Debug.Log("Spawned, Size of queue: " + bulletQueue.Count);
            // Check if object already in world
            objectToSpawn.ResetBullet();
            objectToSpawn.gameObject.SetActive(true);
            // Add the object to the end of the queue

            return objectToSpawn;
        }
    }

    /// <summary>Handles newObject.Despawn. Sets bullet to inactive and adds it to queue.</summary>
    /// <param name="bullet">The object which called Despawn that passes itself into the Despawn method that needs to 
    /// be added back to the pool.</param>
    public void bl_ProcessCompleted(SelfDespawn bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletQueue.Enqueue(bullet as Bullet); // Make sure this is bullet in the future
        //Debug.Log("Despawned, Size of queue: " + bulletQueue.Count);
    }

    public void ResetGameObject()
    {
        Init(bulletPrefab, bulletStartAmnt);
    }
}
