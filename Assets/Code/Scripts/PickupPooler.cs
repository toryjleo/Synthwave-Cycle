using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton wrapper for a pool of pickups.
/// Can be called to place pickups where we need them.
/// </summary>
public class PickupPooler : MonoBehaviour, IResettable
{
    #region Instancing
    private static PickupPooler instance;
    public static PickupPooler Instance { get { return instance; } private set { instance = value; } }
    #endregion

    #region Pooling
    private const int INSTANTIATE_COUNT = 25;

    /// <summary>
    /// Pool this object is wrapping
    /// </summary>
    Generic.ObjectPool pool = null;

    /// <summary>
    /// Prefab to pool
    /// </summary>
    [SerializeField] private Pickup prefab;

    /// <summary>
    /// Used only when PickupPooler is in a scene without LevelLoader
    /// </summary>
    [SerializeField] EditorObject.Arsenal testArsenal = null;
    #endregion

    private void Awake()
    {
        if (instance == null && GameStateController.CanRunGameplay)
        {
            Init(this.testArsenal);
        }
    }

    public void Init(EditorObject.Arsenal arsenal)
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of PickupPooler. Destroying this one: " + this.name);
            Destroy(this.gameObject);
        }
        Instance = this;

        if (arsenal == null) 
        {
            Debug.LogError("Handed PickupPooler a null Arsenal");
        }
        else 
        {
            PickupInstantiateData data = new PickupInstantiateData(arsenal);

            pool = new Generic.ObjectPool(data, prefab);
            pool.PoolObjects(INSTANTIATE_COUNT);
        }
    }

    public void ResetGameObject()
    {
        if (pool != null) 
        {
            pool.ResetGameObject();
        }
        else 
        {
            Debug.LogError("Trying to reset PickupPooler before it was initialized");
        }
    }

    public void SpawnAtLocation(Vector3 location) 
    {
        Pickup pickup = pool.SpawnFromPool() as Pickup;
        pickup.transform.position = location;
    }
}
