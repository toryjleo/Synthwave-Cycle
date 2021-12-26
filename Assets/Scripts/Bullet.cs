using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void NotifyReadyToDespawn(Bullet bulletToDespawn);  // delegate

public class Bullet : MonoBehaviour
{
    private const float DESPAWN_DIST_FROM_PLAYER = 100;

    private Vector3 shootDir;

    // Specific to gun
    public float muzzleVelocity = 60;
    public float mass = 100f;


    public event NotifyReadyToDespawn BulletDespawn; // event

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Assumes player is at world origin
        if (transform.position.sqrMagnitude > DESPAWN_DIST_FROM_PLAYER * DESPAWN_DIST_FROM_PLAYER) 
        {
            // Return to object pool
            OnBulletDespawn();
        }

        Vector3 distanceThisFrame = shootDir.normalized * muzzleVelocity * Time.deltaTime;
        transform.position = transform.position + distanceThisFrame;
    }


    public void Shoot(Vector3 curPosition, Vector3 direction) 
    {
        transform.position = curPosition;
        shootDir = direction;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    protected virtual void OnBulletDespawn() //protected virtual method
    {
        //if BulletDespawn is not null then call delegate
        BulletDespawn?.Invoke(this);
    }

}
