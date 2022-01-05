using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Bullet : SelfDespawn
{

    private Vector3 shootDir;
    private Vector3 initialVelocity;

    // Specific to gun
    protected float muzzleVelocity = 0;
    protected float mass = 0;


    public float MuzzleVelocity
    {
        get => muzzleVelocity;
    }


    public float Mass
    {
        get => mass;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Vector3 distanceThisFrame = ((shootDir * muzzleVelocity) + initialVelocity) * Time.deltaTime;
        transform.position = transform.position + distanceThisFrame;
    }


    public void Shoot(Vector3 curPosition, Vector3 direction, Vector3 initialVelocity) 
    {
        transform.position = curPosition;
        shootDir = direction.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        this.initialVelocity = initialVelocity;
    }

}
