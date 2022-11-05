using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is used to activate and deactivate hitboxes for weapons and ramming zones as well as other abilities 
/// </summary>
public class HitBoxScript : MonoBehaviour
{
    /// <summary>
    /// This script searchers for all colliders in child of the prefab it is attached to. It will then activate and deactivate all of them when it's method is called. 
    /// </summary>
    // This is the list that stores all the hitboxes. 
    private List<Collider> C;


    public void Awake()
    {
        Collider[] cols = GetComponentsInChildren<Collider>();
        C.AddRange(cols);
    }

    /// <summary>
    /// This Method is called to activate each hitbox 
    /// </summary>
    public void Activate()
    {
        C.ForEach(ActivateHitbox);
    }

    /// <summary>
    /// This is a submethod that takes in each collider as a parameter and activates them
    /// </summary>
    /// <param name="c">This is the one of the coliders in the child of the prefab passed in throught the Activate Method</param>
    void ActivateHitbox(Collider c)
    {
        c.enabled = true;
    }
    

    /// <summary>
    /// This method is called to Deactive all hitboxes
    /// </summary>
    public void DeActivate()
    {
        C.ForEach(DeActivateHitbox);
    }

    /// <summary>
    /// This is a submethod that tackes in each  collider as a parameter and deactivates them
    /// </summary>
    /// <param name="c">This is the one of the coliders in the child of the prefab passed in throught the Activate Method</param>
    void DeActivateHitbox(Collider c)
    {
        c.enabled = false;
    }


}
