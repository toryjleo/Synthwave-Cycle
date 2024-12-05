using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTester : MonoBehaviour
{
    [SerializeField] private Gun.Gun gunToTest = null;

    [Range(0.01f, 20f)][SerializeField] private float secondsBetweenTriggerPull = .1f;

    private float secondsSinceLastTriggerPull = 0;

    // Update is called once per frame
    void Update()
    {
        UpdateTriggerPull(Time.deltaTime);
    }

    private void UpdateTriggerPull(float deltaTime) 
    {
        if (gunToTest != null) 
        {
            gunToTest.gameObject.SetActive(true);
            secondsSinceLastTriggerPull += deltaTime;
            if (secondsSinceLastTriggerPull > secondsBetweenTriggerPull)
            {
                gunToTest.ExternalFire = true;
                secondsSinceLastTriggerPull = 0;
            }
        }
    }


}
