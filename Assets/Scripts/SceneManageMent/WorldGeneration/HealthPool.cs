using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : MonoBehaviour
{
    Health playerHealthRef;
    private bool playerInThisPool;

    // Visuals
    private const float MIN_SCALE = 150;
    private const float MAX_SCALE = 300.0f;
    private const float SCALE_SHRINK_PER_SECOND = 3f;
    private const float PLAYER_LINEAR_HEALTH_INCREASE_PER_SECOND = 3f;

    private float curScale;


    public bool PlayerIsInThisPool 
    {
        get { return playerHealthRef != null; }
    }


    private void Update()
    {
        if (curScale <= MIN_SCALE) 
        {
            this.gameObject.SetActive(false);
        }
        if (PlayerIsInThisPool)
        {
            Shrink(SCALE_SHRINK_PER_SECOND * Time.deltaTime);
            playerHealthRef.Heal(PLAYER_LINEAR_HEALTH_INCREASE_PER_SECOND * Time.deltaTime);
        }
    }


    public void Init() 
    {
        playerInThisPool = false;
        SetScale(MAX_SCALE);
        playerHealthRef = null;
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
        return Mathf.Clamp(amnt, MIN_SCALE, MAX_SCALE);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInThisPool = true;
            playerHealthRef = other.GetComponentInChildren<Health>();
            //Debug.Log("Player entered region!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInThisPool = false;
            playerHealthRef = null;
            //Debug.Log("Player exited region!");
        }
    }
}
