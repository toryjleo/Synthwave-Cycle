using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void NotifyDespawnConditionMet();

public class HealthPool : MonoBehaviour
{
    // Visuals
    [SerializeField] private CircleRendererScript circleRenderer;


    private float minScale = 0;
    private float maxScale = 0;
    private float shrinkPerSecond = 0;
    private float curScale = 0;



    public NotifyDespawnConditionMet onDespawnConditionMet;


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
        if (GameStateController.CanRunGameplay)
        {
            if (curScale <= minScale)
            {
                onDespawnConditionMet?.Invoke();
            }
            else
            {
                Shrink(shrinkPerSecond * Time.deltaTime);
                circleRenderer.DrawCircle(transform.position, 80, (transform.localScale.x));
            }
        }
    }

    /// <summary>Reinitializes and turns on this HealthPool gameObject.</summary>
    /// <param name="shrinkPerSecond">The amount at which the scale is reduced per second.</param>
    /// <param name="startScale">The scale at which this healthpool starts at.</param>
    /// <param name="minScale">The minimum scale which the healthpool can shrink to before it is despawned.</param>
    public void Init(float startScale, float minScale, float yScale, float shrinkPerSecond) 
    {
        this.maxScale = startScale;
        this.minScale = minScale;
        transform.localScale = new Vector3(transform.localScale.x, yScale, transform.localScale.z); 
        this.shrinkPerSecond = shrinkPerSecond;

        SetScale(startScale);
    }

    #region ScaleCode
    /// <summary>Updates the current scale.</summary>
    /// <param name="scale">The scale at which to set this object to.</param>
    private void SetScale(float scale)
    {
        curScale = ClampScale(scale);
        Vector3 objScale = new Vector3(curScale, transform.localScale.y, curScale);

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
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit " + other.name);
        if (other.tag == "Player")
        {
            PlayerHealth playerHealthRef = other.GetComponentInChildren<PlayerHealth>();
            playerHealthRef.HealFromHealthPool();
            PickupPooler.Instance.SpawnAtLocation(transform.position);
            onDespawnConditionMet?.Invoke();
        }
    }
}
