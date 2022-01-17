using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthRegen : MonoBehaviour
{
    // Energy
    private Health energy;
    private const float STARTING_ENERGY = 300.0f;
    private float energyRegenPerSec;
    private float STARTING_REGEN_PER_SEC = 5;
    // Start is called before the first frame update
    void Awake()
    {
        energyRegenPerSec = STARTING_REGEN_PER_SEC;
        energy = GetComponentInChildren<Health>();
        energy.Init(STARTING_ENERGY);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRegenEnergy();
    }

    /// <summary>Regenerates the bike's energy by a specified amount. Should be called every frame</summary>
    public void UpdateRegenEnergy()
    {
        float regenAmount = Time.deltaTime * energyRegenPerSec;
        Regen(regenAmount);
    }

    /// <summary>Heals the bike by regenAmount.</summary>
    /// <param name="regenAmount">The amount of points the bike will heal for</param>
    public void Regen(float regenAmount)
    {
        energy.Heal(regenAmount);
    }
}
