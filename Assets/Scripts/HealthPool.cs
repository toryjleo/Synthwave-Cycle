using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : SelfDespawn
{
    // Visuals
    private const float DEFAULT_MIN_SCALE = 50.0f;
    private const float DEFAULT_MAX_SCALE = 200.0f;
    private const float DEFULAT_SCALE_SHRINK_PER_SECOND = 20f;
    private const float PLAYER_HEAL_AMNT = 100f;

    private float minScale;
    private float maxScale;
    private float shrinkPerSecond;
    private float curScale;

    /// <summary>The percentage of how "complete" this pool is.</summary>
    public float PercentFull 
    {
        get 
        {
            float scaleFromBottom = curScale - minScale;
            float maxDiff = maxScale - minScale;
            return scaleFromBottom / maxDiff;
        }
    }


    private void Update()
    {
        if (curScale <= minScale)
        {
            // Set this gameObject to inactive if the scale has been decreased to the minimum
            
            OnDespawn();
        }
        else
        {
            // Shrink if player is in the pool
            Shrink(shrinkPerSecond * Time.deltaTime);
        }
    }

    /// <summary>Reinitializes and turns on this HealthPool gameObject.</summary>
    /// <param name="shrinkPerSecond">The amount at which the scale is reduced per second.</param>
    /// <param name="startScale">The scale at which this healthpool starts at.</param>
    /// <param name="minScale">The minimum scale which the healthpool can shrink to before it is despawned.</param>
    public void Init(float shrinkPerSecond = DEFULAT_SCALE_SHRINK_PER_SECOND, float startScale = DEFAULT_MAX_SCALE, float minScale = DEFAULT_MIN_SCALE) 
    {
        this.maxScale = startScale;
        this.minScale = minScale;
        this.shrinkPerSecond = shrinkPerSecond;

        SetScale(startScale);
        this.gameObject.SetActive(true);
    }

    protected override void OnDespawn() 
    {
        this.gameObject.SetActive(false);
        base.OnDespawn();
    }

    /// <summary>
    /// Should call the default Init method.
    /// </summary>
    public override void Init()
    {
        this.Init();
    }


    /// <summary>Updates the current scale.</summary>
    /// <param name="scale">The scale at which to set this object to.</param>
    private void SetScale(float scale)
    {
        curScale = ClampScale(scale);
        Vector3 objScale = new Vector3(curScale, 1, curScale);

        transform.localScale = objScale;
    }

    /// <summary>Shrinks the regen scale by amnt.</summary>
    /// <param name="amnt">The amount of points to reduce curScale by.</param>
    private void Shrink(float amnt)
    {
        SetScale(curScale - amnt);
    }

    /// <summary>Clamps amnt between MIN_SCALE and MAX_SCALE.</summary>
    /// <param name="amnt">The unit to be clamped.</param>
    /// <returns>The clamped amnt.</returns>
    private float ClampScale(float amnt)
    {
        return Mathf.Clamp(amnt, minScale, maxScale);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Health playerHealthRef = other.GetComponentInChildren<Health>();
            playerHealthRef.Heal(PLAYER_HEAL_AMNT);
            OnDespawn();
            //Debug.Log("Player entered region!");
        }
    }
}
