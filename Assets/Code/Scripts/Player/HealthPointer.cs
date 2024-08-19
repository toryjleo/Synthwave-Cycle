using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates to point toward the healthpool. Used as an indicator of where the healthpool is. 
/// Assumes 1 healthpool in world.
/// </summary>
public class HealthPointer : MonoBehaviour
{
    private HealthPool pool;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(HealthPoolInWorld());   
    }

    // Update is called once per frame
    void Update()
    {
        PointAtPool();
    }

    /// <summary>
    /// Checks to see if the healthpool is in the world.
    /// </summary>
    /// <returns>If the pointer should be enabled</returns>
    private bool HealthPoolInWorld()
    {
        pool = FindObjectOfType<HealthPool>();
        if (pool == null)
        {
            // Disable this object
            return false;
        }
        return true;
    }

    /// <summary>
    /// Rotates this gameobject to point toward the healthpool in the world.
    /// </summary>
    private void PointAtPool() 
    {
        if (pool != null)
        {
            Vector3 positionToLook = new Vector3(pool.transform.position.x, transform.position.y, pool.transform.position.z);
            transform.LookAt(positionToLook);
        }
    }
}
