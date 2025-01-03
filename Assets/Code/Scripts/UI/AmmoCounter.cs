using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script used by GameplayUI to manage the ammo slider and overheat progress slider visually
/// </summary>
public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Slider ammoCounter;
    [SerializeField] private Slider overheatCounter;
    [SerializeField] private Image overheatFill;

    /// <summary>
    /// Manages the state of the gun
    /// </summary>
    GunState.StateController stateController = null;
    private Gun.Gun playerGun;

    // Start is called before the first frame update
    void Start()
    {
        playerGun = FindObjectOfType<Gun.Gun>();
        if (playerGun == null)
        {
            Debug.LogWarning("No gun found for ammo counter!");
            // Disable this object
            ammoCounter.gameObject.SetActive(false);
        }
        stateController = playerGun.GunStateController;

        if (playerGun.IsInfiniteAmmo)
        {
            ammoCounter.gameObject.SetActive(false);
        }
        else
        {
            ammoCounter.gameObject.SetActive(true);
        }

        if (playerGun.IsOverheat)
        {
            overheatCounter.gameObject.SetActive(true);
        }
        else
        {
            overheatCounter.gameObject.SetActive(false);
        }

        SetAmmoCounter(playerGun);
        overheatCounter.value = 0f;
        overheatFill.color = Color.cyan;

        HookUpListeners();
    }

    // Update is called once per frame
    void Update()
    {
        overheatCounter.value = playerGun.OverheatPercent / 100f;
    }

    /// <summary>
    /// Hooks up listeners for gun state events
    /// </summary>
    private void HookUpListeners()
    {
        stateController.overHeated.notifyListenersEnter += HandleOverheatedEnter;
        stateController.overHeated.notifyListenersExit += HandleOverheatedExit;
        playerGun.onAmmoChange += SetAmmoCounter;
    }

    private void SetAmmoCounter(Gun.Gun gun)
    {
        ammoCounter.value = (float)gun.AmmoCount / gun.MaxAmmo;
    }

    private void HandleOverheatedEnter()
    {
        overheatFill.color = Color.red;
    }

    private void HandleOverheatedExit()
    {
        overheatFill.color = Color.cyan;
    }
}
