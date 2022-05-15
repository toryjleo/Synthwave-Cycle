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


    Color notAheadColor;
    float notAheadColorIntensity = .01f;

    Color deadAheadColor;
    float deadAheadColorIntensity = .01f;

    void Start()
    {
        deadAheadColor = new Color(25, 214, 162) * deadAheadColorIntensity;
        notAheadColor = new Color(191, 175, 7) * notAheadColorIntensity;
        light = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) 
        {
            emissiveMaterial.SetColor("_EmissionColor", Color.red);
        }
        
    }

    /// <summary>
    /// Sets the emissive color of the material
    /// </summary>
    /// <param name="color">Color to set the emissive property to.</param>
    private void SetEmissiveColor(Color color)
    {
        emissiveMaterial.SetColor("_EmissionColor", color);
    }

    /// <summary>
    /// Sets the albedo color of the materail
    /// </summary>
    /// <param name="color">Color to set the albedo to.</param>
    private void SetAlbedoColor(Color color)
    {
        emissiveMaterial.SetColor("_Color", color);
    }

    /// <summary>
    /// Sets the material colors for when the bike is pointing at a healthpool.
    /// </summary>
    public void SetDeadAheadColor() 
    {
        SetEmissiveColor(deadAheadColor);
        SetAlbedoColor(deadAheadColor);
        light.color = deadAheadColor;
        spotlight.color = deadAheadColor;
        light.intensity = 1.3f;
    }

    /// <summary>
    /// Sets the material colors for when the bike is NOT pointing at a healthpool.
    /// </summary>
    public void SetNotAheadColor()
    {
        SetEmissiveColor(notAheadColor);
        SetAlbedoColor(notAheadColor);
        light.color = notAheadColor;
        spotlight.color = notAheadColor;
        spotlight.intensity = 0;
        light.intensity = .8f;
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
