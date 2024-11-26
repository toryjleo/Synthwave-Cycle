using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTester : MonoBehaviour
{
    Gun.Gun gun = null;

    [Range(0.01f, 20f)][SerializeField] private float secondsBetweenTriggerPull = .1f;

    private float secondsSinceLastTriggerPull = 0;

    // Update is called once per frame
    void Update()
    {
        gun = FindObjectOfType<Gun.Gun>();

        UpdateTriggerPull(Time.deltaTime);
    }

    private void UpdateTriggerPull(float deltaTime) 
    {
        secondsSinceLastTriggerPull += deltaTime;
        if (secondsSinceLastTriggerPull > secondsBetweenTriggerPull) 
        {
            gun.ExternalFire = true;
            secondsSinceLastTriggerPull = 0;
        }
    }


}
