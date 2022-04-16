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

    // Start is called before the first frame update

    //public List<BikeLight> Lights;

    /*[System.Serializable]
    public class BikeLight
    {
        public GameObject Light;
        public Color color;

        public BikeLight(Color c, GameObject g)
        {
            c = color;
            g = Light;
        }

    }*/


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

    private void SetEmissiveColor(Color color)
    {
        emissiveMaterial.SetColor("_EmissionColor", color);
    }

    private void SetAlbedoColor(Color color)
    {
        emissiveMaterial.SetColor("_Color", color);
    }

    public void SetDeadAheadColor() 
    {
        SetEmissiveColor(deadAheadColor);
        SetAlbedoColor(deadAheadColor);
        light.color = deadAheadColor;
        spotlight.color = deadAheadColor;
        spotlight.intensity = 20;
        light.intensity = 1.3f;
    }

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
