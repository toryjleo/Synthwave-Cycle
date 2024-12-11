using EditorObject;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupInstantiateData : IPoolableInstantiateData 
{
    public PickupInstantiateData(EditorObject.Arsenal arsenal) 
    {
        this.arsenal = arsenal;
    }

    private EditorObject.Arsenal arsenal;

    public EditorObject.Arsenal Arsenal { get { return arsenal; } }
}


public class Pickup : Poolable
{
    /// <summary>
    /// Used to reference alll gun objects. This data comes from the pool.
    /// Can set this in the Unity editor if manually dropping in pickups to world without pool.
    /// </summary>
    [SerializeField] private EditorObject.Arsenal arsenal;
    Renderer renderer = null;
    GunStats gun = null;

    private void Start()
    {
        TestSceneInit();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

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
        Color color = AssignRandomGun();

        if (renderer)
        {
            Material material = new Material(renderer.material);
            material.color = color;
            renderer.material = material;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Arsenal arsenal = other.gameObject.GetComponentInChildren<Arsenal>();
        if (arsenal && other.tag == "Player") 
        {
            if (arsenal.EquipGun(gun)) 
            {
                DespawnPickup();
            }
        }
    }

    private Color AssignRandomGun() 
    {
        DefinedGun[] allGuns = arsenal.AllUnlockableGuns;
        int idx = 1; //TODO: uncomment Random.Range(0, allGuns.Length);
        DefinedGun gunToAssign = allGuns[idx];
        gun = gunToAssign.stats;
        return gunToAssign.barrelColor;
    }

    private void DespawnPickup() 
    {
        gameObject.SetActive(false);
        //TODO: Set gun barrel to white
        //TODO: Set gun to null
    }
}
