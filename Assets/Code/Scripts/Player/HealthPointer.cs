using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates to point toward the healthpool. Used as an indicator of where the healthpool is. 
/// Assumes 1 healthpool in world.
/// </summary>
public class HealthPointer : MonoBehaviour
{
    private HealthPool pool;


    #region Out of Bounds Arrows
    [SerializeField] private GameObject outOfBoundsArrows;
    private Coroutine runningCoroutine = null;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(HealthPoolInWorld());
        outOfBoundsArrows.SetActive(false);

        BoundsChecker boundsChecker = FindObjectOfType<BoundsChecker>();
        if (boundsChecker != null) 
        {
            boundsChecker.OnTimerStateChanged += EnableOutOfBoundsArrows;

            if (GameStateController.StateExists) 
            {
                GameStateController.playerDead.notifyListenersEnter += KillOutOfBoundsArrows;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        PointAtPool();
    }

    /// <summary>
    /// Checks to see if the healthpool is in the world.
    /// </summary>
    /// <returns>If the pointer should be enabled</returns>
    private bool HealthPoolInWorld()
    {
        pool = FindObjectOfType<HealthPool>();
        if (pool == null)
        {
            // Disable this object
            return false;
        }
        return true;
    }

    /// <summary>
    /// Rotates this gameobject to point toward the healthpool in the world.
    /// </summary>
    private void PointAtPool() 
    {
        if (pool != null)
        {
            Vector3 positionToLook = new Vector3(pool.transform.position.x, transform.position.y, pool.transform.position.z);
            transform.LookAt(positionToLook);
        }
    }

    #region Out Of Bounds Arrows
    /// <summary>
    /// Turns out of bounds arrows on and off
    /// </summary>
    /// <param name="enabled"></param>
    public void EnableOutOfBoundsArrows(bool enabled) 
    {
        if (gameObject.active == false) 
        {
            return;
        }

        outOfBoundsArrows.SetActive(enabled);

        if (enabled == false && runningCoroutine != null) 
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        else if (enabled == true && runningCoroutine == null) 
        {
            runningCoroutine = StartCoroutine(OutOfBoundsArrowsBlinkingBehavior());
        }
    }

    private void KillOutOfBoundsArrows() 
    {
        EnableOutOfBoundsArrows(false);
    }

    /// <summary>
    /// Blinks the Out of Bounds arrows
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator OutOfBoundsArrowsBlinkingBehavior() 
    {
        bool isOn = false;
        while (true) 
        {
            isOn = !isOn;
            outOfBoundsArrows.SetActive(isOn);
            yield return new WaitForSeconds(.5f);

        }
    }
    #endregion
}
