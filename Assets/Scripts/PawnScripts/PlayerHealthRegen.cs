using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthRegen : MonoBehaviour
{
    // Energy
    public Health health;
    private const float STARTING_ENERGY = 300.0f;
    private float energyRegenPerSec;
    private float STARTING_REGEN_PER_SEC = 3;
    private float ADDITIONAL_REGEN_MULTIPLIER = 4;


    // Visuals
    private const float MIN_SCALE = 5;
    private const float MAX_SCALE = 30.0f;
    private const float SCALE_SHRINK_PER_SECOND = 2f;
    private const float SCALE_GROW_PER_SECOND = 1f;

    private int numEnemiesInVolume;

    private float curScale;

    void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRegenEnergy();

        //print("Number ofenemies in volume: " + numEnemiesInVolume);

        // Manage PlayerHealthRegen object
        if (numEnemiesInVolume > 0) 
        {
            Shrink(SCALE_SHRINK_PER_SECOND * Time.deltaTime * numEnemiesInVolume);
        }
        else if (curScale < MAX_SCALE)
        {
            Grow(SCALE_GROW_PER_SECOND * Time.deltaTime);
        }
    }


    private void Init() 
    {
        numEnemiesInVolume = 0;
        energyRegenPerSec = STARTING_REGEN_PER_SEC;
        health.Init(STARTING_ENERGY);
        SetScale(MAX_SCALE);
    }

    private float ScaleMINMAXDifference() 
    {
        return MAX_SCALE - MIN_SCALE;
    }

    private float PercentRegen() 
    {
        return (curScale - MIN_SCALE) / ScaleMINMAXDifference();
    }

    /// <summary>Regenerates the bike's energy by a specified amount. Should be called every frame</summary>
    public void UpdateRegenEnergy()
    {
        float defaultRegnAmount = Time.deltaTime * energyRegenPerSec;
        float additionalRegenAmount = Time.deltaTime * energyRegenPerSec * PercentRegen() * ADDITIONAL_REGEN_MULTIPLIER;
        print("Regen: " + (defaultRegnAmount + additionalRegenAmount) / Time.deltaTime); ;
        Regen(defaultRegnAmount + additionalRegenAmount);
    }

    /// <summary>Heals the bike by regenAmount.</summary>
    /// <param name="regenAmount">The amount of points the bike will heal for</param>
    public void Regen(float regenAmount)
    {
        health.Heal(regenAmount);
    }

    private void Shrink(float amnt) 
    {
        SetScale(curScale - amnt);
    }

    private void Grow(float amnt)
    {
        SetScale(curScale + amnt);
    }

    private float ClampScale(float amnt)
    {
        return Mathf.Clamp(amnt, MIN_SCALE, MAX_SCALE);
    }

    private void SetScale(float scale) 
    {
        curScale = ClampScale(scale);
        Vector3 objScale = new Vector3(curScale, 1, curScale);

        transform.localScale = objScale;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") 
        {
            numEnemiesInVolume++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            numEnemiesInVolume--;
        }
    }
}
