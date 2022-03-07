using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>PlayerHealthRegen</c> A Unity Component which will update the player health along with how the player regains health.</summary>
/// Needs to be manually assigned the health in editor
public class PlayerHealthRegen : MonoBehaviour
{
    // Energy
    public Health health;
    private const float STARTING_HEALTH = 300.0f;
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
        health.Init(STARTING_HEALTH);
        SetScale(MAX_SCALE);
    }

    /// <summary>Returns the full length the player's regen can grow or shrink</summary>
    /// <returns>The difference of MAX_SCALE - MIN_SCALE.</returns>
    private float ScaleMINMAXDifference()
    {
        return MAX_SCALE - MIN_SCALE;
    }

    /// <summary>Returns the percentage of the regen. Will be at max when curScale == MAX_SCALE</summary>
    /// <returns>The percentage of the regen.</returns>
    private float PercentRegen()
    {
        return (curScale - MIN_SCALE) / ScaleMINMAXDifference();
    }

    /// <summary>Regenerates the bike's energy by a specified amount. Should be called every frame</summary>
    public void UpdateRegenEnergy()
    {
        float defaultRegnAmount = Time.deltaTime * energyRegenPerSec;
        float additionalRegenAmount = Time.deltaTime * energyRegenPerSec * PercentRegen() * ADDITIONAL_REGEN_MULTIPLIER;
        Regen(defaultRegnAmount + additionalRegenAmount);
    }

    /// <summary>Heals the bike by regenAmount.</summary>
    /// <param name="regenAmount">The amount of points the bike will heal for</param>
    public void Regen(float regenAmount)
    {
        health.Heal(regenAmount);
    }

    /// <summary>Shrinks the regen scale by amnt.</summary>
    /// <param name="amnt">The amount of points to reduce curScale by.</param>
    private void Shrink(float amnt)
    {
        SetScale(curScale - amnt);
    }

    /// <summary>Grows the regen scale by amnt.</summary>
    /// <param name="amnt">The amount of points to increase curScale by.</param>
    private void Grow(float amnt)
    {
        SetScale(curScale + amnt);
    }

    /// <summary>Clamps amnt between MIN_SCALE and MAX_SCALE.</summary>
    /// <param name="amnt">The unit to be clamped.</param>
    /// <returns>The clamped amnt.</returns>
    private float ClampScale(float amnt)
    {
        return Mathf.Clamp(amnt, MIN_SCALE, MAX_SCALE);
    }

    /// <summary>Updates the current scale.</summary>
    /// <param name="scale">The scale at which to set this object to.</param>
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
