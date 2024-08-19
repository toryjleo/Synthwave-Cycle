using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerHealth;

/// <summary>
/// Weapon that triggers a screen-wipe for player
/// </summary>
public class PinkMist : Weapon
{
    float PerSecondGrowth_Meters = 200f;
    float MaxSize_Meters = 100;
    float StartSize_Meters = 1;

    public GameObject CollisionVolume;

    Coroutine runningCoroutine = null;

    public override PlayerWeaponType GetPlayerWeaponType()
    {
        return PlayerWeaponType.PinkMist;
    }

    public override void Init()
    {
        if(CollisionVolume  == null) 
        {
            Debug.LogError("PinkMist needs to be assigned a collision volume!");
        }
        else 
        { 
            CollisionVolume.SetActive(false); 
        }
    }

    public override void PrimaryFire(Vector3 initialVelocity)
    {

        if (CollisionVolume != null)
        {
            if (runningCoroutine != null) 
            {
                StopCoroutine(runningCoroutine);
            }
            
            runningCoroutine = StartCoroutine(GrowVolume(CollisionVolume));
        }
    }

    public override void ReleasePrimaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }

    public override void ReleaseSecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }

    public override void SecondaryFire(Vector3 initialVelocity)
    {
        throw new System.NotImplementedException();
    }

    public void HandleBarUpdate(BarMax oldMax, BarMax newMax, bool hpIsOverBarMax3) 
    {
        if (newMax > oldMax || hpIsOverBarMax3) 
        {
            PrimaryFire(Vector3.zero);
        }
    }

    private IEnumerator GrowVolume(GameObject volume) 
    {
        volume.SetActive(true);
        for (float i = StartSize_Meters; i <= MaxSize_Meters;)
        {
            transform.localScale = new Vector3(i, CollisionVolume.transform.localScale.y, i);
            i += (PerSecondGrowth_Meters * Time.deltaTime);
            yield return null;
        }
        volume.SetActive(false);
        runningCoroutine = null;
    }
}
