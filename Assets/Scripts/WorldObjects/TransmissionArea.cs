using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmissionArea : MonoBehaviour
{
    [SerializeField] private GameObject prefab_HealthPool;

    [SerializeField] private float radius = 4;

    // Start is called before the first frame update
    void Start()
    {
        // Adjusts visual and capsule collider
        transform.localScale = new Vector3(radius, transform.localScale.y, radius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
