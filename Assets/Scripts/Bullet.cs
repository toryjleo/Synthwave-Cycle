using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : SelfDespawn
{

    private Vector3 shootDir;

    // Specific to gun
    public float muzzleVelocity = 180;
    private Vector3 initialVelocity;
    private float mass = 2f;


    public float Mass
    {
        get => mass;
    }


    // Start is called before the first frame update
    void Start()
    {
        
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
