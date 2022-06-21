using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VulkanV64Trail : Bullet
{
    private const float SHOT_RANGE = 100f;
    public TrailRenderer BulletTrail;
    public override void Init()
    {
        muzzleVelocity = 180;
        mass = .1f; //The Mass controlls how slowed down the bike is by recoil
        damageDealt = 1000;
}


public override void Shoot(Vector3 curPosition, Vector3 direction, Vector3 initialVelocity)
    {
        //Debug.Log("VULKAN FIRE!");
        transform.position = curPosition;
        direction.y = 0; // Do not travel vertically
        shootDir = direction.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        this.initialVelocity = initialVelocity;
        this.timeSinceShot = 0;

        RaycastHit hit;
        Ray shotRay = new Ray(curPosition, shootDir);
        if (Physics.Raycast(shotRay, out hit, SHOT_RANGE))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                DealDamageAndDespawn(hit.collider.gameObject);
            }
        }
        TrailRenderer trail = Instantiate(BulletTrail, curPosition, Quaternion.identity);
        SpawnTrail(trail, shotRay.GetPoint(SHOT_RANGE));
    }

    /// <summary>Spawns in a trail from the position of the gun to an endpoint. 
    /// Trail rins through animation and deletes itself</summary>
    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 endLoc)
    {
        float time = 0;
        Vector3 startLoc = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startLoc, endLoc, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        Destroy(trail.gameObject, trail.time);
    }
}
