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
        spotlight.intensity = 20;
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
        spotlight.spotAngle = 50;
        light.intensity = .8f;
    }

    internal void SetHPDistance(float distanceToHP)
    {
        int minA = 30;
        int maxA = 100;
        float useAngle = 2000/distanceToHP;


        if (useAngle < minA)
        {
            spotlight.spotAngle = minA;
        } else if(useAngle > maxA) {
            spotlight.spotAngle = maxA; 
        } else
        {
            spotlight.spotAngle = useAngle;
        }

        


    }
}
