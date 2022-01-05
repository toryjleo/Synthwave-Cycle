using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private Bullet bulletPrefab;
    private int bulletStartAmnt;

    private Queue<Bullet> bulletQueue;

    public void Init(Bullet bulletPrefab) 
    {
        this.bulletPrefab = bulletPrefab;

        bulletStartAmnt = 100;
        bulletQueue = new Queue<Bullet>();

        // Spawn in defualt bullets
        for (int i = 0; i < bulletStartAmnt; i++) 
        {
            Bullet newBullet = CreateNewBullet();
            bulletQueue.Enqueue(newBullet);
        }
    }

    private Bullet CreateNewBullet() 
    {
        Bullet newObject = Instantiate(bulletPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newObject.Init();
        newObject.gameObject.SetActive(false);
        newObject.Despawn += bl_ProcessCompleted;
        return newObject;
    }

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
            Debug.Log("Spawned, Size of queue: " + bulletQueue.Count);
            // Check if object already in world

            objectToSpawn.gameObject.SetActive(true);
            // Add the object to the end of the queue

            return objectToSpawn;
        }
    }

    public void bl_ProcessCompleted(SelfDespawn bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletQueue.Enqueue(bullet as Bullet); // Make sure this is bullet in the future
        Debug.Log("Despawned, Size of queue: " + bulletQueue.Count);
    }
}
