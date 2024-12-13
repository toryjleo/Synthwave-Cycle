using EditorObject;
using Generic;
using Gun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngineInternal;

/// <summary>
/// Contains data needed to initialize a pickup
/// </summary>
public class PickupInstantiateData : IPoolableInstantiateData 
{
    public PickupInstantiateData(EditorObject.Arsenal arsenal) 
    {
        this.arsenal = arsenal;
    }

    private EditorObject.Arsenal arsenal;

    public EditorObject.Arsenal Arsenal { get { return arsenal; } }
}

/// <summary>
/// A persistant world object that will represents a gun in-world
/// </summary>
public class Pickup : Poolable
{
    /// <summary>
    /// Used to reference alll gun objects. This data comes from the pool.
    /// Can set this in the Unity editor if manually dropping in pickups to world without pool.
    /// </summary>
    [SerializeField] private EditorObject.Arsenal arsenal;
    Renderer renderer = null;
    GunStats gun = null;

    [SerializeField] private AmmoCount ammoCount = null;

    private void Start()
    {
        TestSceneInit();
    }

    #region Monobehavior

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    private void OnTriggerStay(Collider other)
    {
        Arsenal arsenal = other.gameObject.GetComponentInChildren<Arsenal>();
        if (arsenal && other.tag == "Player")
        {
            this.AttemptToConsume(arsenal);
        }
    }
    #endregion

    public override void Init(IPoolableInstantiateData stats)
    {
        PickupInstantiateData data = stats as PickupInstantiateData;
        arsenal = data.Arsenal;

        renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Pickup needs a renderer for a visual");
        }
    }

    /// <summary>
    /// Used to initialize a Pickup object that does not come from a pool
    /// </summary>
    private void TestSceneInit()
    {
        // Can assume Init() not called if gun is null
        if (gun == null)
        {
            renderer = GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("Pickup needs a renderer for a visual");
            }

            if (arsenal == null)
            {
                Debug.LogError("Must reference an EditorObject.Arsenal when spawing without pool");
            }
            else
            {
                Reset();
            }
        }
    }

    /// <summary>
    /// Called before removed from pool.
    /// </summary>
    public override void Reset()
    {
        UnityEngine.Color color = AssignRandomGun();

        if (renderer)
        {
            Material material = new Material(renderer.material);
            material.color = color;
            renderer.material = material;
        }
    }


    /// <summary>
    /// Attempts to consume this pickup
    /// </summary>
    /// <param name="arsenal">The arsenal attempting to consume this pickup</param>
    private void AttemptToConsume(Arsenal arsenal) 
    {
        int bulletsLeft = arsenal.ConsumePickup(gun, ammoCount.Count);
        if (bulletsLeft <= 0)
        {
            DespawnPickup();
        }
        else
        {
            ammoCount.SetAmmo(bulletsLeft);
        }
    }

    /// <summary>
    /// Assigns this pickup a random gun and returns that gun's color
    /// </summary>
    /// <returns>A color which represents a gun</returns>
    private UnityEngine.Color AssignRandomGun() 
    {
        DefinedGun[] allGuns = arsenal.AllUnlockableGuns;
        int idx = Random.Range(0, allGuns.Length);
        DefinedGun gunToAssign = allGuns[idx];
        gun = gunToAssign.stats;
        ammoCount = new AmmoCount(gun);
        return gunToAssign.barrelColor;
    }

    /// <summary>
    /// Clears data used by this object
    /// </summary>
    private void DespawnPickup() 
    {
        gameObject.SetActive(false);
        gun = null;
        if (renderer)
        {
            Material material = new Material(renderer.material);
            material.color = UnityEngine.Color.grey;
            renderer.material = material;
        }
    }
}
