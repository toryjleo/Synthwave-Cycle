using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void NotifyReadyToDespawn(Bullet bulletToDespawn);  // delegate

#region BASE_CLASS_CODE

#endregion

public class Bullet : MonoBehaviour
{
    private const float DESPAWN_DIST_FROM_PLAYER = 100;

    private Vector3 shootDir;

    // Specific to gun
    public float muzzleVelocity = 60;
    private Vector3 initialVelocity;
    private float mass = 2f;

#region BASE_CLASS_CODE
    private Vector3 playerPosition; // Need to include in a base class
    public void bl_PlayerPositionUpdated(Vector3 currentPlayerPosition)
    {
        this.playerPosition = currentPlayerPosition;
    }
#endregion


    public float Mass
    {
        get => mass;
    }

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

        Vector3 distanceThisFrame = ((shootDir.normalized * muzzleVelocity) + initialVelocity) * Time.deltaTime;
        transform.position = transform.position + distanceThisFrame;
    }


    public void Shoot(Vector3 curPosition, Vector3 direction, Vector3 initialVelocity) 
    {
        transform.position = curPosition;
        shootDir = direction;
        transform.rotation = Quaternion.LookRotation(direction);
        this.initialVelocity = initialVelocity;
    }

    protected virtual void OnBulletDespawn() //protected virtual method
    {
        //if BulletDespawn is not null then call delegate
        BulletDespawn?.Invoke(this);
    }



}
