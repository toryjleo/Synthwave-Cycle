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
    static Generic.ObjectPool pool = null;

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
        if (instance == null)
        {
            Init(this.testArsenal);
        }
    }

    public void Init(EditorObject.Arsenal arsenal)
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else 
        {
            instance = this;
        }

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

    public static void SpawnAtLocation(Vector3 location) 
    {
        if (pool == null)
        {
            Debug.LogWarning("Attempting to spawn a pickup with uninitialized pool");
        }
        else 
        {
            Pickup pickup = pool.SpawnFromPool() as Pickup;
            pickup.transform.position = location;
        }
    }
}
