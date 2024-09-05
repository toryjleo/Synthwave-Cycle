using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BulletPool</c> A Unity Component works as an object pool for bullets.</summary>
public class BulletPool : MonoBehaviour, IResettable
{
    private EditorObject.GunStats gunStats;
    private Bullet bulletPrefab;
    private int bulletStartAmnt = 0;

    private Queue<Bullet> bulletQueue;
    private ArrayList usedBullets;

    /// <summary>Initialize this class's variables. A replacement for a constructor.</summary>
    /// <param name="bulletPrefab">The template object for this pool.</param>
    public void Init(EditorObject.GunStats gunStats, Bullet bulletPrefab, int bulletPoolSize = 100) 
    {
        bulletStartAmnt = bulletPoolSize;

        this.gunStats = gunStats;
        this.bulletPrefab = bulletPrefab;

        if (bulletQueue == null)
        {
            bulletQueue = new Queue<Bullet>();
        }
        if (usedBullets == null) 
        {
            usedBullets = new ArrayList();
        }
        while (bulletQueue.Count < bulletStartAmnt)
        {
            Bullet newBullet = CreateNewBullet();
            bulletQueue.Enqueue(newBullet);
        }
    }

    /// <summary>Should handle all initialization for a new bullet instance.</summary>
    /// <returns>A new gameObject created from bulletPrefab.</returns>
    private Bullet CreateNewBullet()
    {
        Bullet newObject = Instantiate(bulletPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newObject.Init(gunStats);
        newObject.gameObject.SetActive(false);
        newObject.Despawn += DespawnBullet;
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
            usedBullets.Add(objectToSpawn);

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
    public void DespawnBullet(SelfDespawn bullet)
    {
        bullet.gameObject.SetActive(false);
        usedBullets.Remove((bullet as Bullet));
        bulletQueue.Enqueue(bullet as Bullet); // Make sure this is bullet in the future
    }

    public void ResetGameObject()
    {
        for (int i = 0; i < usedBullets.Count; i++) 
        {
            Bullet b = (Bullet) usedBullets[i];
            b.ResetBullet();
            usedBullets.Remove(b);
            bulletQueue.Enqueue(b);
        }
    }
}
