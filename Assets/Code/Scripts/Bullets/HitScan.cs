using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScan
{

    private EditorObject.GunStats gunStats = null;
    public HitScan(EditorObject.GunStats gunStats) 
    {
        this.gunStats = gunStats;
    }

    public void Shoot(Vector3 curPosition, Vector3 direction, Generic.ObjectPool impactEffectPool)
    {
        // Get hits
        RaycastHit[] hits;
        hits = Physics.RaycastAll(curPosition, direction, gunStats.Range);
        System.Array.Sort(hits, (a, b) => (a.distance.CompareTo(b.distance)));

        int n = Mathf.Min(gunStats.BulletPenetration + 1, hits.Length);

        for (int i = 0; i < n; i++)
        {
            RaycastHit hit = hits[i];
            Debug.Log(hit.transform.name);

            // TODO: Move to DealDamage
            if ((hit.transform.tag == "Enemy" && gunStats.IsPlayerGun) ||
                (hit.transform.tag == "Player" && !gunStats.IsPlayerGun))
            {
                DealDamage(hit.transform.gameObject);
            }


            PooledParticle particle = impactEffectPool.SpawnFromPool() as PooledParticle;
            particle.transform.position = hit.transform.position;
            particle.transform.rotation = Quaternion.LookRotation(hit.normal);
            particle.Play();
        }
    }

    /// <summary>
    /// Inflict gun damage on other
    /// </summary>
    /// <param name="other">Object with Health component</param>
    private void DealDamage(GameObject other)
    {
        
        Health otherHealth = other.GetComponentInChildren<Health>();
        if (otherHealth == null)
        {
            Debug.LogError("Object does not have Health component: " + other.name);
        }
        else
        {
            otherHealth.TakeDamage(gunStats.DamageDealt);

            // TODO: instantiate and play impact effect particlesystem at hit.point and rotate to normal
            // https://www.youtube.com/watch?v=THnivyG0Mvo
        }

    }
}
