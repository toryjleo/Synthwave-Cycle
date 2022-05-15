using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmissiveBikeScript : MonoBehaviour
{

    [SerializeField] private Material emissiveMaterial;
    [SerializeField] private Renderer objectToChange;
    public Light light;
    public Light spotlight;


    void Start()
    {
        light = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) 
        {
            emissiveMaterial.SetColor("_EmissionColor1", Color.red);
        }
        
    }

    /// <summary>
    /// Sets the material colors for when the bike is NOT pointing at a healthpool.
    /// </summary>
    public void SetNotAheadColor()
    {
        spotlight.intensity = 0;
        light.intensity = .8f;
        emissiveMaterial.SetFloat("_EmissionSlider", 0);
    }

    /// <summary>
    /// Updates the spotlight to reflect "closing in" on the healthpool
    /// </summary>
    /// <param name="distanceToHP">The current player distance to the healthpool</param>
    /// <param name="consecutiveDistanceToHP">The distance to the heathpool, updated whenever the player points back at
    /// the healthpool.</param>
    internal void SetHPDistance(float distanceToHP, float consecutiveDistanceToHP)
    {
        float t = PercentToHP(distanceToHP, consecutiveDistanceToHP);
        float minA = 30;
        float maxA = 80;
        float useAngle = Mathf.Lerp(maxA, minA, t);

        light.intensity = 1.3f;
        emissiveMaterial.SetFloat("_EmissionSlider", (t + .1f));

        spotlight.intensity = 5 * (t + .1f);

        if (useAngle < minA)
        {
            spotlight.spotAngle = minA;
        } 
        else if (useAngle > maxA) 
        {
            spotlight.spotAngle = maxA; 
        } else
        {
            spotlight.spotAngle = useAngle;
        }

    }

    /// <summary>
    /// Returns the percentage of the way the player is between the healthpool and from where they started looking at
    /// the healthpool
    /// </summary>
    /// <param name="distanceToHP">The current player distance to the healthpool</param>
    /// <param name="consecutiveDistanceToHP">The distance to the heathpool, updated whenever the player points back at
    /// the healthpool.</param>
    /// <returns>A float between 0 and 1</returns>
    private float PercentToHP(float distanceToHP, float consecutiveDistanceToHP) 
    {
        return 1.0f - (distanceToHP / consecutiveDistanceToHP);
    }
}
