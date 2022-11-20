using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : SelfWorldBoundsDespawn
{
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Despawn += op_SelfDelete;
    }

    /// <summary>
    /// Weapon drops clean themselves up
    /// </summary>
    /// <param name="entity">This GameObject</param>
    public void op_SelfDelete(SelfDespawn entity)
    {
        Destroy(entity.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player hit pickup!");
            BikeScript bs = other.gameObject.GetComponent<BikeScript>();
            if (bs == null) 
            {
                Debug.Log("Fuck");
            }
            else
            {
                Debug.Log("Yay");
                OnDespawn();
            }
        }
    }
}
